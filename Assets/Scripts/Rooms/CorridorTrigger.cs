using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorTrigger : MonoBehaviour
{
    private RoomSpawner rs;
    private void Start()
    {
        rs = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomSpawner>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rs.UpdatePlayerLocationAndSpawnRoom(gameObject);
        }
    }
}
