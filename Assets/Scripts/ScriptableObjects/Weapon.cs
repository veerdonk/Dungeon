using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string id;
    public new string name;
    public string description;
    public Sprite sprite;
    public WeaponType type;
    public int damage;
    public float range = 5f;
    public float knockback;
    public float delay = .3f;
    public float animationSpeed = 1f;
    public float rotationSpeed = 20f;
    public float throwSpeed = 15f;

}
