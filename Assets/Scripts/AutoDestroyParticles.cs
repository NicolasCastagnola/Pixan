using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticles : MonoBehaviour
{
    float lifeTimeParticle;

    void Awake()
    {
        lifeTimeParticle = gameObject.GetComponent<ParticleSystem>().main.duration;
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        
        while (lifeTimeParticle > 0)
        {
            lifeTimeParticle -= Time.deltaTime;


            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }
}
