using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Ranged Weapon")]
public class RangedWeapon : Weapon
{
    public GameObject projectile;
    public Sprite projectileSprite;
    public int numberToSpawn;
    public float projectileDelay;
    public float degreesAround;
    public int projectileDamage = 10;
    public FirePoint FirePoint;
}

public enum FirePoint
{
    ONTRHOW,
    ONHIT,
    ONFLY
}
