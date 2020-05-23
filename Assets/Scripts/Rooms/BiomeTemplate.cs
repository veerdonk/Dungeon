using System.Collections.Generic;
using UnityEngine;

public class BiomeTemplate
{
    public Biome biome;
    public GameObject[] rooms;
    public List<GameObject> Lrooms = new List<GameObject>();
    public List<GameObject> Rrooms = new List<GameObject>();
    public List<GameObject> Trooms = new List<GameObject>();
    public List<GameObject> Brooms = new List<GameObject>();

    public List<Corridor> corridors = new List<Corridor>();

    void InitBiome()
    {

        rooms = Resources.LoadAll<GameObject>(Constants.biomeToResource[biome]);
        GameObject[] corrObj = Resources.LoadAll<GameObject>(Constants.biomeToCorridor[biome]);
        
        foreach (GameObject corr in corrObj)
        {
            corridors.Add(new Corridor(corr));
        }

        foreach (GameObject room in rooms)
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
                                Brooms.Add(room);
                                break;
                            case Constants.EXIT_TOP:
                                Trooms.Add(room);
                                break;
                            case Constants.EXIT_RIGHT:
                                Rrooms.Add(room);
                                break;
                            case Constants.EXIT_LEFT:
                                Lrooms.Add(room);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public BiomeTemplate(Biome biome)
    {
        this.biome = biome;
        InitBiome();
    }
}
