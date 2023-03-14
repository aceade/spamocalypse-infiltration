using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    
    void Damage(GameTagManager.AttackMode attackMode, int damage);

    void Damage(GameTagManager.AttackMode attackMode, int damage, Vector3 attackDirection);
}
