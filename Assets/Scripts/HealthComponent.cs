using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum DamageTypes
{
	None,
	SelfDestruct,
	Environmental,
	Enemy,
	Boss
}
public class HealthComponent : MonoBehaviour
{
	public static event Action<HealthComponent> OnAnyDead;
	public event Action<HealthModificationReport> OnDamage;
	public event Action<HealthModificationReport> OnDead;
	public event Action<HealthModificationReport> OnHeal;
	public event Action<HealthModificationReport> OnRevive;
	public event Action<HealthModificationReport> OnUpdate;

	[SerializeField] private Mode mode;

	[Header("Stacks"), SerializeField] private int startingStacks = 3;
	[Header("Debug"), SerializeField] private bool printDebugs;
	[field: SerializeField] public int Max { get; private set; }

	private readonly HealthModificationReport lastModification = new HealthModificationReport();
    private HealthModificationReport deathReport;
    

    [ShowInInspector] public int Current { get; private set; }
    [ShowInInspector, ReadOnly] public bool IsCritical { get; private set; }

    [ShowInInspector, ReadOnly] private float _damageMultiplier = 1;
    public void SetDamageMultiplier(float damageMultiplier) => _damageMultiplier = damageMultiplier;
    public bool SetCriticalState(bool value) => IsCritical = value;

    public void SetMaxHealth(int newMax, bool shouldFullHeal = false)
    {
	    Max += newMax;

	    if (shouldFullHeal)
	    {
			FullHeal(gameObject);
	    }
    }

    public void ResetHealth(int newMax, bool shouldFullHeal = false)
    {
	    Max = newMax;
        Current = Max;

	    if (shouldFullHeal)
	    {
			FullHeal(gameObject);
	    }
    }

	public float CurrentPercentage => (float)Current / Max;

	public bool IsDead => Current  <= 0;
	public bool IsAlive => Current > 0;
	public bool IsFull => Current >= Max;

    public HealthModificationReport DeathReport => deathReport;
	public Mode HealthMode => mode;
	public int Missing => Max - Current;
	public bool IsInvulnerable { get; private set; }
	public void SetInvulnerability(bool value) => IsInvulnerable = value;
	public void Initialize(Mode healthMode) => Initialize(healthMode, Max);
	public void Initialize(Mode healthMode, int maxHealth)
	{
		Max = maxHealth;
        mode = healthMode;
        Current = mode == Mode.Default ? Max : startingStacks;
    }

    public void InitializeAgonizingMode(Mode healthMode, int maxHealth)
    {
        Initialize(healthMode, maxHealth);
    }

	public void Initialize() => Initialize(mode);

	private void OnDestroy()
    {
        if(IsAlive && isActiveAndEnabled) InstantKill(gameObject, DamageTypes.SelfDestruct);
    }

	[Button, FoldoutGroup("Buttons")]
	public void Heal(int amount, GameObject source)
    {
        if (amount < 0) throw new Exception("Negative Heal");
        
        if (IsDead)
        {
            if (!GameManager.disableLogs) Debug.LogWarning($"{this} is already dead. Source:{source}", this);
            return;
        }

        Modify(amount, source, DamageTypes.None);
    }

	[Button, FoldoutGroup("Buttons")]
	public void Damage(int amount, GameObject source, DamageTypes damageType = DamageTypes.None)
    {
	    if (IsInvulnerable)
	    {
            if (!GameManager.disableLogs) Debug.LogWarning($"{this} is Is Invulnerable. Source:{source}", this);
		    return;
	    }
	    
        if (IsDead)
        {
            if (!GameManager.disableLogs) Debug.LogWarning($"{this} is already dead. Source:{source}", this);
            return;
        }
        
        if (amount < 0) throw new Exception("Negative Damage");
        
		Modify(IsCritical ? -(int)(amount + _damageMultiplier * 100 / amount) : -amount, source, damageType);
    }

	[ResponsiveButtonGroup, FoldoutGroup("Buttons")]
	public void FullHeal() => FullHeal(gameObject);
	public void FullHeal(GameObject source) => Heal(Max - Current, source);

	[Button, FoldoutGroup("Buttons")]
	public void InstantKill(DamageTypes type = DamageTypes.SelfDestruct) => InstantKill(gameObject, type);
    public void InstantKill(GameObject source, DamageTypes damageType)
    {
        var oldMode = mode;
        mode = Mode.Default;
        Damage(Current, source, damageType);
        mode = oldMode;
    }

    public void ReviveAndFullHeal(GameObject source) => Modify(Max - Current, source,  DamageTypes.None);

    private void Modify(int amount, GameObject source, DamageTypes damageType = DamageTypes.None)
    {
	    if (amount == 0)
	    {
            GameManager.ShowLog($"{name} Health Modification is 0. Ignored");
		    return;
	    }

        if (mode == Mode.Stacks) amount = Mathf.Clamp(amount, -1, 1);

        lastModification.DamageSource = source;
        lastModification.Target = this;
        lastModification.StartHealth = Current;
        lastModification.Modification = amount;

        lastModification.DamageType = damageType;
        lastModification.Mitigated = 0;

        if (lastModification.Modification == 0)
        {
	        //Modification Nullified
	        OnUpdate?.Invoke(lastModification);
	        return;
        }

        var prevHealth = Current;
        Current = Mathf.Clamp(Current+lastModification.Modification,0,Max);

        lastModification.Modification = Current - prevHealth;

        OnUpdate?.Invoke(lastModification);
        
        switch (amount)
        {
            case > 0:
                OnHeal?.Invoke(lastModification);
                if(lastModification.StartHealth <= 0 && Current > 0)
                    OnRevive?.Invoke(lastModification);
                break;
            case < 0:
                OnDamage?.Invoke(lastModification);
                if (Current <= 0)
                {
                    deathReport = lastModification;
                    OnDead?.Invoke(lastModification);
                    OnAnyDead?.Invoke(this);
                }
                break;
        }
        
        #if UNITY_EDITOR
        if(printDebugs)
            GameManager.ShowLog(lastModification.ToString());
        #endif
    }

    [System.Serializable]
    public class HealthModificationReport
    {
	    //Origen del daño (Usualmente un Body)
	    public GameObject DamageSource;
	    //Medio por el cual el daño fue transmitido. Ej: Un projectil, o una Skill
	    public GameObject DamageMedium;
	    public HealthComponent Target;
	    public int StartHealth;
	    public int Mitigated;
	    public int Modification;

	    public DamageTypes DamageType = DamageTypes.None;
	    [ShowInInspector] public bool IsHeal => Modification   > 0;
	    [ShowInInspector] public bool IsDamage => Modification < 0;
	    [ShowInInspector] public bool IsLethal => StartHealth > 0 && EndHealth <=0;
	    public int EndHealth => StartHealth + Modification;

	    public override string ToString() => $"<color=green>|HP-Report|</color> Source:{DamageSource} Target:{Target.name} Health:{StartHealth}+{Modification}={EndHealth} WasLethal:{IsLethal} DamageType:{DamageType}";

	    public HealthModificationReport Clone()
        {
            return new HealthModificationReport()
            {
                DamageSource = DamageSource,
                Target = Target,
                StartHealth = StartHealth,
                Modification = Modification,
                Mitigated = Mitigated,
            };
        }

	    public void Mitigate(int amount)
	    {
		    Mitigated -= amount;
		    Modification -= amount;
	    }

	    public void MitigateAll() => Mitigate(Modification);
    }

    public enum Mode { Default, Stacks }

}