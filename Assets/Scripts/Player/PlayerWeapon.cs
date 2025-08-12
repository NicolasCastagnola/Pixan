using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Player owner;
    private void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Player>();
        _damagedEntities = new HashSet<Entity>();
    }

    public bool disableTimescale;
    public bool damageEntitiesOnce = false;
    private HashSet<Entity> _damagedEntities;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Mictlantecuhtli>() != null)
        {
            if (!other.GetComponent<Mictlantecuhtli>().canGetHurt) return;
        }

        if (!other.TryGetComponent(out Entity entity))
        {
            if (other.TryGetComponent(out HealthComponent health))
                health.Damage(owner.CurrentComboDamage, gameObject);
        }
        else
        {
            if (other.gameObject == owner.gameObject) return;

            if (damageEntitiesOnce)
            {
                if (_damagedEntities.Contains(entity)) return;

                _damagedEntities.Add(entity);
            }

            //Debug.Log("DMG TO: " + entity.name + " - from: " + this.GetInstanceID());
            //if (entity as MudEnemyMelee != null || entity as MudEnemyDistance != null)
            //    entity.PushBack(transform.forward * 0.5f);
            entity.Health.Damage(owner.CurrentComboDamage, gameObject);

            owner.OnDamageEntity(entity);

            if (!disableTimescale)
            {
                if (Time.timeScale < 1f) return;
                Time.timeScale = 0.5f;
                StartCoroutine(ProgresiveAddTimescale());
            }
        }
    }

    IEnumerator ProgresiveAddTimescale()
    {
        while (Time.timeScale <= 1f)
        {
            Time.timeScale += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
