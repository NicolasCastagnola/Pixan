
public class MiguelFinalDeathState : IState
{
    private readonly MiguelFinalBoss _enemy;
    private readonly Audio.AudioConfigurationData _audioConfigurationData;

    bool isTrigger;

    public MiguelFinalDeathState(MiguelFinalBoss enemy, Audio.AudioConfigurationData audioConfigurationData)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;

    }

    public void OnEnter()
    {
        (_enemy.AnimatorController as MiguelFinalAnimatorController).OnDead();

        if (!isTrigger)
        {
            isTrigger = true;
            _audioConfigurationData.Play2D();

        }
    }

    public void OnExit()
    {
    }

    public void OnFixedUpdate()
    {
    }

    public void OnUpdate()
    {
    }
}
