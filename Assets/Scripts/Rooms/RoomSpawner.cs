using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RoomSpawner : MonoBehaviour
{
    public static RoomSpawner instance;

    //TODO refactor tp only use Room objects
    private Dictionary<int, Dictionary<int, GameObject>> roomGrid = new Dictionary<int, Dictionary<int, GameObject>>();
    private Dictionary<int, Dictionary<int, bool>> hasNeighbours = new Dictionary<int, Dictionary<int, bool>>();
    private Dictionary<int, Dictionary<int, List<string>>> roomExits = new Dictionary<int, Dictionary<int, List<string>>>();

    private Dictionary<Vector3, Room> rooms = new Dictionary<Vector3, Room>();

    public int playerGridLocX = 0;
    public int playerGridLocY = 0;

    public Vector3 playerGridPosition;
    public GameObject chestPrefab;
    public GameObject weaponDropPrefab;
    [SerializeField] private Transform shadowWallPrefab;
    List<Transform> shadowWalls;

    private string direction;
    private RoomTemplates templates;

    PlayerController2D playerController;

    List<Transform> possibleEntrances;
    List<string> exitCodes = new List<string>{Constants.EXIT_TOP, Constants.EXIT_RIGHT, Constants.EXIT_LEFT, Constants.EXIT_BOT};
    bool isFirstRoom = true;

    //TODO refactor to dictionary?
    private List<Corridor> corridors = new List<Corridor>();

    Transform enemyParent;

    [SerializeField] private NPCSpawner npcSpawner;


    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one instance of RoomSpawner present");
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Retrieve player controller to move player with
        playerController = PlayerController2D.instance;

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
        AddRoom(templates.rooms[0]);

        AddExits(gameObject.transform.GetChild(0));

        //Register event for roomclear
        EnemySpawner.instance.OnAllEnemiesCleared += SpawnLoot;

        
    }

    public GameObject getCurrentRoom()
    {
        return rooms[new Vector3(playerGridLocX, playerGridLocY)].room;
    }

    private void AddExits(Transform room)
    {
        Tilemap walls = null;
        Tilemap floor = null;

        possibleEntrances = new List<Transform>();

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
        
        //Get a list of possible exits - the entrance
        List<string> tempExitCodes = exitCodes.ToList();
        
        int start = 0;
        Dictionary<int, string> exitCodesToAdd = new Dictionary<int, string>();

        if (direction != null){
            tempExitCodes.Remove(Constants.opositeEntrance[direction]);
            exitCodesToAdd.Add(0, Constants.opositeEntrance[direction]);
            start = 1;
        }

        int numEntrances = Random.Range(1 + start, 3 + start);
        
        for (int i = start; i < numEntrances; i++)
        { 
            string code = tempExitCodes[Random.Range(0, tempExitCodes.Count)];
            exitCodesToAdd.Add(i, code);
            tempExitCodes.Remove(code);
        }


        shadowWalls = new List<Transform>();
        //Loop through the codes to add
        for (int i = 0; i < exitCodesToAdd.Count; i++)
        {
            //Get the corridor with the right direction
            foreach (Corridor cor in corridors)
            {
                if (cor.hasDirection(exitCodesToAdd[i])) {
                    Debug.Log($"Corridor selected for code'{exitCodesToAdd[i]}' = '{cor.name}'");
                    //Find the matching entrance transform
                    foreach (Transform entrance in possibleEntrances)
                    {
                        if(entrance.name == exitCodesToAdd[i])
                        {
                            bool createExit = true;
                            
                            //Check room in direction to see if exit should be added
                            switch (entrance.name)
                            {
                                case Constants.EXIT_TOP:
                                    //Check if room above (y+1) has bot exit
                                    if (CheckGridLocationOffset(0, 1))
                                    {
                                        createExit = roomExits[playerGridLocX][playerGridLocY + 1].Contains(Constants.EXIT_BOT);
                                    }
                                    break;
                                case Constants.EXIT_BOT:
                                    //Check if room above (y-1) has bot exit
                                    if (CheckGridLocationOffset(0, -1))
                                    {
                                        createExit = roomExits[playerGridLocX][playerGridLocY - 1].Contains(Constants.EXIT_TOP);
                                    }
                                    break;
                                case Constants.EXIT_LEFT:
                                    //Check if room above (y+1) has bot exit
                                    if (CheckGridLocationOffset(-1, 0))
                                    {
                                        createExit = roomExits[playerGridLocX - 1][playerGridLocY].Contains(Constants.EXIT_RIGHT);
                                    }
                                    break;
                                case Constants.EXIT_RIGHT:
                                    //Check if room above (y+1) has bot exit
                                    if (CheckGridLocationOffset(1, 0))
                                    {
                                        createExit = roomExits[playerGridLocX + 1][playerGridLocY].Contains(Constants.EXIT_LEFT);
                                    }
                                    break;
                                default:
                                    Debug.LogWarning("No behaviour for entrance name: " + entrance.name);
                                    break;
                            }

                            //Only actually create exit if there is no neighbour or neighbour also has an exit there
                            if (createExit)
                            {
                                //Add exit/entrance to our grid
                                if (roomExits.ContainsKey(playerGridLocX))
                                {
                                    if (roomExits[playerGridLocX].ContainsKey(playerGridLocY))
                                    {
                                        roomExits[playerGridLocX][playerGridLocY].Add(entrance.name);
                                    }
                                    else
                                    {
                                        roomExits[playerGridLocX][playerGridLocY] = new List<string> { entrance.name };
                                    }
                                }
                                else
                                {
                                    roomExits[playerGridLocX] = new Dictionary<int, List<string>> { { playerGridLocY, new List<string> { entrance.name } } };
                                }
                                //This is the entrance we want to add
                                AddExit(floor, walls, cor, entrance);
                            }
                        }
                    }
                    
                }
            }

        }

        rooms[new Vector3(playerGridLocX, playerGridLocY)].walls = shadowWalls;
        //Pass floor tilemap to function that finds possible spawn tiles/coords
        FindPossibleSpawnPoints(floor);

    }

    private bool CheckGridLocationOffset(int offsetX, int offsetY)
    {
        Debug.Log($"Checking X={playerGridLocX + offsetX}, Y={playerGridLocY + offsetY}");
        return CheckGridLocationForRoom(playerGridLocX + offsetX, playerGridLocY + offsetY);
    }

    private bool CheckGridLocationForRoom(int x, int y)
    {
        if (roomGrid.ContainsKey(x))
        {
            return roomGrid[x].ContainsKey(y);
        }
        else
        {
            return false;
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
        Vector3 triggerPosition = new Vector3();
        

        //Set position of new corridor
        Vector3Int newPositionWall = new Vector3Int();
        Vector3Int newPositionFloor = new Vector3Int();
        Vector3 wallPos = entranceToAdd.position;
        float wallRotation = 0;
        switch (entranceToAdd.name)
        {
            case Constants.EXIT_LEFT:
                //For this to work the 'opening'of the entrance has to be -exactly- on X=0
                newPositionWall.x = initialPosition.x + corridor.wallBounds.xMin;
                newPositionFloor.x = initialPosition.x + corridor.floorBounds.xMin;
                Debug.Log(newPositionWall);
                // 0 > 0?
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin;
                //wallPos.x -= 0.2f;
                wallPos.y += 0.5f;
                wallRotation -= 90;
                break;

            case Constants.EXIT_RIGHT:
                //X position is the same
                newPositionWall.x = initialPosition.x;
                newPositionFloor.x = initialPosition.x;
                //set Y position to smallest bounds y pos
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin;
                wallRotation += 90;
                wallPos.x += 1f;
                wallPos.y += 0.5f;
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
                int startX = newPositionWall.x + 1;// -(int)Constants.ENTRANCE_WIDTH / 2; //-1
                int startY = (int)initialPosition.y - 1;
                for (int i = startX; i < Constants.ENTRANCE_WIDTH + startX; i++)
                {
                    RemoveTile(walls, new Vector3Int(i, startY, 0));
                }

                wallPos.x += 0.5f;
                wallPos.y += 0.2f;
                wallRotation += 180;
                break;

            case Constants.EXIT_BOT:
                //Set X position
                newPositionWall.x = initialPosition.x + corridor.wallBounds.xMin;
                newPositionFloor.x = initialPosition.x + corridor.floorBounds.xMin;
                //set Y
                //For this to work the 'opening' of the corridor has to be at -exaclty- Y=0
                newPositionWall.y = initialPosition.y + corridor.wallBounds.yMin - 1;
                newPositionFloor.y = initialPosition.y + corridor.floorBounds.yMin - 1;
                //Lightly tweak position of wall
                wallPos.x += 0.5f;
                wallPos.y -= 1f;
                break;

            default:
                Debug.LogWarning($"No behaviour for exit with code: {entranceToAdd.name}");
                break;
        }

        

        if (!isFirstRoom)
        {
            AddShadowWall(wallPos, wallRotation);
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
        corTrigger.transform.position = initialPosition + corridor.trigger.transform.position;//floorBounds.center +  corridor.trigger.transform.position;

    }

    void AddShadowWall(Vector3 position, float rotation)
    {
        Transform shadowWall = Instantiate(shadowWallPrefab, getCurrentRoom().transform);
        shadowWall.position = position;
        shadowWall.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        shadowWalls.Add(shadowWall);
    }

    public void EnableShadowWalls()
    {
        foreach (Transform shadowWall in shadowWalls)
        {
            shadowWall.GetComponent<ShadowTrigger>().EnableWall();
        }
    }

    public void DisableShadowWalls()
    {
        foreach (Transform shadowWall in shadowWalls)
        {
            shadowWall.gameObject.SetActive(false);
        }
    }

    //Checks whether room exists in collection and adds if it doesnt
    //Also initializes the hasNeigbours collection
    private GameObject AddRoom(GameObject room)
    {
        Debug.Log("Adding room: " + room.name);
        GameObject roomObject = Instantiate(room, gameObject.transform);


        Vector3 gridLocVector = new Vector3(playerGridLocX, playerGridLocY);
        Room roomToAdd = new Room(roomObject, gridLocVector);
        if (rooms.ContainsKey(gridLocVector))
        {
            Debug.LogWarning("Room already exists at position: " + gridLocVector.ToString());

        }
        else
        {
            rooms[gridLocVector] = roomToAdd;
        }

        //Set new room as parent for enemies
        enemyParent = roomObject.transform;
        int x = playerGridLocX;
        int y = playerGridLocY;

        //roomsObj.name += $"_X{x}_Y{y}";

        if (roomGrid.ContainsKey(x))
        {
            if (!roomGrid[x].ContainsKey(y))
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

        AstarPath.active.Scan();

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
        //Disable old walls
        foreach (Transform wall in rooms[new Vector3(playerGridLocX, playerGridLocY)].walls)
        {
            wall.GetComponent<ShadowTrigger>().DisableWall();
        }
        Vector3 newPlayerPos = new Vector3();

        //Create new room in right direction
        int playerXOfsset = 0;
        int playerYOfsset = 0;

        GameObject roomToAdd;

        switch (direction)
        {
            case Constants.EXIT_TOP:
                playerGridLocY++;
                playerYOfsset = -2;
                roomToAdd = templates.Brooms[Random.Range(0, templates.Brooms.Count)];
                break;

            case Constants.EXIT_BOT:
                playerGridLocY--;
                playerYOfsset = 2;
                roomToAdd = templates.Trooms[Random.Range(0, templates.Trooms.Count)];
                break;
            case Constants.EXIT_RIGHT:
                playerGridLocX++;
                playerXOfsset = -2;
                roomToAdd = templates.Lrooms[Random.Range(0, templates.Lrooms.Count)];
                break;

            case Constants.EXIT_LEFT:
                playerGridLocX--;
                playerXOfsset = 2;
                roomToAdd = templates.Rrooms[Random.Range(0, templates.Rrooms.Count)];
                break;

            default:
                roomToAdd = null;
                Debug.LogError($"Entrance code unknown: {direction}");
                break;
        }

        GameObject addedRoom = null;
        if (roomGrid.ContainsKey(playerGridLocX))
        {
            if (!roomGrid[playerGridLocX].ContainsKey(playerGridLocY)){
                //X exists, Y does not
                addedRoom = AddRoom(roomToAdd);
                AddExits(addedRoom.transform);
            }
            else
            {
                //X/Y both exist -> enable existing room
                addedRoom = roomGrid[playerGridLocX][playerGridLocY];
                roomGrid[playerGridLocX][playerGridLocY].SetActive(true);
                //foreach (Transform wall in rooms[new Vector3(playerGridLocX, playerGridLocY)].walls)
                //{
                //    wall.GetComponent<ShadowTrigger>().EnableWall();
                //} 
            }
        }
        else
        {
            //X/Y do not exist
            addedRoom = AddRoom(templates.rooms[Random.Range(0, templates.rooms.Length)]);
            AddExits(addedRoom.transform);
        }

        foreach (Transform entrance in possibleEntrances)
        {
            if (entrance.name == Constants.opositeEntrance[direction])
            {
                newPlayerPos = entrance.position;
                break;
            }
        }

        newPlayerPos.x += playerXOfsset;
        newPlayerPos.y += playerYOfsset;

        //Set player to right spot
        playerController.setPlayerPosition(newPlayerPos);

        //Set direction to origin dir
        direction = Constants.opositeEntrance[direction];
        //Once all exits have been added rescan the A* pathfinding
        //AstarPath.active.Scan();
    }

    //Used to determine viable spawnpoints
    public void FindPossibleSpawnPoints(Tilemap floor)
    {
        List<Vector3> allowedSpawnPoints = new List<Vector3>();

        //Look at floor tilemap
        BoundsInt floorBounds = floor.cellBounds;
        TileBase[] allFloorTiles = floor.GetTilesBlock(floorBounds);

        Debug.Log("Finding all possible spawnlocations for tilemap: " + floor.name);
        //Loop through all floor positions
        foreach (var pos in floor.cellBounds.allPositionsWithin)
        {
            //Only check positions that actually have a tile
            if (floor.HasTile(pos))
            {
                
                bool isMissingNeighbours = false;

                //Check all neighbours of tile
                //    X       Y
                //-1, 0, 1  1,1,1
                //-1, 0, 1  0,0,0
                //-1, 0, 1  -1-1-1
                for (int lx = -2; lx < 3; lx++)
                {
                    for (int ly = -2; ly < 3; ly++)
                    {
                        Vector3Int newPos = pos;
                        newPos.x += lx;
                        newPos.y += ly;
                        //Check if neibour is null
                        if (!floor.HasTile(newPos))
                        {
                            isMissingNeighbours = true;
                        }
                    }
                }
                //If no neighbours were missing add tile coordinates to list of allowed spawn points
                if (!isMissingNeighbours)
                {
                    allowedSpawnPoints.Add(pos);
                }
            }


        }
        rooms[new Vector3(playerGridLocX, playerGridLocY)].possibleSpawnPoints = allowedSpawnPoints;

        if (isFirstRoom)
        {
            //Spawn some a chest w/ weapons to start with
            SpawnLoot();
            //also spawn a tutorial npc
            npcSpawner.SpawnNPC("Tutorial", allowedSpawnPoints[Random.Range(0, allowedSpawnPoints.Count)]);

            isFirstRoom = false;
        }
        else
        {
            //Once positions are known instruct enemyspawner to instantiate enemies
            EnemySpawner.instance.SpawnEnemies(allowedSpawnPoints, enemyParent, new Vector3(playerGridLocX, playerGridLocY, 0));
            EnemySpawner.instance.OnAllEnemiesCleared += DisableShadowWalls;
        }


    }

    public List<Vector3> FindLocationsNearPoint(Vector3 point, int numPoints)
    {
        //open spawnlocations of room
        List<Vector3> allSpawnPoints = rooms[new Vector3(playerGridLocX, playerGridLocY)].possibleSpawnPoints.ToList();
        //If spawnlocations contains our starting point -> remove it
        if (allSpawnPoints.Contains(point)) 
        {
            allSpawnPoints.Remove(point);
        }
        List<Vector3> closePoints = new List<Vector3>();



        for (int i = 0; i < numPoints; i++)
        {
            Vector3 closestVec =  new Vector3();
            float minDist = Mathf.Infinity;
            foreach (Vector3 spawnLoc in allSpawnPoints)
            {
                float dist = Vector3.Distance(spawnLoc, point);
                if (dist < minDist)
                {
                    closestVec = spawnLoc;
                    minDist = dist;
                }
            }
            closePoints.Add(closestVec);
            allSpawnPoints.Remove(closestVec);
        }

        Debug.Log($"Close points to '{point}' are:");
        foreach (Vector3 vec in closePoints)
        {
            Debug.Log(vec);
        }

        return closePoints;
    }

    //Spawn loot on a random available location in the room
    private void SpawnLoot()
    {

        //Select random location for loot/chest
        Vector3 lootPoint = rooms[new Vector3(playerGridLocX, playerGridLocY)].possibleSpawnPoints[Random.Range(0, rooms[new Vector3(playerGridLocX, playerGridLocY)].possibleSpawnPoints.Count)];
        Debug.Log("Spawning loot at position: " + lootPoint.ToString());
        if (isFirstRoom)
        {
            List<Item> items = new List<Item>();
            //Player starts out with 3 green items
            for (int i = 0; i < 3; i++)
            {

                items.Add(Inventory.instance.greenItems[Random.Range(0, Inventory.instance.greenItems.Count)]);
            }

            SpawnItemChest(items, lootPoint);
        }
        else
        {

            //Chance for items or coins
            //Chance to select different items
            int chance = Random.Range(0, 100);
            List<Item> items = new List<Item>();
            if (chance < Constants.CHANCE_CHEST_HAS_COINS)
            {
                SpawnCoinChest(Random.Range(15, 40), lootPoint);
            }
            else if(chance < Constants.CHANCE_CHEST_HAS_WHITE)
            {
                for (int i = 0; i < Random.Range(1, Constants.MAX_GREEN_ITEM_SPAWN); i++)
                {

                    items.Add(Inventory.instance.whiteItems[Random.Range(0, Inventory.instance.whiteItems.Count)]);
                }
                SpawnItemChest(items, lootPoint);
                return;
            }
            else if(chance < Constants.CHANCE_CHEST_HAS_GREEN)
            { 
                //Spawn a chest with green items
                for (int i = 0; i < Random.Range(1, Constants.MAX_GREEN_ITEM_SPAWN); i++)
                {

                    items.Add(Inventory.instance.greenItems[Random.Range(0, Inventory.instance.greenItems.Count)]);
                }
                SpawnItemChest(items, lootPoint);
                return;
            }
            else if(chance < Constants.CHANCE_CHEST_HAS_PURPLE)
            {
                //Spawn a chest with epic items
                for (int i = 0; i < Random.Range(1, Constants.MAX_PURPLE_ITEM_SPAWN); i++)
                {

                    items.Add(Inventory.instance.purpleItems[Random.Range(0, Inventory.instance.purpleItems.Count)]);
                }
                SpawnItemChest(items, lootPoint);
                return;
            }
            else
            {
                //Spawn a chest with legendary items
                for (int i = 0; i < Random.Range(1, Constants.MAX_LEGEND_ITEM_SPAWN); i++)
                {

                    items.Add(Inventory.instance.legendaryItems[Random.Range(0, Inventory.instance.legendaryItems.Count)]);
                }
                SpawnItemChest(items, lootPoint);
                return;
            }
 

        }
        
    }

    private void SpawnCoinChest(int amount, Vector3 position)
    {
        GameObject chestObj = Instantiate(chestPrefab, rooms[new Vector3(playerGridLocX, playerGridLocY)].room.transform);
        Chest chest = chestObj.GetComponentInChildren<Chest>();
        chestObj.transform.position = position;
        chest.type = ChestType.COIN;
        chest.coinCount = amount;
    }

    private void SpawnItemChest(List<Item> itemNames, Vector3 position)
    {
        GameObject chestObj = Instantiate(chestPrefab, rooms[new Vector3(playerGridLocX, playerGridLocY)].room.transform);
        chestObj.transform.position = position;
        Chest chest = chestObj.GetComponentInChildren<Chest>();
        chest.type = ChestType.ITEM;
        chest.items = itemNames;
    }

}

