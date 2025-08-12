using System.Collections;
using Audio;
using UnityEngine;
public class MictlantecuhtliIntroState : IState
{
    private static readonly int Intro = Animator.StringToHash("Intro");
    private readonly Mictlantecuhtli _owner;
    private readonly AudioConfigurationData _audioConfigurationData;
    private readonly float _waitForIntro;
    public MictlantecuhtliIntroState(Mictlantecuhtli owner, AudioConfigurationData audioConfigurationData, float waitForIntro)
    {
        _owner = owner;
        _audioConfigurationData = audioConfigurationData;
        _waitForIntro = waitForIntro;
    }
    private IEnumerator WaitForIntro()
    {
        _owner.Animator.SetTrigger(Intro);

        yield return new WaitForSeconds(_waitForIntro);
        _owner.canGetHurt = true;
        _owner.StateMachine.ChangeState(States.EnemyIdle);
    }
    public void OnEnter()
    {
        _audioConfigurationData.Play2D();
        
        _owner.StartCoroutine(WaitForIntro());
    }
    public void OnUpdate()
    {
    }
    public void OnExit()
    {
    }
    public void OnFixedUpdate()
    {
    }
}