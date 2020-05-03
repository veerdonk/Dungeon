using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Standard Enemy")]
public class Enemy : ScriptableObject
{
    public int health = 100;
    public float staggerTime = 0.8f;
    public float speed = 350f;
    public float attackSpeedModifier = 1f;
    public float damageModifier = 1f;
    public float followDistance = 1f;
    public float minDistance = 0f;
    public float sizeModifier = 1f;
    public int coinValue;
    public int expValue;
    public EnemyType type;
    public EnemyRace race;
    public Sprite sprite;
    public int creditCost;
    public int minPlayerLevel = 1;
}

public enum EnemyRace
{
    ORC,
    SKELETON,
    LIZARD_F
}

public enum EnemyType
{
    MELEE,
    ARCHER,
    MAGE
}