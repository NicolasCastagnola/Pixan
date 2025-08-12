using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using Audio;
using Unity.VisualScripting;

public class MudEnemyMelee : Entity
{

    [field: SerializeField, TabGroup("AI")] public  float cooldown;
    [HideInInspector]public float currentCooldown;

    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("References")] private EnemyManager Owner;

    private Vector3 _initialPos;
    public Vector3 InitialPos { get => _initialPos; }
    private Quaternion _initialRot;

    [SerializeField, TabGroup("References")] private UIEnemy EnemyGUI;
    [field: SerializeField, TabGroup("Entity")] public KineCharacterController Controller { get; private set; }
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("Combat")] public Player PlayerTarget { get; private set; }
    [field:SerializeField, TabGroup("Combat")] public EnemyColliderDamage MeleeHitBox { get; private set; }
    [field:SerializeField, Range(5, 30), TabGroup("Combat")] public float AwarenessViewRadius { get; private set; } = 18;
    [field:SerializeField, Range(5, 30), TabGroup("Combat")] public float LostRadius { get; private set; } = 10;
    [field:SerializeField, Range(5, 100), TabGroup("Combat")] public int MeleeDamage { get; private set; } = 18;
    [field:SerializeField, Range(0.5f, 5), TabGroup("Combat")] public float ValidRangeToAttack { get; private set; } = 1f;
    [field:SerializeField, Range(1, 5), TabGroup("Combat")] public float AttackTimeInterval { get; private set; } = 1f;
    [field:SerializeField, Range(120, 360), TabGroup("Combat")] public float FieldOfView { get; private set; } = 120f;
    [field:SerializeField, Range(50, 180), TabGroup("Combat")] public float RotationSpeed { get; private set; } = 10f;

    [TabGroup("Combat")] public LayerMask playerLayer;
    [TabGroup("Combat")] public LayerMask detectionLayer;

    [field: SerializeField, TabGroup("Audio")] public AudioConfigurationData Hurt, Attack, DeathSound;


    public void Initialize(Player player, EnemyManager owner = null)
    {
        Owner = owner;
        PlayerTarget = player;
        EnemyGUI.Initialize(PlayerTarget.PlayerCamera.transform, this);
        
        MeleeHitBox.SetDamage(MeleeDamage);

        NavMeshAgent.speed = EntityStats.rawWalkSpeed;
        NavMeshAgent.angularSpeed = RotationSpeed;

        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();


        _initialPos=transform.position;
        _initialRot=transform.rotation;

    Controller ??= GetComponent<KineCharacterController>();
    }
    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();
        
        StateMachine.AddState(States.EnemyIdle, new MeleeEnemyIdleState(this));
        StateMachine.AddState(States.EnemyChase, new MeleeEnemyChaseState(this));
        StateMachine.AddState(States.EnemyAttack, new MeleeEnemyAttackState(this, AttackTimeInterval));
        StateMachine.AddState(States.EnemyStagger, new MeleeEnemyStunState(this));
        StateMachine.AddState(States.EnemyHurt, new MeleeEnemyHurtState(this));
        StateMachine.AddState(States.EnemyDeath, new MeleeEnemyDeathState(this));
        StateMachine.AddState(States.Paused, new EnemyPauseState<MudEnemyMelee>(this));

        StateMachine.ChangeState(States.EnemyIdle);

        //Initialize animation states
        foreach (var behaviour in Animator.GetBehaviours<MudEnemyBaseState>())
        {
            behaviour.Initialize(this);
        }
    }
    public override void Stagger()
    {
        StateMachine.ChangeState(States.EnemyStagger);
    }
    public void SetAwarenessViewRadius(float value) => AwarenessViewRadius = value;
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        base.Death(obj);
        
        StateMachine.ChangeState(States.EnemyDeath);
        
        EnemyGUI.Terminate();
        
        Rigidbody.useGravity = false;
        Rigidbody.detectCollisions = false;

        if (Owner != null)
        {
            Owner.RemoveMudMeleeEnemy(this);
        }
    }

    protected override void Update()
    {
        if (Health.IsDead) return;
        
        base.Update();
    }
    public bool PlayerWithinAwarenessRadius() 
        => Physics.CheckSphere(transform.position, LostRadius, playerLayer);

    public bool PlayerCanBeSeen()
    {
        if (Player.Instance == null)
            return false;

        RaycastHit hit;
        var dir = Player.Instance.transform.position - transform.position;
        if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, out hit, AwarenessViewRadius, detectionLayer))
        {
            if (hit.transform.root.CompareTag("Player"))
                return true;
        }
        //else return false;
        return false;
    }
    protected override void Damage(HealthComponent.HealthModificationReport obj)
    {
        base.Damage(obj);
        
        if (StateMachine.GetCurrentState() == States.EnemyHurt) return;
            
        StateMachine.ChangeState(States.EnemyHurt);
    }

    private string currentAnimationKey;
    public void PlayAnimation(string key) {
        if (currentAnimationKey == "Death") return;
        Animator.SetBool(key, true);
        if(currentAnimationKey != null) Animator.SetBool(currentAnimationKey, false);
        currentAnimationKey = key;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AwarenessViewRadius);
    }

    public bool EnemyPreValidations()
    {
        //if(StateMachine.GetCurrentState()!= States.EnemyAttack) MeleeHitBox.Activate();

        //if (PlayerTarget) return false;

        if (PlayerTarget!=null && PlayerTarget.isDead)
        {
            StateMachine = new StateMachine();
            PlayAnimation("IdleState");
            Motor.enabled = false;
            enabled = false;
            return true;
        }
        
        if(Health.IsDead && StateMachine.GetCurrentState() != States.EnemyIdle)
        {
            StateMachine.ChangeState(States.EnemyDeath);
            return true;
        }
        
        return false;
    }
    public bool WithInAttackRange() => Vector3.Distance(PlayerTarget.transform.position, transform.position) < ValidRangeToAttack;
    public void ReviveAndResetPosition()
    {
        StateMachine.ChangeState(States.EnemyIdle);
        Health.ReviveAndFullHeal(EnemyManager.Instance.gameObject);

        if (Motor != null)
        {
            gameObject.SetActive(true);

            Motor.enabled = true;
            Motor.Capsule.enabled = true;

            Rigidbody.useGravity = true;
            Rigidbody.detectCollisions = true;


            EnemyGUI.Initialize(PlayerTarget.PlayerCamera.transform, this);
            transform.GetChild(0).tag = "Enemy";

            Animator.SetBool("Death", false);

            Motor.SetPositionAndRotation(_initialPos, _initialRot);



            

        }
    }
}