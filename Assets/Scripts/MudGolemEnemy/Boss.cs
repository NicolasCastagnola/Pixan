using Audio;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
public abstract class Boss : Entity
{
    public BossFightArena BossFightArena { get; private set; }
    
    public bool IsInitialized { get; private set; }
    
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("References")] private EnemyManager Owner;
    [field: SerializeField, TabGroup("Entity")] public KineCharacterController Controller { get; private set; }
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly, TabGroup("Combat")] public Player PlayerTarget { get; private set; }
    [field:SerializeField, Range(5, 30), TabGroup("Combat")] public float AwarenessViewRadius { get; private set; } = 18;
    [field:SerializeField, Range(5, 30), TabGroup("Combat")] public bool Unstaggerable { get; private set; } = true;
    [field:SerializeField, Range(0.5f, 5), TabGroup("Combat")] public float ValidRangeToAttack { get; private set; } = 1f;
    [field:SerializeField, Range(1, 5), TabGroup("Combat")] public float AttackTimeInterval { get; private set; } = 1f;
    [field:SerializeField, Range(120, 180), TabGroup("Combat")] public float FieldOfView { get; private set; } = 120f;
    [field:SerializeField, Range(10f, 20), TabGroup("Combat")] public float RotationSpeed { get; private set; } = 10f;
    [field: SerializeField, TabGroup("Properties")] public string BossName { get; private set; } = "Boss";
    [field: SerializeField, TabGroup("Properties")] public string OnKillBossMessage { get; private set; } = "You Defeated";

    public void Initialize(Player player, EnemyManager owner)
    {
        Owner = owner;
        PlayerTarget = player;
        
        Initialize();
    }
    public void SetArena(BossFightArena bossFightArena) => BossFightArena = bossFightArena;
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        Canvas_Playing.Instance.GameEventMessage.ShowMessage(OnKillBossMessage, Color.yellow, 5f);

        base.Death(obj);
    }
    public override void Initialize()
    {
        base.Initialize();
        
        Controller ??= GetComponent<KineCharacterController>();
    }
    public virtual void InitializeAI()
    {
        IsInitialized = true;

        Canvas_Playing.Instance.BossHealthBar.Initialize(this);
    }

}