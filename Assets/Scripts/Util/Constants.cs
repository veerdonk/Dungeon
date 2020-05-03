using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{

    //Chances
    public const int CHANCE_CHEST_HAS_COINS = 30;
    public const int CHANCE_CHEST_HAS_WHITE = 80;
    public const int MAX_GREEN_ITEM_SPAWN = 4;
    public const int CHANCE_WEAPON_BREAKS = 33;

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
    public const string CORRIDORS_FOLDER = ROOM_TEMPLATES_FOLDER + "Corridors/";
    public const string ENEMY_PATH = "ENEMIES/";

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

    //Enemy animation triggers
    public const string SKELETON_ANIM = "IsSkeleton";
    public const string ORC_ANIM = "IsOrc";
    public const string LIZARD_F_ANIM = "IsLizard_f";

    //Weapon statnames - deprecated
    public const string PARAM_DAMAGE = "damage";
    public const string PARAM_RANGE = "range";
    public const string PARAM_KNOCKBACK = "knockback";
    public const string PARAM_DELAY = "delay";
    public const string PARAM_ANIM_SPEED = "animation_speed";
    public const string PARAM_WEAPON_TYPE = "weapon_type";
    public const string PARAM_ROTATION_SPEED = "rotation_speed";
    public const string PARAM_THROW_SPEED = "throw_speed";

    //Enemies
    public static Dictionary<EnemyType, List<WeaponType>> enemyTypeToWeaponType = new Dictionary<EnemyType, List<WeaponType>>()
    {
        { EnemyType.MELEE, new List<WeaponType>{WeaponType.SWORD } },
        { EnemyType.ARCHER, new List<WeaponType>{WeaponType.BOW } },
        { EnemyType.MAGE, new List<WeaponType>() }
    };
    public const string ENEMY_FIRE_BOW = "FireBow";


}