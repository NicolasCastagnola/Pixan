using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;
using Sirenix.OdinInspector;

public class MudGolemEnemy : Boss
{
    private float summonTimer;
    public List<MudEnemyMelee> SummonedMinions { get; private set; } = new List<MudEnemyMelee>();
    
    private List<Transform> availableSpawnPoints = new List<Transform>();
    
    [SerializeField] private MudEnemyMelee SummonedMinionPrefab;
    
    [SerializeField] private float summonTimeLimit = 20f; 
    [field: SerializeField, Range(2, 4)] public int MaxSummons { get; private set; } = 2;
    public bool CanSummonsMinions { get; private set; }

    [SerializeField] private float makeSummonAvailableInterval = 45f;
    [ShowInInspector, ReadOnly, TabGroup("Combat")] public int RangeAttackToken { get; private set; }
    public bool HasThrowTokens => RangeAttackToken > 0;

    [SerializeField, TabGroup("Combat")] private float ThrowRange = 15f;
    [SerializeField, TabGroup("Combat")] private float StompRange = 5f;


    [SerializeField, Range(4, 10), TabGroup("Combat")] private float addTokenIntervals = 3f; 
    [SerializeField, Range(1, 3), TabGroup("Combat")] private int maxRangeAttackToken; 
    
    [Space(10), Range(10, 100), TabGroup("Combat")] public float viewRange;
    [Range(1, 10), TabGroup("Combat")] public float attackRange = 4f;
    [HideInInspector] public float chaseRange;
    
    public LayerMask playerLayer;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletInitialPosition;
    [SerializeField] private ParticleSystem deathParticlePrefab, SummonParticles, castSummonParticles;
    
    [SerializeField] private Transform deathParticleInitialPosition;

    [SerializeField, TabGroup("Sound")] private AudioConfigurationData deathSound, throwSound, stompSound, hurtSound, footstepSound, stunSound;
    [field:SerializeField, Range(5, 100), TabGroup("Damage")] public  int StompDamage { get; private set; }
    [field:SerializeField, Range(5, 100), TabGroup("Damage")] public  int ThrowDamage { get; private set; }
    
    public override void InitializeAI()
    {
        base.InitializeAI();

        StateMachine.ChangeState(States.EnemyIdle);

        StartCoroutine(RangeAttackTokenGenerator());
    }
    private IEnumerator RangeAttackTokenGenerator()
    {
        while (Health.IsAlive)
        {
            yield return new WaitForSeconds(addTokenIntervals);

            if (RangeAttackToken < maxRangeAttackToken)
            {
                RangeAttackToken++;
            }
        }
    }

    private void LateUpdate()
    {    
        if(!IsInitialized) return;
        
        summonTimer += Time.deltaTime;

        if (summonTimer >= summonTimeLimit)
        {
            summonTimer = 0f;
            CanSummonsMinions = true;
        }
    }
    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();
        
        StateMachine.AddState(States.EnemyIdle, new GolemEnemyIdleState(this));
        StateMachine.AddState(States.EnemyChase, new GolemEnemyChaseState(this, footstepSound));
        StateMachine.AddState(States.EnemyAttack, new GolemEnemyAttackState(this, stompSound));
        StateMachine.AddState(States.EnemyKick, new GolemEnemyArenaSlowState(this));
        StateMachine.AddState(States.EnemySpawnMinions, new GolemEnemySummonMinionsState(this));
        StateMachine.AddState(States.EnemyStagger, new GolemEnemyStunState(this, stunSound));
        StateMachine.AddState(States.EnemyHurt, new GolemEnemyHurtState(this, hurtSound));
        StateMachine.AddState(States.EnemyDeath, new GolemEnemyDeathState(this, deathSound));
        StateMachine.AddState(States.EnemyRangeAttack, new GolemEnemyRangeAttackState(this, throwSound));
        StateMachine.AddState(States.Paused, new EnemyPauseState<MudGolemEnemy>(this));

        chaseRange = viewRange * 2 / 3;
    }
    public override void Stagger()
    {
        if (Unstaggerable) return;
        
        StateMachine.ChangeState(States.EnemyStagger);
    }
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        base.Death(obj);

        Instantiate(deathParticlePrefab, deathParticleInitialPosition.position, deathParticlePrefab.transform.rotation, deathParticleInitialPosition);
        Instantiate(deathParticlePrefab, new Vector3(deathParticleInitialPosition.position.x,deathParticleInitialPosition.position.y + 2f ,deathParticleInitialPosition.position.z), deathParticlePrefab.transform.rotation, deathParticleInitialPosition);
        
        StateMachine.ChangeState(States.EnemyDeath);

        Rigidbody.detectCollisions = false;
        
        foreach (var iMinion in SummonedMinions.Where(iMinion => iMinion != null))
        {
            iMinion.Health.OnDead -= RemoveSummon;
            iMinion.Health.InstantKill();
        }
    }
    public bool PlayerOnSight() => Physics.CheckSphere(transform.position, viewRange, playerLayer);
    public bool PlayerOnRange(float range) => Physics.CheckSphere(transform.position, range, playerLayer);
    public void PlayAnimation(string key) => Animator.SetTrigger(key);
    public void ThrowBullet()
    {
        RangeAttackToken--;
        
        Instantiate(bulletPrefab, bulletInitialPosition.position, Quaternion.identity).GetComponent<BulletController>()
                                                                                      .SetTarget(PlayerTarget.gameObject.transform)
                                                                                      .SetDamage(ThrowDamage);
    }
    public void StartSummoningMinions()
    {
        if (SummonedMinions.Count >= MaxSummons) return;
        
        //Health.SetInvulnerability(true);
        
        BossFightArena.AvailableSpawnPoints.Shuffle();
        availableSpawnPoints = BossFightArena.AvailableSpawnPoints.Take(2).ToList();

        foreach (var t in availableSpawnPoints)
        {
            Instantiate(SummonParticles, t.position, SummonParticles.transform.rotation);
        }
        
        Instantiate(castSummonParticles, deathParticleInitialPosition.position, castSummonParticles.transform.rotation);
    }
    public void EndSummoningMinions()
    {
        if (SummonedMinions.Count == MaxSummons)
        {
            foreach (var summoned in SummonedMinions)
            {
                summoned.Health.FullHeal();
            }
        }
        else
        {
            foreach (var point in availableSpawnPoints)
            {
                if (SummonedMinions.Count >= MaxSummons) break;
                
                var summon = Instantiate(SummonedMinionPrefab, point.position, Quaternion.identity).GetComponent<MudEnemyMelee>();

                //used in pause mode
                if (EnemyManager.Instance != null)
                {
                    EnemyManager.Instance.AddMeleeEnemy(summon);
                    summon.Health.OnDead += x =>
                    {
                        if (EnemyManager.Instance != null)
                            EnemyManager.Instance.RemoveMeleeEnemy(summon);
                    };
                }

                summon.Initialize(PlayerTarget);
                summon.SetAwarenessViewRadius(40f);
                summon.Health.OnDead += RemoveSummon;
                SummonedMinions.Add(summon);
            }
        }
        
        availableSpawnPoints.Clear();

        CanSummonsMinions = false;

        StateMachine.ChangeState(States.EnemyChase);
            
        //Health.SetInvulnerability(false);

        summonTimer = 0f;
    }
    private void RemoveSummon(HealthComponent.HealthModificationReport healthModificationReport)
    {
        var enemy = healthModificationReport.Target.gameObject.GetComponent<MudEnemyMelee>();

        SummonedMinions.Remove(enemy);
        
        enemy.Health.OnDead -= RemoveSummon;
    }
    public bool InThrowRange() => Physics.CheckSphere(transform.position, ThrowRange, playerLayer);
    public bool InStompRange() => Physics.CheckSphere(transform.position, StompRange, playerLayer);
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ThrowRange);
        Gizmos.DrawWireSphere(transform.position, StompRange);
    }

    public bool EnemyPreValidations()
    {
        if (PlayerTarget.isDead && StateMachine.GetCurrentState() != States.EnemyIdle)
        {
            StateMachine.ChangeState(States.EnemyIdle);
            return true;
        }
        
        if (Health.IsDead && StateMachine.GetCurrentState() != States.EnemyIdle)
        {
            StateMachine.ChangeState(States.EnemyDeath);
            return true;
        }
        
        return false;
    }

    public void Stomp()
    {
    }
}