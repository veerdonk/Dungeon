using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Projectile
{
    public Animator animator;
    public LayerMask layer;

    public override void PlaySoundEffect()
    {
        //Play a sound effect on creation
        AudioManager.instance.PlayRandomOfType(SoundType.FIREBALL);
    }

    protected override void HandleCollision(Collider2D other)
    {
        //Handle collision
        //set animator trigger
        animator.SetTrigger("Explode");
        //Play a sound effect
        AudioManager.instance.PlayOnce(Constants.FIREBALL_EXPLOSION);
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        isFlying = false;

        //Scan a circle to damage enemies in
        Collider2D[] charactersToDamage = Physics2D.OverlapCircleAll(transform.position, weapon.range, layer);
        foreach (Collider2D character in charactersToDamage)
        {
            if(character.gameObject != shooter)
            {
                int damage = weapon.projectileDamage;
                bool isCrit = false;

                if (Util.CheckChance(weapon.critChance))
                {
                     isCrit = true;
                    damage *= 2;
                }

                //Damage the character
                character.GetComponent<HitManager>().TakeDamage(damage, direction, weapon.knockback, isCrit);
            }
        }
    }
}
