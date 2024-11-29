using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSword.asset", menuName = "Weapons/NewSword", order = 51)]
public class NewSword : Weapon
{
    public override Collider2D[] NormalAttack(LayerMask targetLayer, Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, attackRange, targetLayer);
        return hits;
    }
}
