using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyBaseState : StateMachineBehaviour
{
    [SerializeField] protected MudEnemyMelee _mudEnemyMelee;

    public virtual void Initialize(MudEnemyMelee mudEnemyMelee)
    {
        _mudEnemyMelee = mudEnemyMelee;
    }
}