using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class EnemyManager : BaseMonoSingleton<EnemyManager>
{
    private Player _playerReference;
    
    [SerializeField] private List<MudEnemyMelee> LevelMeleeEnemies = new List<MudEnemyMelee>();
    [SerializeField] private List<MudEnemyDistance> LevelDistanceEnemies = new List<MudEnemyDistance>();
    [SerializeField] private List<Boss> LevelBosses = new List<Boss>();

    [SerializeField] MudEnemyMelee meleePrefab;
    [SerializeField] MudEnemyDistance distancePrefab;

    [SerializeField] public int enemyDeathCounter { get; set; }

    [SerializeField] int ammountToLevelUp;

    [SerializeField] private UnityEvent onGetReward;

    [Button, Tooltip("Use after parenting all enemies")] public void PopulateMeleeEnemies() => LevelMeleeEnemies = GetComponentsInChildren<MudEnemyMelee>().ToList();
    [Button, Tooltip("Use after parenting all enemies")] public void PopulateDistanceEnemies() => LevelDistanceEnemies = GetComponentsInChildren<MudEnemyDistance>().ToList();
    [Button, Tooltip("Use after parenting all Bosses")] public void PopulateBosses() => LevelBosses = GetComponentsInChildren<Boss>().ToList();
    
    protected override void Start()
    {
        onGetReward.AddListener(() => enemyDeathCounter = 0); 
    }

    [Button]
    public void Initialize(Player playerReference)
    {
        _playerReference = playerReference;
        
        // PopulateMeleeEnemies();
        // PopulateDistanceEnemies();
        // PopulateBosses();

        if (LevelBosses.Count > 0)
        {
            foreach (var boss in LevelBosses)
            {
                boss.Initialize(playerReference, this);
            }
        }
        if (LevelMeleeEnemies.Count > 0)
        {
            foreach (var enemy in LevelMeleeEnemies)
            {
                enemy.Initialize(playerReference, this);
            }
        }
        if (LevelDistanceEnemies.Count > 0)
        {
            foreach (var enemy in LevelDistanceEnemies)
            {
                enemy.Initialize(playerReference, this);
            }
        }
    }
    public void RemoveMudMeleeEnemy(MudEnemyMelee enemy)
    {
        if (LevelMeleeEnemies.Contains(enemy))
        {
            enemyDeathCounter++;

            if (enemyDeathCounter <= ammountToLevelUp) Canvas_Playing.Instance.OnUpdateExp(enemyDeathCounter, ammountToLevelUp, onGetReward);

            //LevelMeleeEnemies.Remove(enemy);
        }
    }
    public void RemoveMudDistanceEnemy(MudEnemyDistance enemy)
    {
        if (LevelDistanceEnemies.Contains(enemy))
        {
            enemyDeathCounter++;

            if (enemyDeathCounter <= ammountToLevelUp) Canvas_Playing.Instance.OnUpdateExp(enemyDeathCounter, ammountToLevelUp, onGetReward);

            //LevelDistanceEnemies.Remove(enemy);
        }
    }

    public List<Entity> GetEnemies()
    {
        return LevelMeleeEnemies.Cast<Entity>()
               .Concat(LevelDistanceEnemies.Cast<Entity>())
               .Concat(LevelBosses.Cast<Entity>())
               .ToList();
    }

    public void ResetEnemies()
    {
        //TODO: Resusitar y re poscionar los nemigos

        if (LevelMeleeEnemies.Count > 0)
        {
            for (int i = 0; i < LevelMeleeEnemies.Count; i++)
            {
                var enemy = LevelMeleeEnemies[i];

                if (enemy == null)
                    continue;

                if (enemy.gameObject.activeInHierarchy == false)
                {
                    var _enemy = Instantiate(meleePrefab, enemy.InitialPos, Quaternion.identity);

                    Destroy(enemy.gameObject);

                    LevelMeleeEnemies[i] = _enemy;
                    _enemy.Initialize(Player.Instance, this);
                }

                LevelMeleeEnemies[i].ReviveAndResetPosition();
            }
        }
        if (LevelDistanceEnemies.Count > 0)
        {
            for (int i = 0; i < LevelDistanceEnemies.Count; i++)
            {
                var enemy = LevelDistanceEnemies[i];

                if (enemy == null)
                    continue;

                if (enemy.gameObject.activeInHierarchy == false)
                {
                    var _enemy = Instantiate(distancePrefab, enemy.InitialPos, Quaternion.identity);

                    Destroy(enemy.gameObject);

                    LevelDistanceEnemies[i] = _enemy;
                    _enemy.Initialize(Player.Instance, this);
                }

                LevelDistanceEnemies[i].ReviveAndResetPosition();
            }
        }
        GameManager.ShowLog("ENEMY MANAGER: Resetting all enemies!!!");
    }

    //methods using for spawning enemies in bossfights
    public void AddMeleeEnemy(MudEnemyMelee enemy)
    {
        if (!LevelMeleeEnemies.Contains(enemy))
            LevelMeleeEnemies.Add(enemy);
    }
    public void RemoveMeleeEnemy(MudEnemyMelee enemy)
    { 
        if (LevelMeleeEnemies.Contains(enemy)) 
            LevelMeleeEnemies.Remove(enemy); 
    }
    public void AddDistanceEnemy(MudEnemyDistance enemy)
    {
        if (!LevelDistanceEnemies.Contains(enemy))
            LevelDistanceEnemies.Add(enemy);
    }
    public void RemoveDistanceEnemy(MudEnemyDistance enemy)
    {
        if (LevelDistanceEnemies.Contains(enemy))
            LevelDistanceEnemies.Remove(enemy);
    }
}
