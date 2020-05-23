using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public GameObject room;
    public Vector3 gridPosition;
    public bool hasNeighbours;
    public List<string> exits = new List<string>();
    public List<Transform> entrancePositions = new List<Transform>();
    public List<Vector3> possibleSpawnPoints;
    public List<Transform> walls;

    public Room(GameObject room, Vector3 gridPosition)
    {
        this.room = room;
        this.gridPosition = gridPosition;
    }
}
