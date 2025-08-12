using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using DG.Tweening;

public class MiguelFinalIdleState : IState
{
    private readonly MiguelFinalBoss _enemy;

    AudioConfigurationData _idleSound;
    float _idleTimeMin, _idleTimeMax;


    public MiguelFinalIdleState(MiguelFinalBoss owner,AudioConfigurationData idleSound, float idleTimeMin, float idleTimeMax)
    {
        _enemy = owner;
        _idleTimeMin = idleTimeMin;
        _idleSound = idleSound;
        _idleTimeMax = idleTimeMax;
    }
    public void OnEnter(){

        _enemy.Animator.ResetTrigger("Stun");
        _enemy.Animator.ResetTrigger("StopStun");
        _enemy.Animator.ResetTrigger("RightAttack");
        _enemy.Animator.ResetTrigger("StopRightAttack");
        _enemy.Animator.ResetTrigger("LeftAttack");
        _enemy.Animator.ResetTrigger("StopLeftAttack");

        _enemy.PlayAnimation("Idle");

        if(_enemy.gameObject.activeInHierarchy)
            _enemy.StartCoroutine(AimToPlayer());
    }
    IEnumerator AimToPlayer()
    {
        yield return new WaitForSeconds(Random.Range(_idleTimeMin, _idleTimeMax + 1));
        var endRot = Quaternion.LookRotation(Player.Instance.Controller.transform.position - _enemy.transform.position);
        while (Mathf.Abs(endRot.eulerAngles.y-_enemy.transform.eulerAngles.y)>15)
        {
            endRot = Quaternion.LookRotation(Player.Instance.Controller.transform.position - _enemy.transform.position);
            var lerp = Quaternion.Lerp(_enemy.transform.rotation, endRot, 0.06f);
            _enemy.transform.eulerAngles = lerp.eulerAngles.y*Vector3.up;
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        if (_enemy.EnemyPreValidations()) yield break;

        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        _enemy.StateMachine.ChangeState(Random.Range(0, 11) >= 5 ? States.EnemyAttack:States.EnemyKick);
    
    }
    public void OnExit()
    {
        _enemy.StopAllCoroutines();
    }
    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
    }
}
