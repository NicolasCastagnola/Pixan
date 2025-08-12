using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Sirenix.OdinInspector;

public class MiguelFinalBoss : Boss
{
    [SerializeField, TabGroup("References")] private MiguelStunTrigger leftHand, rightHand;
    [SerializeField, TabGroup("Sound")] private AudioConfigurationData deathSound, stunSound,attackSound,idleSound,hitsound;
    [SerializeField, TabGroup("Combat")] float stunTime,leftAttackTime, rightAttackTime, idleRangeMinTime, idleRangeMaxTime;

    [HideInInspector] public bool started;

    public override void Stagger()
    {
        if (Unstaggerable) return;

        StateMachine.ChangeState(States.EnemyStagger);
    }

    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();

        StateMachine.AddState(States.EnemyIdle/*only used in init*/, new MiguelFinalInitState(this));
        StateMachine.AddState(States.EnemyBuff, new MiguelFinalIdleState(this, idleSound, idleRangeMinTime, idleRangeMaxTime));
        StateMachine.AddState(States.EnemyAttack, new MiguelFinalHitState(this, attackSound, leftHand, leftAttackTime, "Left"));//left Attack
        StateMachine.AddState(States.EnemyKick, new MiguelFinalHitState(this, attackSound, rightHand, rightAttackTime, "Right"));//right Attack
        StateMachine.AddState(States.EnemyStagger, new MiguelFinalStunState(this, stunSound, stunTime));//stun
        StateMachine.AddState(States.EnemyDeath, new MiguelFinalDeathState(this, deathSound));
        StateMachine.AddState(States.Paused, new EnemyPauseState<MiguelFinalBoss>(this));

        Health.OnDamage += hitsound.Play2D;
        Health.OnDead += x => StateMachine.ChangeState(States.EnemyDeath);

        StateMachine.ChangeState(States.EnemyBuff);
    }
    public void PlayAnimation(string key) => Animator.SetTrigger(key);
    public void StunBoss()
    {
        StateMachine.ChangeState(States.EnemyStagger);
    }
    
    public bool EnemyPreValidations()
    {
        if (PlayerTarget.isDead && StateMachine.GetCurrentState() != States.EnemyBuff)
        {
            StateMachine.ChangeState(States.EnemyIdle);
            return true;
        }

        if (Health.IsDead && StateMachine.GetCurrentState() != States.EnemyDeath)
        {
            StateMachine.ChangeState(States.EnemyDeath);
            return true;
        }

        return false;
    }
}
