using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 direction;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public Weapon weapon;
    public Vector2 rotation;
    private bool isFlying = true;
    public GameObject shooter;

    public int? damage;
    public float? speed;

    private void Start()
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            if(speed == null)
            {
                speed = weapon.throwSpeed;
            }
            rb.MovePosition(rb.position + (Vector2)direction * (int)speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != shooter)
        {
            if (other.CompareTag(Constants.PLAYER_TAG) || other.CompareTag(Constants.ENEMY_TAG) || other.CompareTag(Constants.WALL_TAG))
            {
                transform.position = transform.position + direction * Random.Range(0.3f, 0.45f);
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                bc.enabled = false;
                if (other.CompareTag(Constants.PLAYER_TAG))
                {
                    transform.SetParent(other.transform.Find("Arrows"));
                }
                else
                {
                    transform.SetParent(other.transform);
                }
                isFlying = false;
                if (other.CompareTag(Constants.PLAYER_TAG) || other.CompareTag(Constants.ENEMY_TAG)){
                    if(damage == null)
                    {
                        damage = weapon.damage;
                    }
                    other.GetComponent<HitManager>().TakeDamage((int)damage, transform.position, weapon.knockback);
                }
            }
        }
    }
}
