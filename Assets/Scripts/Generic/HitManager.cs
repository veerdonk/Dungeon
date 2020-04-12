using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitManager : MonoBehaviour
{

    public abstract void TakeDamage(int damage, Vector2 attackerPos, float knockBack);



}
