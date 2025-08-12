using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using KinematicCharacterController;
using UnityEngine.AI;

public abstract class Entity : MonoBehaviour
{
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("References")] public StateMachine StateMachine { get; protected set; }
    [field: SerializeField, TabGroup("References")] public KinematicCharacterMotor Motor { get; private set; }
    [field: SerializeField, TabGroup("References")] public HealthComponent Health { get; private set; }
    [field: SerializeField, TabGroup("References")] public StaminaComponent Stamina { get; private set; }
    [field: SerializeField, TabGroup("References")] public Animator Animator { get; private set; }
    [field: SerializeField, TabGroup("References")] public EntityAnimatorController AnimatorController { get; private set; }
    [field: SerializeField, TabGroup("References")] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField, TabGroup("Flags")] public bool HasBeenInitialized { get; private set; }
    [field: SerializeField, TabGroup("Properties")] public EntityStats EntityStats { get; private set; }
    [field:SerializeField, TabGroup("Properties")] public Transform CenterOfMassPivot { get; protected set; }
    [field: SerializeField, TabGroup("AI")] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: SerializeField, Tooltip("Used in particle system to spawn hurt particles")] public string hurtParticle { get; private set; } = "AttackMud";


    public virtual void Initialize()
    {
        HasBeenInitialized = true;
        
        GetNullComponents();

        Health.Initialize();

        Health.OnHeal += Heal;
        Health.OnDead +=  Death;
        Health.OnDamage += Damage;

        Stamina.OnConsume += ConsumeStamina;
        Stamina.OnUpdate += UpdateStamina;
        Stamina.OnPostureBreak += PostureBroken;
        
        AnimatorController.Initialize(this);

        InitializeStateMachine();
    }
    [Button]
    private void GetNullComponents()
    {
        Rigidbody ??= GetComponent<Rigidbody>();
        Health ??= GetComponent<HealthComponent>();
        Stamina ??= GetComponent<StaminaComponent>();
        Animator ??= GetComponentInChildren<Animator>();
        Motor ??= GetComponent<KinematicCharacterMotor>();
        AnimatorController ??= GetComponentInChildren<EntityAnimatorController>();
    }
    public virtual void Terminate()
    {
        Health.OnDamage -= Damage;
        Health.OnHeal -= Heal;
        Health.OnDead -=  Death;
        
        Stamina.OnConsume -= ConsumeStamina;
        Stamina.OnPostureBreak -= PostureBroken;
    }
    public void PushBack(Vector3 dir)
    {
        Motor.SetPosition(Motor.transform.position+ dir);
    }
    protected IEnumerator FadeAndDestroy()
    {
        //TODO: RAGDOLL??
        
        yield return new WaitForSeconds(6f);
        
        Terminate();

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
    protected virtual void FixedUpdate() => StateMachine?.FixedUpdate();
    protected virtual void Update() => StateMachine?.Update();
    protected abstract void InitializeStateMachine();
    public abstract void Stagger();
    public void SetInvulnerability(bool value) => Health.SetInvulnerability(value);
    protected virtual void PostureBroken() => Stagger();
    protected virtual void UpdateStamina(float obj){}
    protected virtual void ConsumeStamina(float obj){}
    protected virtual void Heal(HealthComponent.HealthModificationReport obj){}
    protected virtual void Damage(HealthComponent.HealthModificationReport obj){}
    protected virtual void Death(HealthComponent.HealthModificationReport obj)
    {
        AnimatorController.Terminate();

        StartCoroutine(FadeAndDestroy());
    }
}