using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class MudEnemyDistance : Entity
{

    [field: SerializeField, TabGroup("AI")] public float cooldown;
    [HideInInspector] public float currentCooldown;


    private Vector3 _initialPos;
    public Vector3 InitialPos { get => _initialPos; }
    private Quaternion _initialRot;

    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("References")] private EnemyManager Owner;

    [SerializeField, TabGroup("References")] private UIEnemy EnemyGUI;
    [field: SerializeField, TabGroup("Entity")] public KineCharacterController Controller { get; private set; }
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("Combat")] public Player PlayerTarget { get; private set; }
    [field:SerializeField, TabGroup("Combat")] public EnemyColliderDamage MeleeHitBoxColliderDamage { get; private set; }
    [field:SerializeField, Range(5, 60), TabGroup("Combat")] public float DistanceViewRadius { get; private set; } = 18;
    [field: SerializeField, Range(5, 100), TabGroup("Combat")] public int MeleeDamage { get; private set; } = 18;
    [field: SerializeField, Range(5, 100), TabGroup("Combat")] public int DistanceDamage { get; private set; } = 18;
    [field:SerializeField, Range(1, 5), TabGroup("Combat")] public float MeleeViewRadius { get; private set; } = 18;
    [field:SerializeField, Range(1, 5), TabGroup("Combat")] public float DistanceAttackTimeInterval { get; private set; } = 1f;
    [field:SerializeField, Range(1, 5), TabGroup("Combat")] public float MeleeAttackTimeInterval { get; private set; } = 1f;
    [field:SerializeField, Range(120, 360), TabGroup("Combat")] public float FieldOfView { get; private set; } = 120f;
    [field:SerializeField, Range(50, 180), TabGroup("Combat")] public float RotationSpeed { get; private set; } = 10f;


    [field: SerializeField, TabGroup("Combat")] public EnemyColliderDamage MeleeHitBox { get; private set; }
    [SerializeField, TabGroup("Combat")] private Transform ThrowObjectPivot;
    [SerializeField, TabGroup("Combat")] private GameObject ThrowObjectPrefab;
    
    [TabGroup("Combat")] public LayerMask playerLayer;
    
    public void Initialize(Player player, EnemyManager owner)
    {
        Owner = owner;
        PlayerTarget = player;
        EnemyGUI.Initialize(PlayerTarget.PlayerCamera.transform, this);

        MeleeHitBox.SetDamage(MeleeDamage);

        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();

        _initialPos = transform.position;
        _initialRot = transform.rotation;

        NavMeshAgent.speed = EntityStats.rawWalkSpeed;
        NavMeshAgent.angularSpeed = RotationSpeed;

        Controller ??= GetComponent<KineCharacterController>();
    }
    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();
        
        StateMachine.AddState(States.EnemyIdle, new DistanceEnemyIdleState(this));
        //StateMachine.AddState(States.EnemyChase, new DistanceEnemyChaseState(this));
        StateMachine.AddState(States.EnemyAttack, new DistanceEnemyMeleeAttackState(this, MeleeAttackTimeInterval));
        StateMachine.AddState(States.EnemyDistanceAttack, new DistanceEnemyThrowAttackState(this, DistanceAttackTimeInterval));
        StateMachine.AddState(States.EnemyStagger, new DistanceEnemyStunState(this));
        StateMachine.AddState(States.EnemyHurt, new DistanceEnemyHurtState(this));
        StateMachine.AddState(States.EnemyDeath, new DistanceEnemyDeathState(this));
        StateMachine.AddState(States.Paused, new EnemyPauseState<MudEnemyDistance>(this));

        StateMachine.ChangeState(States.EnemyIdle);
    }
    public override void Stagger()
    {
        StateMachine.ChangeState(States.EnemyStagger);
    }
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        base.Death(obj);
        
        StateMachine.ChangeState(States.EnemyDeath);
        
        EnemyGUI.Terminate();
        transform.GetChild(0).tag = "Untagged";
        
        Rigidbody.useGravity = false;;
        Rigidbody.detectCollisions = false;;
        
        Owner!.RemoveMudDistanceEnemy(this);
    }

    protected override void Update()
    {
        if (Health.IsDead || StateMachine == null) return;
        
        base.Update();
    }
    public bool PlayerWithinDistanceAttackRadius() => Physics.CheckSphere(transform.position, DistanceViewRadius, playerLayer);
    public bool PlayerWithinMeleeAttackRadius() => Physics.CheckSphere(transform.position, MeleeViewRadius, playerLayer);
    protected override void Damage(HealthComponent.HealthModificationReport obj)
    {
        base.Damage(obj);
        
        if (StateMachine.GetCurrentState() == States.EnemyHurt) return;
            
        StateMachine.ChangeState(States.EnemyHurt);
    }
    public void PlayAnimation(string key)
    {
        if (key == "Death")
        {
            Animator.SetBool(key, true);
        } 
        
        else Animator.SetTrigger(key); 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, DistanceViewRadius);
    }

    public bool EnemyPreValidations()
    {
        //if (PlayerTarget) return false;

        if (PlayerTarget != null && PlayerTarget.isDead)
        {
            StateMachine = new StateMachine();
            PlayAnimation("IdleState");
            Motor.enabled = false;
            enabled = false;
            return true;
        }

        if (Health.IsDead && StateMachine.GetCurrentState() != States.EnemyIdle)
        {
            StateMachine.ChangeState(States.EnemyDeath);
            return true;
        }
        
        return false;
    }
    public void PrepareProjectile(){}
    public void ShootProjectile() => Instantiate(ThrowObjectPrefab, ThrowObjectPivot.position, Quaternion.identity).GetComponent<BulletController>().SetTarget(PlayerTarget.gameObject.transform,false,Vector3.up).SetDamage(DistanceDamage);
    
    public void ReviveAndResetPosition()
    {
        Health.ReviveAndFullHeal(EnemyManager.Instance.gameObject);

        if (Motor != null)
        {
            StateMachine.ChangeState(States.EnemyIdle);

            Motor.enabled = true;
            Motor.Capsule.enabled = true;

            Motor.SetPositionAndRotation(_initialPos,_initialRot);

            Rigidbody.useGravity = true;
            Rigidbody.detectCollisions = true;

            EnemyGUI.Initialize(PlayerTarget.PlayerCamera.transform, this);
            transform.GetChild(0).tag = "Enemy";

            Animator.SetBool("Death", false);

            gameObject.SetActive(true);
            //currentAnimationKey = null;
        }
    }
}   