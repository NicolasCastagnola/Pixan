using UnityEngine;

public class MiguelFinalStunState : IState
{
    MiguelFinalBoss _enemy;
    private readonly Audio.AudioConfigurationData _audioConfigurationData;

    float _stunTime;

    public MiguelFinalStunState(MiguelFinalBoss enemy, Audio.AudioConfigurationData audioConfigurationData,float stunTime)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;
        _stunTime = stunTime;
    }

    public void OnEnter()
    {
        if (_enemy.EnemyPreValidations()) return;

        _enemy.Animator.ResetTrigger("Idle");
        _enemy.PlayAnimation("Stun");

        _audioConfigurationData.Play2D();

        GameManager.instance.WaitForXSeconds(_stunTime, () => {
            _enemy.PlayAnimation("StopStun");
            GameManager.instance.WaitForXSeconds(1f, () =>
                _enemy.StateMachine.ChangeState(States.EnemyBuff));
        });
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
    public void OnFixedUpdate()
    {

    }
}
