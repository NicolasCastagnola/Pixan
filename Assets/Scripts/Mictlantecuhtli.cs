using System;
using System.Collections;
using Audio;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;


public class Mictlantecuhtli : Boss
{
    //audios orden: intro,muerte,stomp,footstep,hurt,spell,summon

    [SerializeField, TabGroup("Combat")] private float waitForIntroDuration = 10f;
    [SerializeField, TabGroup("Combat")] private float waitForPunchDuration = 2f;
    [SerializeField, TabGroup("Combat")] private float waitForStompDuration = 4f;

    [SerializeField, TabGroup("Combat")] private int stompDamage = 1;
    [SerializeField, TabGroup("Combat")] private float meleeRange = 5f;
    [SerializeField, TabGroup("Combat")] private float meleeRadius = 2f;
    //[SerializeField, TabGroup("Combat")] private EnemyColliderDamage meleeColliderDamage;

    [SerializeField, TabGroup("Combat")] private float stompRange = 3f;
    [SerializeField, TabGroup("Combat")] private float stompRadius = 2f;
    //[SerializeField, TabGroup("Combat")] private EnemyColliderDamage stompColliderDamage;

    [SerializeField, TabGroup("Combat")] public EnemyWeapon enemyWeapon;

    [SerializeField] private ParticleSystem deathParticles, healingParticles, buffParticles, stompParticle;

    [SerializeField, TabGroup("Sound")] private AudioConfigurationData deathSound, summonSound, punchSound, stompSound, hurtSound, footstepSound, stunSound, introSound;

    public bool canGetHurt { set; get; }

    public override void InitializeAI()
    {
        base.InitializeAI();

        StateMachine.ChangeState(States.EnemyIntro);
    }
    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();

        StateMachine.AddState(States.EnemyIntro, new MictlantecuhtliIntroState(this, introSound, waitForIntroDuration));
        StateMachine.AddState(States.EnemyIdle, new MictlantecuhtliIdleState(this));
        StateMachine.AddState(States.EnemyChase, new MictlantecuhtliChaseState(this, footstepSound));
        StateMachine.AddState(States.EnemyHeal, new MictlantecuhtliHealState(this));
        StateMachine.AddState(States.EnemyBuff, new MictlantecuhtliBuffState(this));
        StateMachine.AddState(States.EnemySpawnMinions, new MictlantecuhtliSpawnMinionsState(this, summonSound));
        StateMachine.AddState(States.EnemyAttack, new MictlantecuhtliPunchState(this, waitForPunchDuration, punchSound));
        StateMachine.AddState(States.StompAttack, new MictlantecuhtliStompAttackState(this, waitForStompDuration));
        StateMachine.AddState(States.EnemyDeath, new MictlantecuhtliDeathState(this));
        StateMachine.AddState(States.Paused, new EnemyPauseState<Mictlantecuhtli>(this));
    }
    public override void Stagger()
    {
        if (Unstaggerable) return;

        StateMachine.ChangeState(States.EnemyStagger);

    }
    // private void LateUpdate()
    // {
    //     GameManager.ShowLog(StateMachine?.CurrentStateName);
    // }
    protected override void Damage(HealthComponent.HealthModificationReport obj)
    {
        if(!canGetHurt) return;

        hurtSound.Play2D();

        base.Damage(obj);
    }
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        StateMachine.ChangeState(States.EnemyDeath);

        deathSound.Play2D();
        deathParticles.gameObject.SetActive(true);
        deathParticles.Play();

        base.Death(obj);
    }
    public bool PlayerIsValidToAttack() => PlayerTarget.Health.IsAlive && IsInitialized;
    private bool WithinFOV() => Vector3.Angle(CenterOfMassPivot.transform.forward, PlayerTarget.transform.InverseTransformDirection(transform.position - PlayerTarget.CenterOfMassPivot.position)) < FieldOfView;
    public bool InStompRange() => Vector3.Distance(transform.position, PlayerTarget.transform.position) <= stompRange;
    public bool InMeleeRange() => Vector3.Distance(transform.position, PlayerTarget.transform.position) <= meleeRange;
    public void Stomp()
    {
        // stompColliderDamage.Activate();

        if (stompParticle.gameObject.activeSelf)
        {
            stompParticle.Stop();
            stompParticle.gameObject.SetActive(false);
        }
        else
        {
            var bx = enemyWeapon.gameObject.GetComponent<BoxCollider>();
            bx.center = new Vector3(0.1f, 0, -1.7f);
            bx.size = new Vector3(6f, 5f, 6f);

            stompSound.Play2D();
            stompParticle.gameObject.SetActive(true);
            stompParticle.Play();
        }
    }

    public void Punch()
    {
        ParticlesManager.Instance.SpawnParticles(enemyWeapon.transform.position, "Punch");

        var bx = enemyWeapon.gameObject.GetComponent<BoxCollider>();
        bx.center = new Vector3(0.1f, 0, 0.45f);
        bx.size = new Vector3(1.5f, 4.6f, 5.5f);

        punchSound.Play2D();
    }
    public void StartHealing()
    {
        Instantiate(healingParticles, transform.position, Quaternion.identity, transform);

        healingParticles.gameObject.SetActive(true);
        healingParticles.Play();

        Health.Heal(10, gameObject);
    }
    public void StartBuffing()
    {
        buffParticles.gameObject.SetActive(true);
        buffParticles.Play();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.DrawWireSphere(transform.position, stompRange);

        float halfFOV = FieldOfView * 0.5f;
        var leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        var rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        var leftRayDirection = leftRayRotation * CenterOfMassPivot.forward;
        var rightRayDirection = rightRayRotation * CenterOfMassPivot.forward;

        Gizmos.DrawRay(CenterOfMassPivot.position, leftRayDirection * AwarenessViewRadius);
        Gizmos.DrawRay(CenterOfMassPivot.position, rightRayDirection * AwarenessViewRadius);
        Gizmos.DrawLine(CenterOfMassPivot.position, transform.position + leftRayDirection * AwarenessViewRadius);
        Gizmos.DrawLine(CenterOfMassPivot.position, transform.position + rightRayDirection * AwarenessViewRadius);
    }
}