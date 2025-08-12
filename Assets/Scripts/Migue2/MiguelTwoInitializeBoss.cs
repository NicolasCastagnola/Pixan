using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Audio;

public class MiguelTwoInitializeBoss : MonoBehaviour
{
    [SerializeField] BossFightArena arena;
    [SerializeField] MiguelFinalBoss migue;
    [SerializeField] AudioConfigurationData enterBoss;

    [SerializeField] GameObject nextLevelcoll;   

    bool started;

    private void Start()
    {
        StartCoroutine(disableUntilFight());
    }
    IEnumerator disableUntilFight()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        migue.transform.position = new Vector3(migue.transform.position.x, -35, migue.transform.position.z);
        migue.gameObject.SetActive(false);

        migue.Health.OnDead += x=>migue.StartCoroutine(NextLevel());
    }
    IEnumerator NextLevel()
    {
        yield return new WaitForSecondsRealtime(5f);
        nextLevelcoll.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (started)
                return;
            started = true;

            enterBoss.Play2D();
            StartCoroutine(CheckInitialize());
        }
    }
    IEnumerator CheckInitialize()
    {
        migue.gameObject.SetActive(true);
        migue.transform.position = new Vector3(migue.transform.position.x, -35, migue.transform.position.z);
        while (migue.transform.position.y<=7)
        {
            migue.transform.position += Vector3.up * Time.deltaTime*10;
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        migue.transform.position = new Vector3(migue.transform.position.x, 7, migue.transform.position.z);
        migue.started = true;
        migue.StateMachine.ChangeState(States.EnemyBuff);
        (migue.AnimatorController as MiguelFinalAnimatorController).EnableBoss();
        arena.InitializeArena();

    }
}
