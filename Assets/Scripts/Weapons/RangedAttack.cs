using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AbstractAttack
{
    public override void ExecuteAttack()
    {

        if (timeSinceAttack < 0)
        {
            //Set animation trigger
            WeaponAnimator.SetTrigger(Constants.ENEMY_FIRE_BOW);
            RangedWeapon weap = (RangedWeapon)weapon;
            //Instantiate projectile
            GameObject projectile = Instantiate(weap.projectile, transform.parent);
            projectile.transform.position = attackPos.position;
            Arrow arrowC = projectile.GetComponent<Arrow>();
            Vector2 heading = PlayerManager.instance.transform.position - attackPos.position;
            float distance = heading.magnitude;
            arrowC.direction = heading / distance;
            arrowC.weapon = weap;
            arrowC.shooter = gameObject;
            arrowC.damage = (int)(weapon.damage * enemy.damageModifier);
            timeSinceAttack = weapon.delay * enemy.attackSpeedModifier;
        }
    }

}
