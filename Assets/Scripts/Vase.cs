using Audio;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
public class Vase : MonoBehaviour
{
    [SerializeField, Tooltip("Activate this boolean to spawn items on destroy")] private bool containsItems;
    [SerializeField, ShowIf(nameof(containsItems))] private GameObject[] _objectsToSpawnOnDestroyed;
    [SerializeField] private AudioConfigurationData vaseDestroyedSoundEffect;

    
    private void Start()
    {
        if (containsItems)
        {
            GetComponent<Transform>().localScale = Vector3.one * 1.5f;
        }
    }

    private void DestroyVase()
    {
        //Instantiate(ParticlesManager.Instance.GetHitParticle()).transform.position = transform.position;
        
        vaseDestroyedSoundEffect.Play2D();

        if (containsItems)
        {
            foreach (var obj in _objectsToSpawnOnDestroyed)
            {
                var currentObject = Instantiate(obj, transform.position, Quaternion.identity);
            }
        }
              
        //TODO: Hacer una animacion de destroy!
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            DestroyVase();
        }
    }
}
