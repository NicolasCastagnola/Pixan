using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MiguelAreaAttack : MonoBehaviour
{
    [SerializeField] Audio.AudioConfigurationData hitSound;
    [SerializeField] float timeOfProcess = 1.6f;
    [SerializeField] int damage;

    private void Start()
    {
        hitSound.Play2D();
        StartCoroutine(Scale());
    }
    IEnumerator Scale()
    {
        Vector3 scale = 60*Vector3.one;
        while ((transform.localScale - scale).magnitude >= 0.5f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, 0.08f);
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        transform.localScale = scale;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player") {

            var health = other.GetComponent<HealthComponent>();

            if (health!=null)
                health.Damage(damage, gameObject);

        }
            
    }
}
