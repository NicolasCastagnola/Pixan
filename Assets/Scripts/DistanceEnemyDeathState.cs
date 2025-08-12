public class DistanceEnemyDeathState : IState
{
    private readonly MudEnemyDistance _mudEnemyDistance;
    public DistanceEnemyDeathState(MudEnemyDistance mudEnemyDistance)
    {
        _mudEnemyDistance = mudEnemyDistance;
    }
    public void OnEnter()
    {
        _mudEnemyDistance.PlayAnimation("Death");
        _mudEnemyDistance.Controller.enabled = false;
        _mudEnemyDistance.Motor.Capsule.enabled = false;
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