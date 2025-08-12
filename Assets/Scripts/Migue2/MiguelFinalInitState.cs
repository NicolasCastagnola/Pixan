public class MiguelFinalInitState : IState
{
    private readonly MiguelFinalBoss _enemy;

    bool isReady => _enemy.transform.position.y>=7;

    public MiguelFinalInitState(MiguelFinalBoss enemy)
    {
        _enemy = enemy;

    }

    public void OnEnter()
    {
        _enemy.Motor.enabled = false;

        if(isReady)
            _enemy.StateMachine.ChangeState(States.EnemyBuff);
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