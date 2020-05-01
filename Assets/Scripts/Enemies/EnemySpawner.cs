using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;

    //TODO: Increase/Decrease by some manner
    int numberOfEnemiesToSpawn = 3;

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
        
        Debug.Log($"Spawning {numberOfEnemiesToSpawn} enemies");

        for (int i = 0; i < numberOfEnemiesToSpawn + Random.Range(-1, 2); i++)
        {
            //TODO: make sure enemies are parented correctly
            //TODO: Between room permanence -> dead enemies should stay dead
            Vector3 pos = possiblePositions[Random.Range(0, possiblePositions.Count)];
            GameObject newEnemy = Instantiate(enemyPrefab, parent);
            newEnemy.transform.position = pos;
            //Set the enemies grid position
            EnemyManager em = newEnemy.GetComponentInChildren<EnemyManager>();

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
