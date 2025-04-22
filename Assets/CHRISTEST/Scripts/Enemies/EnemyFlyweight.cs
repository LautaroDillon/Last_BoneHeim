using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyweight
{
    public static readonly EnemysFlight Shooter = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 90
    };

    public static readonly EnemysFlight Boomer = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 100
    };

    public static readonly EnemysFlight Pawn = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 120
    };
}
