using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RoomTemplates : MonoBehaviour
{

    public GameObject[] dungeonRooms;
    public List<GameObject> dungeonLrooms = new List<GameObject>();
    public List<GameObject> dungeonRrooms = new List<GameObject>();
    public List<GameObject> dungeonTrooms = new List<GameObject>();
    public List<GameObject> dungeonBrooms = new List<GameObject>();    
    
    public GameObject[] grassRooms;
    public List<GameObject> grassLrooms = new List<GameObject>();
    public List<GameObject> grassRrooms = new List<GameObject>();
    public List<GameObject> grassTrooms = new List<GameObject>();
    public List<GameObject> grassBrooms = new List<GameObject>();

    public Dictionary<Biome, BiomeTemplate> roomsPerBiome = new Dictionary<Biome, BiomeTemplate>();

    void Awake()
    {
        dungeonRooms = Resources.LoadAll<GameObject>(Constants.ROOM_TEMPLATES_FOLDER + Constants.DUNGEON_ROOMS_FOLDER);

        foreach (GameObject room in dungeonRooms)
        {
            foreach (Transform child in room.transform)
            {
                if (child.name == Constants.ENTRANCE_CONTAINER)
                {
                    foreach (Transform entrance in child)
                    {
                        switch (entrance.name)
                        {
                            case Constants.EXIT_BOT:
                                dungeonBrooms.Add(room);
                                break;
                            case Constants.EXIT_TOP:
                                dungeonTrooms.Add(room);
                                break;
                            case Constants.EXIT_RIGHT:
                                dungeonRrooms.Add(room);
                                break;
                            case Constants.EXIT_LEFT:
                                dungeonLrooms.Add(room);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        grassRooms = Resources.LoadAll<GameObject>(Constants.ROOM_TEMPLATES_FOLDER + Constants.GRASS_ROOMS_FOLDER);

        foreach (GameObject room in grassRooms)
        {
            foreach (Transform child in room.transform)
            {
                if (child.name == Constants.ENTRANCE_CONTAINER)
                {
                    foreach (Transform entrance in child)
                    {
                        switch (entrance.name)
                        {
                            case Constants.EXIT_BOT:
                                grassBrooms.Add(room);
                                break;
                            case Constants.EXIT_TOP:
                                grassTrooms.Add(room);
                                break;
                            case Constants.EXIT_RIGHT:
                                grassRrooms.Add(room);
                                break;
                            case Constants.EXIT_LEFT:
                                grassLrooms.Add(room);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


        foreach(Biome biome in Enum.GetValues(typeof(Biome)))
        {
            if (!roomsPerBiome.ContainsKey(biome))
            {
                roomsPerBiome[biome] = new BiomeTemplate(biome);
            }
        }

        Debug.Log($"Biomes added: {roomsPerBiome.Keys}");

    }
    


}
