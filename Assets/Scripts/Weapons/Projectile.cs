using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{

    public Vector3 direction;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public RangedWeapon weapon;
    public Vector2 rotation;
    protected bool isFlying = true;
    public GameObject shooter;
    public GameObject ignore;

    public int? damage;
    public float? speed;

    private void Start()
    {
        PlaySoundEffect();
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        GetComponent<SpriteRenderer>().sprite = weapon.projectileSprite;
        
    }

    abstract public void PlaySoundEffect();


    void FixedUpdate()
    {
        if (isFlying)
        {
            if (speed == null)
            {
                speed = weapon.throwSpeed;
            }
            rb.MovePosition(rb.position + (Vector2)direction * (int)speed * Time.deltaTime);
        }
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != shooter && other.gameObject != ignore)
        {
            if (other.CompareTag(Constants.PLAYER_TAG) || other.CompareTag(Constants.ENEMY_TAG) || other.CompareTag(Constants.WALL_TAG))
            {
                HandleCollision(other);
            }
        }
    }

    protected abstract void HandleCollision(Collider2D other);
}
