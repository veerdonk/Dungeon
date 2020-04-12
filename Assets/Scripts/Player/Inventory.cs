using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public bool[] weaponSlotsFull;
    public GameObject[] weaponslots;

    public int currentSelectedSlot = 0;


    public GameObject GetCurrentWeapon()
    {
        return weaponslots[currentSelectedSlot];
    }

    public string GetCurrentWeaponName()
    {
        Debug.Log(GetCurrentWeapon().GetComponentsInChildren<Image>()[1].sprite.name);
        return GetCurrentWeapon().GetComponentsInChildren<Image>()[1].sprite.name;
    }

}
