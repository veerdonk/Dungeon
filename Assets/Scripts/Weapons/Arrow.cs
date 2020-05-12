using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    protected override void HandleCollision(Collider2D other)
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

        if (other.CompareTag(Constants.PLAYER_TAG) || other.CompareTag(Constants.ENEMY_TAG))
        {
            if (damage == null)
            {
                damage = weapon.damage;
            }
            bool isCrit = false;
            if (Util.CheckChance(weapon.critChance))
            {
                damage *= 2;
                isCrit = true;
            }
            other.GetComponent<HitManager>().TakeDamage((int)damage, transform.position, weapon.knockback, isCrit);
        }
    }

}
