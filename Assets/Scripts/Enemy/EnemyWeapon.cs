using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private MudEnemyMelee _mudEnemyMelee;

    private void Awake()
    {
        try
        {
            _mudEnemyMelee = GetComponentInParent<MudEnemyMelee>();
        }
        catch
        {
        }
    }

    public void EnableDisableWeapon(bool value)
    {
        GetComponent<BoxCollider>().enabled = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            
            CheckPlayerState(false, other.GetComponent<Player>());
        }

        if (other.CompareTag("Parry"))
        {
            GetComponent<BoxCollider>().enabled = false;
            ParticlesManager.Instance.SpawnParticles(this.transform.position, "Parry");
            if(_mudEnemyMelee!=null)
                _mudEnemyMelee.StateMachine.ChangeState(States.EnemyStagger);

            CheckPlayerState(false, other.GetComponent<Player>());
            //TODO: PLAY AUDIO Parry
        }
    }

    private void CheckPlayerState(bool blocking, Player player)
    {
        if(player == null) return;
         
        if (!blocking)
        {
            if (_mudEnemyMelee != null)
                player.Health.Damage(_mudEnemyMelee.EntityStats.rawDamage, gameObject, DamageTypes.Enemy);
            else
                player.Health.Damage(30, gameObject, DamageTypes.Enemy);


            ParticlesManager.Instance.SpawnParticles(this.transform.position, "");

            //TODO: PLAY AUDIO Hit Take Damage

        }
        else
        {
            if (_mudEnemyMelee != null)
                player.BlockAttack(_mudEnemyMelee.EntityStats.rawDamage);
            else
                player.BlockAttack(20);

            ParticlesManager.Instance.SpawnParticles(this.transform.position, "Block");

            //TODO: PLAY AUDIO Block
        }
    }
}
