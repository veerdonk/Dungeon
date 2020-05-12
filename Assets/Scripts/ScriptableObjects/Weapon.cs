using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Melee Weapon")]
public class Weapon : Item
{
    public WeaponType type;
    public int damage;
    public float range = 5f;
    public float knockback;
    public float delay = .3f;
    public float animationSpeed = 1f;
    public float rotationSpeed = 20f;
    public float throwSpeed = 15f;
    public float chanceToBreak = 33f;
    public float sharpness = 50f;
    public int critChance = 10;
    public float sizeModifier = 1f;
    public PolygonCollider2D collider;
}
