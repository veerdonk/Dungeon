using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController2D : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator animator;

    public float MOVEMENT_BASE_SPEED = 1.0f;

    public Vector2 movementDirection;
    public float movementSpeed = 5f;

    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {

        ProcessInputs();

    }

    private void FixedUpdate()
    {
        Move();
    }


    void ProcessInputs()
    {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDirection.Normalize();
        animator.SetFloat("Speed", movementDirection.sqrMagnitude);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            GetComponentInChildren<Attack>().ExecuteAttack();
        }

        if (Input.GetKeyDown(Constants.SWITCH_WEAPON_KEY)){
            //make sure we currently have a weapon
            if (inventory.weaponSlotsFull[inventory.currentSelectedSlot])
            {
                SwitchWeapon();
            }
        }

    }

    public void SwitchWeapon()
    {

        int slotToSwitchTo = 0;

        if (inventory.currentSelectedSlot == 1)
        {
            slotToSwitchTo = 0;
            inventory.currentSelectedSlot = 0;
        }
        else if (inventory.weaponSlotsFull[1])
        {
            slotToSwitchTo = 1;
            inventory.currentSelectedSlot = 1;
        }

        string nameToLookup = inventory.weaponslots[slotToSwitchTo].GetComponentsInChildren<Image>()[1].sprite.name;
        Dictionary<string, object> weaponStats = Constants.weaponNameToStats[nameToLookup];

        //Get rid of previous weapon
        Destroy(GameObject.FindGameObjectWithTag("weapon"));
        //Instantiate new one
        GameObject newWeapon = (GameObject)Instantiate(Resources.Load(Constants.PREFABS_FOLDER + Constants.WEAPONS_FOLDER + nameToLookup));
        //Parent to player
        newWeapon.transform.parent = transform;
        //make sure its not flipped
        newWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void Move()
    {
        Vector3 newScale = transform.localScale;

        if (movementDirection.x < 0)
        {
            newScale.x = -1;   
        }
        else if( movementDirection.x > 0)
        {
            newScale.x = 1;
        }

        transform.localScale = newScale;
        
        rb.MovePosition(rb.position + movementDirection * movementSpeed * MOVEMENT_BASE_SPEED * Time.fixedDeltaTime);
        
    }

}

public enum WeaponType
{
    SWORD
}
