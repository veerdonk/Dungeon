using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Unarmed Enemy")]
public class UnarmedEnemy : Enemy
{
    public int damage;
    public float range;
    public float knockback;
    public float delay;
    public float attackDuration;
    public Color color;
}
