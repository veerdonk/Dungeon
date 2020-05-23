using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{

    public int xOffset = 0;
    public int yOffset = 0;

    private string entranceDir;

    private RoomSpawner rs;

    private void Start()
    {
        rs = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            //Set a flag with the current direction
            rs.setDirectionFlag(gameObject.name);
        }


    }

}
