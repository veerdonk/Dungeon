using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    //Weapon statnames
    public const string PARAM_DAMAGE = "damage";
    public const string PARAM_RANGE = "range";
    public const string PARAM_KNOCKBACK = "knockback";
    public const string PARAM_DELAY = "delay";
    public const string PARAM_ANIM_SPEED = "animation_speed";
    public const string PARAM_WEAPON_TYPE = "weapon_type";

    //Folders
    public const string PREFABS_FOLDER = "Prefabs/";
    public const string PICKUPS_FOLDER = "Pickups/";
    public const string WEAPONS_FOLDER = "Weapons/";
    public const string PICKUP_SUFFIX = "_pickup";
    public const string ROOMS_FOLDER = "Rooms/";
    public const string ROOM_TEMPLATES_FOLDER = "Room_templates/";
    public const string CORRIDORS_FOLDER = PREFABS_FOLDER + ROOMS_FOLDER + ROOM_TEMPLATES_FOLDER + "Corridors/";


    //Controls
    public const KeyCode SWITCH_WEAPON_KEY = KeyCode.Q;
    public const KeyCode PICKUP_WEAPON_KEY = KeyCode.R;

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


    //damage            -> only whole numbers (is cast to int)
    //Range             -> Size of scanning circle
    //knockback         -> How far to knock enemy back (300+)
    //delay             -> Minimum delay between attacks (seconds)
    //animation_speed   -> Speed for animation (1 = normal speed, 0.5 half speed, 2 double speed)
    public static Dictionary<string, Dictionary<string, object>> weaponNameToStats = new Dictionary<string, Dictionary<string, object>>()
    {
        { "weapon_rusty_sword", new Dictionary<string, object>(){ { "weapon_type",  WeaponType.SWORD }, { "damage", 15 }, { "range", 0.7f} , { "knockback", 400f }, { "delay", 0.3f }, { "animation_speed", 1.2f } } },
        { "weapon_cleaver", new Dictionary<string, object>(){ { "weapon_type", WeaponType.SWORD }, { "damage", 25 }, { "range", 0.8f} , { "knockback", 600f }, { "delay", 0.5f }, { "animation_speed", 0.8f } } }
    };
    
}
