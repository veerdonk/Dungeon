using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Corridor
{
    public string name;
    public Tilemap floor { get; set; }
    public Tilemap walls { get; set; }
    string direction { get; set; }
    public BoundsInt wallBounds { get; set; }
    public BoundsInt floorBounds { get; set; }
    public TileBase[] wallTiles { get; set; }
    public TileBase[] floorTiles { get; set; }
    public GameObject trigger { get; set; }
    public Corridor(GameObject corridor)
    {
        name = corridor.name;
        direction = name.Substring(corridor.name.Length - 1);
        //Retrieve tilemaps and compress bounds (very important)
        floor = corridor.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();
        floor.CompressBounds();
        walls = corridor.transform.GetChild(0).GetChild(1).GetComponent<Tilemap>();
        walls.CompressBounds();
        //Get the bounds of both tilemaps
        wallBounds = walls.cellBounds;
        floorBounds = floor.cellBounds;
        //Retrieve the actual tiles
        wallTiles = walls.GetTilesBlock(wallBounds);
        floorTiles = floor.GetTilesBlock(floorBounds);
        //Retrieve the corridor trigger
        trigger = corridor.transform.GetChild(1).gameObject;
    }

    public override string ToString()
    {
        return $"name = {name}\ndirections = {direction}";
    }

    public bool hasDirection(string dir)
    {
        return direction.Contains(dir);
    }
}
