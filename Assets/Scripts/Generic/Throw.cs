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
    public ParticleSystem DestroyPS;

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
                throwRotation += weapon.rotationSpeed;
            }
            else
            {
                throwRotation -= weapon.rotationSpeed;
            }
            //Make sure weapon rotates in right direction
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
        

        if(weapon.type == WeaponType.BOW)
        {
            RangedWeapon rangedWeapon = (RangedWeapon)weapon;
            if (rangedWeapon.FirePoint == FirePoint.ONTRHOW)
            {
                //Spawn the number of projectiles around the player
                float degOffsetPerProjectile = rangedWeapon.degreesAround/rangedWeapon.numberToSpawn;
                float degOffsetStart = -(rangedWeapon.degreesAround/2);
                for (int i = 0; i < rangedWeapon.numberToSpawn; i++)
                {
                    GameObject projectile = Instantiate(rangedWeapon.projectile);
                    Arrow arrow = projectile.GetComponent<Arrow>();
                    arrow.speed = weapon.throwSpeed +2;
                    arrow.transform.position = transform.parent.position;
                    Vector3 newPos = Quaternion.AngleAxis(degOffsetStart, Vector3.forward) * throwPosition;// throwPosition;
                    degOffsetStart += degOffsetPerProjectile;
                    Vector2 heading = newPos;
                    float distance = heading.magnitude;
                    arrow.direction = heading / distance;
                    arrow.weapon = weapon;
                    arrow.weapon.throwSpeed = weapon.throwSpeed;
                    arrow.shooter = transform.parent.gameObject;

                }
            }
        }

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
                    other.GetComponent<HitManager>().TakeDamage(weapon.damage, throwOrigin, weapon.knockback);
                }
            }
            else if (other.CompareTag("Wall"))
            {
                if (UnityEngine.Random.Range(0, 100) > Constants.CHANCE_WEAPON_BREAKS || !Inventory.instance.hasWeapon())
                {
                    GameObject pickup = Instantiate(pickupVersion, RoomSpawner.instance.getCurrentRoom().transform);
                    pickup.GetComponent<Pickup>().item = weapon;
                    pickup.GetComponent<Pickup>().SetStatic();
                    pickup.transform.position = transform.position;
                    pickup.transform.rotation = transform.GetChild(0).rotation;
                }
                else
                {
                    //Play some particle effect
                    ParticleSystem ps = Instantiate(DestroyPS);
                    ps.transform.position = transform.position;
                }
                Destroy(gameObject);
            }
        }
    }

}
