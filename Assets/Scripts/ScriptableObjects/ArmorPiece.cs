using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armorpiece", menuName = "Armor/Armorpiece")]
public class ArmorPiece : Item
{
    public ArmorPart armorPart;
    public Color color;
    public Sprite[] sprites;

    public float defense = 1;

    public ArmorEffect effect;

    public bool equipped;
}

public enum ArmorEffect
{
    NONE
}

public enum ArmorPart
{
    HEAD,
    TORSO
}