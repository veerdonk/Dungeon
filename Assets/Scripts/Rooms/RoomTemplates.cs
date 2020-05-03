using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomTemplates : MonoBehaviour
{

    public GameObject[] rooms;
    public List<GameObject> Lrooms = new List<GameObject>();
    public List<GameObject> Rrooms = new List<GameObject>();
    public List<GameObject> Trooms = new List<GameObject>();
    public List<GameObject> Brooms = new List<GameObject>();

    void Awake()
    {
        rooms = Resources.LoadAll<GameObject>(Constants.ROOM_TEMPLATES_FOLDER + Constants.ROOMS_FOLDER);

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



}
