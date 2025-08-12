using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class MictlantecuhtliStompAttackState : IState
{
    private readonly Mictlantecuhtli _owner;
    private readonly float _waitForStompDuration;
    private readonly float _stompRadius;
    public MictlantecuhtliStompAttackState(Mictlantecuhtli owner, float waitForStompDuration, float stompRadius = 5f)
    {
        _owner = owner;
        _stompRadius = stompRadius;
        _waitForStompDuration = waitForStompDuration;
    }
    public void OnEnter() => _owner.StartCoroutine(WaitForStompAttack());
    public void OnUpdate(){}
    public void OnExit() => _owner.StopAllCoroutines();
    public void OnFixedUpdate(){}
    private IEnumerator WaitForStompAttack()
    {
        _owner.AnimatorController.TriggerStomp();
        
        ParticlesManager.Instance.SpawnParticles(_owner.transform.position,"Stomp");

        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(_waitForStompDuration);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);

        if (WithinRange())
        {
            if (WithinFOV())
            {
                _owner.StateMachine.ChangeState(States.EnemyAttack);
                yield break;
            }
            
            _owner.StartCoroutine(WaitForStompAttack());
        }
        
        else _owner.StateMachine.ChangeState(States.EnemyChase);
        
    }
    private bool WithinFOV()
    {
        return Vector3.Angle(_owner.transform.forward,
            _owner.PlayerTarget.transform.InverseTransformDirection(_owner.transform.position - _owner.PlayerTarget.CenterOfMassPivot.position)) < _owner.FieldOfView;
    }
    

    private bool WithinRange()
    {
        GameManager.ShowLog("Distance: " + Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position));

        return Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position) < _owner.ValidRangeToAttack;
    }
}