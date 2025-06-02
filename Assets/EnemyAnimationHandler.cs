using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    public EnemySkeleton enemySkeleton;  // Reference to your main enemy script

    public void Attack()
    {
        if (enemySkeleton != null)
            enemySkeleton.SpawnAttackCollider();
    }
}
