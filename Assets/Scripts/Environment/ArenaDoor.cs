using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaDoor : Doors
{
    public List<EnemisBehaivor> enemies;

    protected override void Update()
    {
        base.Update();
        if(openDoor == false)
        EnemyCheck();
    }

    public override void Activate()
    {
        base.Activate();
    }


    private void EnemyCheck()
    {
        if(enemies.Count <= 0)
        {
            Activate();
        }

        foreach(EnemisBehaivor elem in enemies)
        {
            if(elem == null)
            {
                enemies.Remove(elem);
                return;
            }
        }
    }

}
