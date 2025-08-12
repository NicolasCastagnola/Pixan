
using KinematicCharacterController;
using UnityEngine;

public class MeleeEnemyDeathState : IState
{
    private readonly MudEnemyMelee _mudEnemyMelee;
    
    public MeleeEnemyDeathState(MudEnemyMelee mudEnemyMelee)
    {
        _mudEnemyMelee = mudEnemyMelee;
    }
    public void OnEnter()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };

        _mudEnemyMelee.Controller.SetInputs(ref inputs);

        _mudEnemyMelee.PlayAnimation("Death");

    }
    public void OnUpdate(){}
    public void OnExit(){}
    public void OnFixedUpdate(){}
}
