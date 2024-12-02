using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyweightPointer
{
    public static readonly EnemysFlight Eshoot = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 90
    };

    public static readonly EnemysFlight Eboomer = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 100
    };

    public static readonly EnemysFlight Ehealer = new EnemysFlight()
    {
        speed = 2,
        Damage = 20,
        maxLife = 120
    };

    public static readonly EnemysFlight Player = new EnemysFlight()
    {
        Damage = 30,
        maxLife = 100
    };

    public static readonly Flywigthorgans organs = new Flywigthorgans()
    {
        heart = 50,
        pulmon = 40,
        higado = 45,
        Heart = "Heart",
        Higado = "Higado",
        Pulmon = "Pulmon"
    };
}
