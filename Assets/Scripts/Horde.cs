using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horde : MonoBehaviour
{
    [SerializeField] KeyBehaviour finalKey;

    [SerializeField] GameObject[] fogWalls;

    [SerializeField] ParticleSystem SummonParticles;

    [SerializeField] MudEnemyMelee enemyPrefab;
    
    [SerializeField] List<Transform> spawnpoints = new List<Transform>();
    List<MudEnemyMelee> SummonedMinions = new List<MudEnemyMelee>();

    bool initialize;

    public void Start()
    {
        if (PlayerPrefs.GetInt(finalKey + ".Horde", 0) == 1)
        {
            finalKey.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null&&!initialize)
        {
            initialize = true;
            StartCoroutine(HordeRutine());
        }
    }
    IEnumerator HordeRutine()
    {
        foreach (var fogwall in fogWalls)
            fogwall.SetActive(true);

        spawnpoints.Shuffle();
        while (spawnpoints.Count>0)
        {
            var spawnpoint = spawnpoints[0];
            spawnpoints.RemoveAt(0);

            Instantiate(SummonParticles, spawnpoint.position, SummonParticles.transform.rotation);
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
            yield return new WaitForSeconds(Random.Range(0f, 1));
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
            var summon = Instantiate(enemyPrefab, spawnpoint.position, Quaternion.identity).GetComponent<MudEnemyMelee>();

            //used in pause mode
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.AddMeleeEnemy(summon);
                summon.Health.OnDead += x =>
                {
                    if (EnemyManager.Instance != null)
                        EnemyManager.Instance.RemoveMeleeEnemy(summon);
                };
            }
            if(Player.Instance!=null)
                summon.Initialize(Player.Instance);

            summon.SetAwarenessViewRadius(40f);
            summon.Health.OnDead += RemoveSummon;
            SummonedMinions.Add(summon);

            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
            yield return new WaitForSeconds(Random.Range(1f, 10));
            yield return new WaitUntil(() => Canvas_Pause.IsPlaying);
        }
        yield return new WaitUntil(() => Canvas_Pause.IsPlaying);

        yield return new WaitUntil(() => SummonedMinions.Count <= 0);
        finalKey.gameObject.SetActive(true);
        PlayerPrefs.SetInt(finalKey + ".Horde", 1);
        foreach (var fogwall in fogWalls)
            fogwall.SetActive(false);
    }
    private void RemoveSummon(HealthComponent.HealthModificationReport healthModificationReport)
    {
        var enemy = healthModificationReport.Target.gameObject.GetComponent<MudEnemyMelee>();

        SummonedMinions.Remove(enemy);

        enemy.Health.OnDead -= RemoveSummon;
    }
}
