using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomTemplates : MonoBehaviour
{

    public GameObject[] rooms;


    void Awake()
    {

        string roomTemplateFolder = Constants.ROOM_TEMPLATES_FOLDER;

        rooms = Resources.LoadAll<GameObject>(Constants.ROOM_TEMPLATES_FOLDER + Constants.ROOMS_FOLDER);

    }



}
