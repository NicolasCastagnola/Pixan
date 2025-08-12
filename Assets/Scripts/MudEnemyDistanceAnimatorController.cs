using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyDistanceAnimatorController : EntityAnimatorController
{
    [SerializeField] private MudEnemyDistance _mudEnemyDistance;
    [SerializeField] EnemyColliderDamage _weapon;

    //private void Awake() => _weapon = GetComponentInChildren<EnemyWeapon>();
    public void WeaponActive() => _weapon.Activate();
    public void WeaponDesactive() => _weapon.Deactivate();
    public void PrepareProjectile() => _mudEnemyDistance.PrepareProjectile();
    public void ShootProjectile() => _mudEnemyDistance.ShootProjectile();
}
