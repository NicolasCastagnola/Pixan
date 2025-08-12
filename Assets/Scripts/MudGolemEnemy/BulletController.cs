using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Collider _collider;
    private Vector3 _targetPosition;
    
    public float speed = 25f;
    public int baseDamage = 10;

    [SerializeField] private ParticleSystem particlesPrefab;

    public BulletController SetDamage(int newDamage)
    {
        baseDamage = newDamage;
        
        return this;
    }

    public BulletController SetTarget(Transform playerTransform, bool followTarget=false, Vector3 offset = default)
    {
        if (followTarget) StartCoroutine(FollowTarget(playerTransform));

        _collider = GetComponent<Collider>();
        
        _targetPosition = playerTransform.position + offset;
        
        return this;
    }
    private IEnumerator FollowTarget(Transform target, Vector3 offset = default)
    {
        GetComponent<Rigidbody>().detectCollisions = false;

        yield return new WaitForSeconds(0.2f);
        
        GetComponent<Rigidbody>().detectCollisions = true;
        
        while (true)
        {
            _targetPosition = target.position + offset;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if (_targetPosition == null)
        {
            StopAllCoroutines();
            Destroy(gameObject);
            return;
        }

        var direction = _targetPosition - transform.position;

        if (direction.magnitude < 0.5f)
            ValidCollision();

        
        direction.Normalize();
        
        transform.Translate(direction * (speed * Time.deltaTime));

    }

    private void ValidCollision()
    {
        var part = Instantiate(particlesPrefab, transform.position,Quaternion.identity);
        Destroy(part, 2f);
        StopAllCoroutines();
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Physics.IgnoreCollision(_collider, other, true);
            
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();

            player.Health.Damage(baseDamage, gameObject, DamageTypes.Enemy);
        }
        else if (other.gameObject.CompareTag("Parry"))
        {
            ParticlesManager.Instance.SpawnParticles(transform.position, "Parry");
        }
        ValidCollision();
    }
}