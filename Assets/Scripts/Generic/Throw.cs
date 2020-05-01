using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Throw : MonoBehaviour
{
    public Weapon weapon;

    public int offset = 45;
    public LayerMask layer;

    public Animator WeaponAnimator;
    public SpriteRenderer weaponRenderer;
    public GameObject pickupVersion;

    Dictionary<string, object> weaponStats;

    public bool isThrown = false;
    bool left = false;
    Vector3 throwPosition;
    float throwRotation;
    Vector3 throwOrigin;

    List<GameObject> enemiesHurt = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        weaponRenderer.sprite = weapon.sprite;
        UpdateWeaponStats(weaponRenderer.sprite.name);
    }
    public void UpdateWeaponStats(string weaponName)
    {
        weaponStats = Constants.weaponNameToStats[weaponName];
        //WeaponAnimator.speed = (float)weaponStats[Constants.PARAM_ANIM_SPEED];
        WeaponAnimator.speed = weapon.animationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!isThrown)
        {
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            float WorldXPos = Camera.main.ScreenToWorldPoint(pos).x;
            int flipOffset;
            if (WorldXPos > transform.position.x) // character it's your char game object
            {
                flipOffset = 0 + offset;
            }
            else
            {
                flipOffset = 180 - offset;
            }

            //Each frame make sure the weapon is rotated correctly
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + flipOffset);
        }

    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            if (left)
            {
                //throwRotation += (float) weaponStats[Constants.PARAM_ROTATION_SPEED];
                throwRotation += weapon.rotationSpeed;
            }
            else
            {
                //throwRotation -= (float) weaponStats[Constants.PARAM_ROTATION_SPEED];
                throwRotation -= weapon.rotationSpeed;
            }
            //Make sure weapon rotates in right direction

            //transform.position = transform.position + throwPosition.normalized * (float)weaponStats[Constants.PARAM_THROW_SPEED] * Time.deltaTime;
            transform.position = transform.position + throwPosition.normalized * weapon.throwSpeed * Time.deltaTime;

            transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, throwRotation); ;
        }
    }

    public void ExecuteThrow()
    {
        throwPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        throwOrigin = transform.position;
        throwPosition.z = 0;
        left = throwPosition.x < transform.position.x;
        throwRotation = transform.rotation.z;
        isThrown = true;
        transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isThrown)
        {

            if (other.CompareTag(Constants.ENEMY_TAG))
            {
                if (!enemiesHurt.Contains(other.transform.gameObject))
                {
                    //other.GetComponent<HitManager>().TakeDamage(Convert.ToInt32(weaponStats[Constants.PARAM_DAMAGE]), throwOrigin, (float)weaponStats[Constants.PARAM_KNOCKBACK]);
                    other.GetComponent<HitManager>().TakeDamage(weapon.damage, throwOrigin, weapon.knockback);
                }
            }
            else if (other.CompareTag("Wall"))
            {
                //TODO instantiate pickup as child of the current room
                GameObject pickup = Instantiate(pickupVersion, RoomSpawner.instance.getCurrentRoom().transform);
                pickup.GetComponent<SpriteRenderer>().sprite = weaponRenderer.sprite;
                pickup.transform.position = transform.position;
                pickup.transform.rotation = transform.GetChild(0).rotation;
                Destroy(gameObject);
            }
        }
    }

}
