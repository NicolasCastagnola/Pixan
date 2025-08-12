using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class MiguelFinalHitState : IState
{
    MiguelFinalBoss _enemy;
    private readonly AudioConfigurationData _audioConfigurationData;
    MiguelStunTrigger _trigger;

    float _attackTime;
    string _attackType;

    public MiguelFinalHitState(MiguelFinalBoss enemy, AudioConfigurationData audioConfigurationData,MiguelStunTrigger trigger, float attackTime,string attackType)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;
        _attackTime = attackTime;
        _attackType = attackType;
        _trigger = trigger;
    }

    public void OnEnter()
    {
        if (_enemy.EnemyPreValidations()) return;

        _trigger.coll.enabled = true;
        _enemy.Animator.ResetTrigger("Idle");

        GameManager.instance.StartCoroutine(AttackTime());
    }
    IEnumerator AttackTime()
    {
        _enemy.PlayAnimation(_attackType + "Attack");

        _audioConfigurationData.Play2D();
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(_attackTime);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        _enemy.PlayAnimation("Stop"+ _attackType + "Attack");

        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        _enemy.StateMachine.ChangeState(States.EnemyBuff);
    }

    public void OnExit()
    {
        GameManager.instance.StopAllCoroutines();
        _trigger.coll.enabled = false;
        _enemy.PlayAnimation("Stop" + _attackType + "Attack");
    }

    public void OnUpdate()
    {

    }
    public void OnFixedUpdate()
    {

    }
}
