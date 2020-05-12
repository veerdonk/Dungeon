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

    bool hasTriggered = false;
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
        transform.localScale = transform.localScale * weapon.sizeModifier;
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

            //.GetChild(0)
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
        

        if(Constants.rangedWeapons.Contains(weapon.type))
        {
            //cast to ranged weapon
            RangedWeapon rangedWeapon = (RangedWeapon)weapon;
            if (rangedWeapon.FirePoint == FirePoint.ONTRHOW)
            {
                SpawnProjectiles(rangedWeapon, transform.parent.position, null);
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
                    int damageTodo = weapon.damage;
                    bool isCrit = false;
                    if (Util.CheckChance(weapon.critChance))
                    {
                        damageTodo *= 2;
                        isCrit = true;
                    }
                    other.GetComponent<HitManager>().TakeDamage(damageTodo, throwOrigin, weapon.knockback, isCrit);
                }

                if (Constants.rangedWeapons.Contains(weapon.type))
                {
                    //Check if we need to perform any on hit tasks
                    RangedWeapon rw = (RangedWeapon)weapon;
                    if(rw.FirePoint == FirePoint.ONHIT && !hasTriggered)
                    {
                        SpawnProjectiles(rw, other.transform.position, other.gameObject);
                        hasTriggered = true;
                    }
                }

                //Check whether weapon should get stuck/fall
                float sharpnessCheck = UnityEngine.Random.Range(0f, 100f);
                if(weapon.sharpness < sharpnessCheck)
                {
                    //Make weapon stuck or fall based on sharpness
                    if (Constants.sharpWeapons.Contains(weapon.type))
                    {
                        //Get weapon stuck
                        GameObject pickup = Instantiate(pickupVersion, RoomSpawner.instance.getCurrentRoom().transform);
                        pickup.GetComponent<Pickup>().item = weapon;
                        pickup.GetComponent<Pickup>().SetStatic(true);
                        pickup.transform.position = transform.position;
                        pickup.transform.parent = other.transform;
                        pickup.transform.rotation = transform.GetChild(0).rotation;

                        if (UnityEngine.Random.Range(0, 100) > weapon.chanceToBreak)
                        {
                            if (other.GetComponent<EnemyManager>().pickupsToDrop.ContainsKey(weapon.rarity))
                            {
                                other.GetComponent<EnemyManager>().pickupsToDrop[weapon.rarity].Add(weapon);
                            }
                            else
                            {
                                other.GetComponent<EnemyManager>().pickupsToDrop[weapon.rarity] = new List<Item> { weapon };
                            }
                        }
                        //Play slice sound
                        AudioManager.instance.PlayRandomOfType(SoundType.METAL_HIT);
                    }
                    else
                    {
                        //Play thud sound
                        AudioManager.instance.PlayOnce(Constants.WEAPON_THUD);

                        //Bounce weapon and spawn drop
                        GameObject pickup = Instantiate(pickupVersion, RoomSpawner.instance.getCurrentRoom().transform);
                        pickup.GetComponent<Pickup>().item = weapon;
                        pickup.transform.position = transform.position;
                        
                    }
                    Destroy(gameObject);

                }
                else
                {
                    //Play weapon hit sound
                    AudioManager.instance.PlayRandomOfType(SoundType.METAL_HIT);
                }

            }
            else if (other.CompareTag("Wall"))
            {
                if (UnityEngine.Random.Range(0, 100) > weapon.chanceToBreak || !Inventory.instance.hasWeapon())
                {
                    GameObject pickup = Instantiate(pickupVersion, RoomSpawner.instance.getCurrentRoom().transform);
                    pickup.GetComponent<Pickup>().item = weapon;
                    //TODO make static again
                    pickup.GetComponent<Pickup>().SetStatic(true);
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


    private void SpawnProjectiles(RangedWeapon rangedWeapon, Vector3 position, GameObject ignore)
    {
        //Spawn the number of projectiles around the player
        float degOffsetPerProjectile = rangedWeapon.degreesAround / rangedWeapon.numberToSpawn;
        float degOffsetStart = -(rangedWeapon.degreesAround / 2);
        for (int i = 0; i < rangedWeapon.numberToSpawn; i++)
        {
            //Spawn projectile
            GameObject projectileObject = Instantiate(rangedWeapon.projectile);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.speed = rangedWeapon.throwSpeed + 2;
            projectile.transform.position = position;
            Vector3 newPos = Quaternion.AngleAxis(degOffsetStart, Vector3.forward) * throwPosition;
            degOffsetStart += degOffsetPerProjectile;
            Vector2 heading = newPos;
            float distance = heading.magnitude;
            projectile.damage = rangedWeapon.projectileDamage;
            projectile.direction = heading / distance;
            projectile.weapon = rangedWeapon;
            projectile.weapon.throwSpeed = rangedWeapon.throwSpeed;
            projectile.shooter = PlayerManager.instance.gameObject;
            projectile.ignore = ignore;
        }
    }

}
