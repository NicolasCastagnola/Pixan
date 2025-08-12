using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MigueEnragedDamageSystem : MonoBehaviour
{
    [SerializeField] HealthComponent healthComponent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ChargeBulletAttack>() || other.GetComponent<PlayerWeapon>())
            healthComponent.Damage(30, other.gameObject);
    }
}
