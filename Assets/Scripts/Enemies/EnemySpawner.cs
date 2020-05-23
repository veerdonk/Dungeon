using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject unarmedEnemyPrefab;

    //TODO biomes?
    public Enemy[] enemyData;
    Dictionary<int, List<Enemy>> enemiesPerPlayerLevel = new Dictionary<int, List<Enemy>>();
    List<Enemy> enemiesToSelectFrom = new List<Enemy>();

    Dictionary<Vector3, List<EnemyManager>> enemies = new Dictionary<Vector3, List<EnemyManager>>();

    public delegate void RoomClearDelegate();
    public event RoomClearDelegate OnAllEnemiesCleared;

    int credits = 30;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("EnemySpawner has multiple copies");
        }
        instance = this;
    }

    private void Start()
    {
        PlayerManager.instance.OnLevelIncrease += UpdateDifficultyAfterLevelIncrease;

        enemyData = Resources.LoadAll<Enemy>(Constants.ENEMY_PATH);

        foreach (Enemy enemySO in enemyData)
        {
            if (enemiesPerPlayerLevel.ContainsKey(enemySO.minPlayerLevel))
            {
                enemiesPerPlayerLevel[enemySO.minPlayerLevel].Add(enemySO);
            }
            else
            {
                enemiesPerPlayerLevel[enemySO.minPlayerLevel] = new List<Enemy>{enemySO};
            }
        }
        enemiesToSelectFrom.AddRange(enemiesPerPlayerLevel[1]);
    }

    public void SpawnEnemies(List<Vector3> possiblePositions, Transform parent, Vector3 gridPosition)
    {
        int currentCredits = credits;
        //Certain number of credits to work with
        while(currentCredits > 0)
        {
            //Randomly select enemy object
            Enemy enemyToSpawn = enemiesToSelectFrom[Random.Range(0, enemiesToSelectFrom.Count)];
            //Substract cost for this enemy
            currentCredits -= enemyToSpawn.creditCost;
            GameObject newEnemy = null;
            if (enemyToSpawn.type != EnemyType.UNARMED)
            {

                //Choose what rarity weapon the enemy will have
                //0-70 = white, 70-90 = green 90-100 = epic
                int chance = Random.Range(0, 100);
                Weapon enemyWeapon;

                if (chance < 70)
                {
                    enemyWeapon = Inventory.instance.getRandomWeaponOfTypeAndRarity(Util.getRandomWeaponTypeForEnemy(enemyToSpawn.type), Rarity.COMMON);
                }
                else
                {
                    enemyWeapon = Inventory.instance.getRandomWeaponOfTypeAndRarity(Util.getRandomWeaponTypeForEnemy(enemyToSpawn.type), Rarity.RARE);
                }

                if (enemyWeapon == null)
                {
                    enemyWeapon = Inventory.instance.getRandomWeaponOfTypeAndRarity(Util.getRandomWeaponTypeForEnemy(enemyToSpawn.type), Rarity.COMMON);
                }

                //TODO add epic items here
               
                switch (enemyWeapon.type)
                {
                    case WeaponType.BOW:
                        newEnemy = Instantiate(rangedEnemyPrefab, parent);
                        break;
                    case WeaponType.SWORD:
                        newEnemy = Instantiate(enemyPrefab, parent);
                        break;
                    default:
                        break;
                }
                newEnemy.GetComponentInChildren<AbstractAttack>().weapon = enemyWeapon;
            }
            else
            {
                newEnemy = Instantiate(unarmedEnemyPrefab, parent);
            }
            EnemyManager em = newEnemy.GetComponent<EnemyManager>();
            em.enemy = enemyToSpawn;
            //TODO possibly mutate enemy stats

            Vector3 pos = possiblePositions[Random.Range(0, possiblePositions.Count)];

            newEnemy.transform.position = pos;
            //Set the enemies grid position
            
            if (enemies.ContainsKey(gridPosition))
            {
                enemies[gridPosition].Add(em);
            }
            else
            {
                enemies[gridPosition] = new List<EnemyManager> { em };
            }

            em.enemyGridPosition = gridPosition;
            //Register event handler
            em.onEnemyDeath += RemoveEnemyFromRoomList;
            PlayerManager.instance.subscribeToEnemy(newEnemy.GetComponentInChildren<EnemyManager>());


        }
        
        //TODO: Increase/Decrease by some manner
        int numberOfEnemiesToSpawn = 3 + PlayerManager.instance.playerLevel;



        //TODO select enemytype/weapontype
        //Debug.Log($"Spawning {numberOfEnemiesToSpawn} enemies");

        //for (int i = 0; i < numberOfEnemiesToSpawn + Random.Range(-1, 2); i++)
        //{



        //}

    }

    void RemoveEnemyFromRoomList(int exp, int gold, EnemyManager em)
    {

        enemies[em.enemyGridPosition].Remove(em);
        if(enemies[em.enemyGridPosition].Count <= 0)
        {
            //If all enemies in a room are dead trigger event
            OnAllEnemiesCleared.Invoke();
        }

    }

    void UpdateDifficultyAfterLevelIncrease(int newPlayerLevel)
    {
        //TODO better scaling
        credits += newPlayerLevel * 10;
        if (enemiesPerPlayerLevel.ContainsKey(newPlayerLevel))
        {
            enemiesToSelectFrom.AddRange(enemiesPerPlayerLevel[newPlayerLevel]);
        }
    }

}
