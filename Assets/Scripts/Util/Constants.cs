using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{

    //Folders
    public const string PREFABS_FOLDER = "Prefabs/";
    public const string PICKUPS_FOLDER = "Pickups/";
    public const string WEAPONS_FOLDER = "Weapons/";
    public const string SPRITES_FOLDER = "Sprites/";
    public const string WEAPONS_FOLDER_FULL = SPRITES_FOLDER + "Weapons/";
    public const string ITEMS_FOLDER_FULL = SPRITES_FOLDER + "Items/";
    public const string PICKUP_SUFFIX = "_pickup";
    public const string ROOMS_FOLDER = "Rooms/";
    public const string ROOM_TEMPLATES_FOLDER = PREFABS_FOLDER + ROOMS_FOLDER + "Room_templates/";
    public const string CORRIDORS_FOLDER =  ROOM_TEMPLATES_FOLDER + "Corridors/";

    //Controls
    public const KeyCode SWITCH_WEAPON_KEY = KeyCode.Q;
    public const KeyCode PICKUP_WEAPON_KEY = KeyCode.R;
    public const string INVENTORY_BUTTON_TAG = "Inventory";

    //Room generation
    public const string EXIT_TOP = "T";
    public const string EXIT_BOT = "B";
    public const string EXIT_RIGHT = "R";
    public const string EXIT_LEFT = "L";
    public const string ENTRANCE_CONTAINER = "Entrances";
    public const string GRID_NAME = "Grid";
    public const string TILEMAP_FLOOR_NAME = "Floor";
    public const string TILEMAP_WALLS_NAME = "Walls";
    public const string ROOM_COORDS_NAME = "RoomCoords";
    public const string TOP_LEFT = "TopLeft";
    public const string BOT_RIGHT = "BotLeft";
    public const int ENTRANCE_WIDTH = 3;
    public static Dictionary<string, string> opositeEntrance = new Dictionary<string, string>()
    {
        { "T", "B" },
        { "B", "T" },
        { "L", "R" },
        { "R", "L" }
    };

    //Tags
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string WALL_TAG = "Wall";
    public const string ROOMS_TAG = "Rooms";
    public const string INVENTORY_ICON_TAG = "InventoryIcon";


    //Weapon statnames
    public const string PARAM_DAMAGE = "damage";
    public const string PARAM_RANGE = "range";
    public const string PARAM_KNOCKBACK = "knockback";
    public const string PARAM_DELAY = "delay";
    public const string PARAM_ANIM_SPEED = "animation_speed";
    public const string PARAM_WEAPON_TYPE = "weapon_type";
    public const string PARAM_ROTATION_SPEED= "rotation_speed";
    public const string PARAM_THROW_SPEED= "throw_speed";

}