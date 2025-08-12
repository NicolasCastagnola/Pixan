using System.Collections;
using Audio;
using KinematicCharacterController;
using Sirenix.Utilities;
using UnityEngine;
public class MictlantecuhtliPunchState : IState
{
    private const int SEED = 123;
    
    private readonly Mictlantecuhtli _owner;
    private readonly float _duration;

    private bool attacking = true;

    private AudioConfigurationData _punchSound;
    public MictlantecuhtliPunchState(Mictlantecuhtli owner, float duration, AudioConfigurationData punchSound)
    {
        _owner = owner;
        _duration = duration;
        _punchSound = punchSound;
    }
    public void OnEnter() => _owner.StartCoroutine(WaitForPunchAttack());
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs { MoveVector = Vector3.zero };

        if(!attacking)
        {
            inputs = new AICharacterInputs { MoveVector = Vector3.zero, LookVector = (_owner.PlayerTarget.transform.position - _owner.transform.position).normalized};
        }

        _owner.Controller.SetInputs(ref inputs);

    }
    public void OnExit()
    {
    }

    private IEnumerator WaitForPunchAttack()
    {
        attacking = false;
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        attacking = true;

        _owner.AnimatorController.TriggerPunch();

        _punchSound.Play2D();
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSecondsRealtime(_duration / 2);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(_duration/2);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);

        if (WithinRange())
        {
            if (WithinFOV())
            {
                Random.InitState(SEED);

                float randomValue = Random.value;
                
                if ((int)randomValue % 2 == 0)
                {
                    GameManager.ShowLog("RANDOM SEED IS PAIR!! " + (int)randomValue);
                    
                    _owner.StateMachine.ChangeState(States.EnemyIdle);
                    yield break;
                }
                
                _owner.StartCoroutine(WaitForPunchAttack());
                yield break;
            }
                
            _owner.StateMachine.ChangeState(States.EnemyIdle);
        }
        
        else _owner.StateMachine.ChangeState(States.EnemyIdle);
        
    }
    private bool WithinFOV() => Vector3.Angle(_owner.transform.forward, _owner.PlayerTarget.transform.InverseTransformDirection(_owner.transform.position - _owner.PlayerTarget.CenterOfMassPivot.position)) < _owner.FieldOfView;
    private bool WithinRange()
    {
        GameManager.ShowLog("Distance: " + Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position));

        return Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position) < 3 /*_owner.ValidRangeToAttack*/;
    }
    
    public void OnFixedUpdate()
    {
    }
}