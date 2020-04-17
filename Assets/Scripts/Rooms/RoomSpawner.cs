using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RoomSpawner : MonoBehaviour
{
    private GameObject roomsObj;

    private Dictionary<int, Dictionary<int, GameObject>> roomGrid = new Dictionary<int, Dictionary<int, GameObject>>();
    private Dictionary<int, Dictionary<int, bool>> hasNeighbours = new Dictionary<int, Dictionary<int, bool>>();
    public int playerGridLocX = 0;
    public int playerGridLocY = 0;

    private string direction = "N";
    private RoomTemplates templates;

    PlayerController2D playerController;

    List<Transform> possibleEntrances;

    //TODO refactor to dictionary?
    private List<Corridor> corridors = new List<Corridor>();

    // Start is called before the first frame update
    void Start()
    {
        //Retrieve player controller to move player with
        playerController = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).GetComponent<PlayerController2D>();

        //Initialize corridors
        GameObject[] corridorTemplates = Resources.LoadAll<GameObject>(Constants.CORRIDORS_FOLDER);
        foreach (GameObject corr in corridorTemplates)
        {
            corridors.Add(new Corridor(corr));
        }

        Debug.Log("Spawning corridors");

        //Possible room templates
        templates = GetComponent<RoomTemplates>();

        //Add first/start room 
        addRoom(templates.rooms[0]);

        AddExits(gameObject.transform.GetChild(0));



        //Get the first/start room and add it to our grid
        //roomGrid[0][0] = gameObject.transform.GetChild(0);


        //TODO uncomment
        //Initially spawn neighbours for start room
        //UpdatePlayerLocationAndSpawnRoom(0, 0, "No origin direction");

    }

    private void AddExits(Transform room)
    {
        Tilemap walls = null;
        Tilemap floor = null;

        possibleEntrances = new List<Transform>();

        Vector3 topLeft = new Vector3();
        Vector3 botRight = new Vector3();

        //Select the floor and wall tilemaps of room
        //Also retrieve all entrance objects
        foreach (Transform child in room)
        {
            if (child.name == Constants.GRID_NAME)
            {
                foreach (Transform tilemapTransform in child)
                {
                    if (tilemapTransform.name == Constants.TILEMAP_FLOOR_NAME)
                    {
                        floor = tilemapTransform.GetComponent<Tilemap>();
                    }
                    else if (tilemapTransform.name == Constants.TILEMAP_WALLS_NAME)
                    {
                        walls = tilemapTransform.GetComponent<Tilemap>();
                    }
                }
            }
            else if (child.name == Constants.ENTRANCE_CONTAINER)
            {
                foreach (Transform entrance in child)
                {
                    possibleEntrances.Add(entrance);
                }
            }
        }

        //TODO change to smart/random process
        //select opening

        Dictionary<int, string> test = new Dictionary<int, string>()
        {
            {0,"T" },
            {1,"L" },
            {2,"R" },
            {3,"B" }
        };

        for (int i = 0; i < 4; i++)
        {

            foreach (Corridor cor in corridors)
            {
                if (cor.hasDirection(test[i])) {
                    AddExit(floor, walls, cor, possibleEntrances[i]);
                }
            }

        }

    }

    private void RemoveTile(Tilemap tm, Vector3Int position)
    {
        tm.SetTile(position, null);
    }

    private void AddExit(Tilemap floor, Tilemap walls, Corridor corridor, Transform entranceToAdd)
    {

        //Get position of entrance
        Vector3Int initialPosition = floor.WorldToCell(entranceToAdd.position);

        //Debug.Log($"Entrance to add x= {entranceToAdd.position.x}\ncorridorWallBounds x= {corridor.wallBounds.x}" +
        //    $"\ncorridorWallBounds.min = {corridor.wallBounds.min}" +
        //    $"\ncorridorWallBounds.max = {corridor.wallBounds.max}" +
        //    $"\ncorridorWallBounds.position = {corridor.wallBounds.position}" +
        //    $"\ncorridorWallBounds.size = {corridor.wallBounds.size}");


        //Set position of new corridor
        Vector3Int newPositionWall = new Vector3Int();
        Vector3Int newPositionFloor = new Vector3Int();
        switch (entranceToAdd.name)
        {
            case Constants.EXIT_LEFT:
                //substract twice the distance of the center to the left most point
                newPositionWall.x = initialPosition.x + corridor.wallBounds.xMin * 2;
                newPositionFloor.x = initialPosition.x + corridor.floorBounds.xMin * 2;
                // 0 > 0?
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin;
                break;

            case Constants.EXIT_RIGHT:
                //X position is the same
                newPositionWall.x = initialPosition.x;
                newPositionFloor.x = initialPosition.x;
                //set Y position to smallest bounds y pos
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin;
                break;

            case Constants.EXIT_TOP:
                //Set X position
                newPositionWall.x = initialPosition.x + corridor.wallBounds.xMin;
                newPositionFloor.x = initialPosition.x + corridor.floorBounds.xMin;
                //set Y
                newPositionWall.y = initialPosition.y;
                //offset by 1 because the walls have to be filled in
                newPositionFloor.y = initialPosition.y - 1;

                //Remove extra walls
                //Get relative x and y start position
                int startX = -(int)Constants.ENTRANCE_WIDTH / 2; //-1
                int startY = (int)initialPosition.y - 1;
                for (int i = startX; i < Constants.ENTRANCE_WIDTH + startX; i++)
                {
                    RemoveTile(walls, new Vector3Int(i, startY, 0));
                }

                break;

            case Constants.EXIT_BOT:
                //Set X position
                newPositionWall.x = initialPosition.x + corridor.wallBounds.xMin;
                newPositionFloor.x = initialPosition.x + corridor.floorBounds.xMin;
                //set Y
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin * 2 - 1;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin * 2 - 1;

                break;

            default:
                Debug.LogWarning($"No behaviour for exit with code: {entranceToAdd.name}");
                break;
        }

        //Debug.Log($"\nnew position wall = {newPositionWall}\n" +
            //$"new position floor = {newPositionFloor}");
        BoundsInt wallBounds = new BoundsInt(newPositionWall, corridor.wallBounds.size);
        BoundsInt floorBounds = new BoundsInt(newPositionFloor, corridor.floorBounds.size);

        //Set the actual tiles
        walls.SetTilesBlock(wallBounds, corridor.wallTiles);
        floor.SetTilesBlock(floorBounds, corridor.floorTiles);

        //Add the corridor trigger
        GameObject corTrigger = Instantiate(corridor.trigger, roomGrid[playerGridLocX][playerGridLocY].transform);
        corTrigger.transform.position = floorBounds.center +  corridor.trigger.transform.position;

    }

    //Checks whether room exists in collection and adds if it doesnt
    //Also initializes the hasNeigbours collection
    private GameObject addRoom(GameObject room)
    {

        GameObject roomObject = Instantiate(room, gameObject.transform);
        int x = playerGridLocX;
        int y = playerGridLocY;

        if (roomGrid.ContainsKey(x))
        {
            if (!roomGrid.ContainsKey(y))
            {
                //X coord exist but room doesnt
                roomGrid[x].Add(y, roomObject);
                hasNeighbours[x].Add(y, false);
                Debug.Log($"New entry ({roomObject.name}) in roomgrid added at EXISTING x = {x} and NEW y = {y}");
            }
        }
        else
        {
            //both X and Y coords do not exist
            roomGrid.Add(x, new Dictionary<int, GameObject>() { { y, roomObject } });
            hasNeighbours.Add(x, new Dictionary<int, bool>() { { y, false } });
            Debug.Log($"New entry ({roomObject.name}) in roomgrid added at x = {x} and y = {y}");
        }

        return roomObject;
    }

    public void setDirectionFlag(string dir)
    {
        direction = dir;
    }

    public void UpdatePlayerLocationAndSpawnRoom(GameObject corridor)
    {
        //Retrieve information about corridor
        Corridor corridorData = null;
        foreach (Corridor corr in corridors)
        {
            if (corridor.name == corr.name)
            {
                corridorData = corr;
            }
        }

        //Disable old room
        roomGrid[playerGridLocX][playerGridLocY].SetActive(false);
        Vector3 newPlayerPos = new Vector3();

        foreach (Transform entrance in possibleEntrances)
        {
            if (entrance.name.Equals(Constants.opositeEntrance[direction]))
            {
                newPlayerPos = entrance.position;
            }
        }


       

        //Create new room in right direction
        switch (direction)
        {
            case Constants.EXIT_TOP:
                playerGridLocY++;
                newPlayerPos.y -= 2;
                break;

            case Constants.EXIT_BOT:
                playerGridLocY--;
                newPlayerPos.y += 2;
                break;
            case Constants.EXIT_RIGHT:
                playerGridLocX++;
                newPlayerPos.x -= 2;
                break;

            case Constants.EXIT_LEFT:
                playerGridLocX--;
                newPlayerPos.x += 2;
                break;

            default:
                Debug.LogError($"Entrance code unknown: {direction}");
                break;
        }

        GameObject addedRoom = null;
        if (roomGrid.ContainsKey(playerGridLocX))
        {
            if (!roomGrid[playerGridLocX].ContainsKey(playerGridLocY)){
                //X exists, Y does not
                addedRoom = addRoom(templates.rooms[Random.Range(0, templates.rooms.Length)]);
            }
            else
            {
                //X/Y both exist -> enable existing room
                addedRoom = roomGrid[playerGridLocX][playerGridLocY];
                roomGrid[playerGridLocX][playerGridLocY].SetActive(true);
            }
        }
        else
        {
            //X/Y do not exist
            addedRoom = addRoom(templates.rooms[Random.Range(0, templates.rooms.Length)]);
        }
        
        //Set player to right spot
        playerController.setPlayerPosition(newPlayerPos);
        AddExits(addedRoom.transform);
        
    }






}

