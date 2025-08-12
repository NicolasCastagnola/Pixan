using System;
using Sirenix.OdinInspector;
using UnityEngine;
public class EnemyColliderDamage : MonoBehaviour
{
    [ReadOnly, ShowInInspector] private int Damage;
    
    [SerializeField] private Collider _collider;
    [SerializeField] private Entity Owner;

    public void SetDamage(int damage) => Damage = damage;
    public void Activate() => _collider.enabled = true;
    public void Deactivate() => _collider.enabled = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield"))
        {
            var player = other.GetComponentInParent<Player>();
            
            if (player.ParryComponent.CanParry)
            {
                player.Parry();
                Owner.Stagger();
                Deactivate();
                ParticlesManager.Instance.SpawnParticles(transform.position, "Parry");
            }
            else
            {
                player.BlockAttack(Damage);
                Deactivate();
                ParticlesManager.Instance.SpawnParticles(this.transform.position, "Block"); 
            }
            
            return;
        }

        if (!other.CompareTag("Player")) return;
            
        other.GetComponent<Player>().Health.Damage(Damage, gameObject, DamageTypes.Enemy);
        Deactivate();
        ParticlesManager.Instance.SpawnParticles(transform.position, "");
    }
}