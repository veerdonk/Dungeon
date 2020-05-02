using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;
    public GameObject rangedEnemyPrefab;

    Dictionary<Vector3, List<EnemyManager>> enemies = new Dictionary<Vector3, List<EnemyManager>>();

    public delegate void RoomClearDelegate();
    public event RoomClearDelegate OnAllEnemiesCleared;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("EnemySpawner has multiple copies");
        }
        instance = this;
    }

    public void SpawnEnemies(List<Vector3> possiblePositions, Transform parent, Vector3 gridPosition)
    {
        //TODO: Increase/Decrease by some manner
        int numberOfEnemiesToSpawn = 3 + PlayerManager.instance.playerLevel;

        //TODO select enemytype/weapontype
        Debug.Log($"Spawning {numberOfEnemiesToSpawn} enemies");

        for (int i = 0; i < numberOfEnemiesToSpawn + Random.Range(-1, 2); i++)
        {
            //Choose what rarity weapon the enemy will have
            //0-70 = white, 70-90 = green 90-100 = epic
            int chance = Random.Range(0, 100);
            Weapon enemyWeapon;
            if(chance < 70)
            {
                enemyWeapon = (Weapon)Inventory.instance.whiteItems[Random.Range(0, Inventory.instance.whiteItems.Count)];
            }
            else{
                enemyWeapon = (Weapon)Inventory.instance.greenItems[Random.Range(0, Inventory.instance.greenItems.Count)];
            }
            //TODO add epic items here
            GameObject newEnemy = null;
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

            Vector3 pos = possiblePositions[Random.Range(0, possiblePositions.Count)];
             
            newEnemy.transform.position = pos;
            //Set the enemies grid position
            EnemyManager em = newEnemy.GetComponentInChildren<EnemyManager>();
            newEnemy.GetComponentInChildren<AbstractAttack>().weapon = enemyWeapon;
            if (enemies.ContainsKey(gridPosition)){
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



}
