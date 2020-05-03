using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AbstractAttack
{
    public override void ExecuteAttack()
    {

        if (timeSinceAttack < 0)
        {
            Debug.Log("Attacking");
            WeaponAnimator.SetTrigger(Constants.ENEMY_FIRE_BOW);
            RangedWeapon weap = (RangedWeapon)weapon;
            GameObject arrow = Instantiate(weap.projectile, transform.parent);
            arrow.transform.position = attackPos.position;
            Arrow arrowC = arrow.GetComponent<Arrow>();
            Vector2 heading = PlayerManager.instance.transform.position - attackPos.position;
            float distance = heading.magnitude;
            arrowC.direction = heading / distance;
            arrowC.weapon = weapon;
            arrowC.shooter = gameObject;
            arrowC.damage = (int)(weapon.damage * enemy.damageModifier);
            timeSinceAttack = weapon.delay * enemy.attackSpeedModifier;
        }
    }

}
