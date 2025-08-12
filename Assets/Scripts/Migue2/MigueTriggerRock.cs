using UnityEngine;
using DG.Tweening;
using System.Collections;
using Audio;

public class MigueTriggerRock : MonoBehaviour
{
    [SerializeField] AudioConfigurationData hitSound;
    [SerializeField] MiguelRockStunTrigger rockPrefab;
    [SerializeField] MiguelFinalBoss migueBoss;
    [SerializeField] Transform pivot;
    [SerializeField] Vector3 endValue;

    HealthComponent health;
    bool canSpawn=true;
    Vector3 initialPos;


    void Start() {
        health = GetComponent<HealthComponent>();
        health.Initialize();
        health.ReviveAndFullHeal(gameObject);
        initialPos = transform.position;

        health.OnDead += x => StartCoroutine(SpawnRock());
        health.OnRevive += x => StopAllCoroutines();
    }
    IEnumerator SpawnRock()
    {
        hitSound.Play2D();
        while ((transform.position-endValue).magnitude >= 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, endValue,0.1f);
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        transform.position = endValue;
        if (canSpawn)
        {
            var currentState = migueBoss.StateMachine.GetCurrentState();
            var animatorControler = migueBoss.AnimatorController as MiguelFinalAnimatorController;

            if (currentState == States.EnemyAttack)
                Instantiate(rockPrefab,
                    new Vector3(animatorControler.leftAreaPivot.position.x, pivot.position.y, animatorControler.leftAreaPivot.position.z), pivot.rotation);
            else if (currentState == States.EnemyKick)
                Instantiate(rockPrefab,
                    new Vector3(animatorControler.rightAreaPivot.position.x, pivot.position.y, animatorControler.rightAreaPivot.position.z), pivot.rotation);
            else
                Instantiate(rockPrefab, pivot.position, pivot.rotation);

            canSpawn = false;
        }

        yield return new WaitForSeconds(5);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(6);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        yield return new WaitForSeconds(6);
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);

        while ((transform.position - initialPos).magnitude >= 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, initialPos, 0.1f);
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        transform.position = initialPos;
        canSpawn = true;
        health.ReviveAndFullHeal(gameObject);
       
    }
       
}
