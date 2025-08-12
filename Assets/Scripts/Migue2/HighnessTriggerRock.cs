using UnityEngine;
using DG.Tweening;
using System.Collections;
using Audio;

public class HighnessTriggerRock : MonoBehaviour
{
    [SerializeField] AudioConfigurationData hitSound;
    [SerializeField] GameObject soul;
    [SerializeField] Vector3 endValue;

    HealthComponent health;


    void Start() {
        health = GetComponent<HealthComponent>();
        health.Initialize();
        health.ReviveAndFullHeal(gameObject);

        health.OnDead += x => StartCoroutine(AppearSoul());
        health.OnRevive += x => StopAllCoroutines();
    }
    IEnumerator AppearSoul()
    {
        hitSound.Play2D();
        while ((transform.position-endValue).magnitude >= 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, endValue,0.1f);
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        transform.position = endValue;
        soul.SetActive(true);
       
    }
       
}
