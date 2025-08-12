using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyAnimatorController : EntityAnimatorController
{
    [SerializeField] private EnemyColliderDamage meleeEnemyColliderDamage;
    [SerializeField] private MudEnemyMelee parent;

    public void WeaponActive() => meleeEnemyColliderDamage.Activate();
    public void WeaponDeactivate() => meleeEnemyColliderDamage.Deactivate();
}
