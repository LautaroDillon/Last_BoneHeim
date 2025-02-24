using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Question_Node : Node
{
    public Node trueNode;
    public Node falseNode;
    public TypeQuest type;

    public override void Execute(EnemisBehaivor enemys)
    {
        switch (type)
        {
            case TypeQuest.LowHP:
                if (enemys.currentlife <= 40)
                    enemys.fsm.ChangeState("Escape");
                else
                    falseNode.Execute(enemys);

                break;

            case TypeQuest.InFOVEnemy:
                if (enemys.FieldOfViewCheck())
                {
                    //enemys.path.Clear();
                    enemys.fsm.ChangeState("Attack");
                }
                else
                    falseNode.Execute(enemys);
                break;

                case TypeQuest.OutFOVEnemy:
                if (!enemys.FieldOfViewCheck())
                {
                    enemys.fsm.ChangeState("Walk");
                }
                else 
                    falseNode.Execute(enemys);
                break;
            case TypeQuest.HasNearbyAllies:
                Debug.Log("Verificando si el enemigo tiene aliados cercanos...");
                if (enemys.enemyType == EnemyType.Healer)
                {
                    Debug.Log("El enemigo es un curandero, verificando aliados cercanos...");
                    if (enemys.HasEnoughNearbyAllies()) // Verifica si hay aliados cercanos
                    {
                        Debug.Log("Aliados cercanos detectados, cambiando al estado de curación.");
                        enemys.fsm.ChangeState("Heal");
                    }
                    else
                    {
                        Debug.Log("No hay aliados cercanos, ejecutando el nodo falso.");
                        falseNode.Execute(enemys);
                    }
                }
                else
                {
                    Debug.LogWarning("Este enemigo no es un curandero, ignorando HasNearbyAllies.");
                    falseNode.Execute(enemys);
                }
                break;
        }
    }
}

public enum TypeQuest
{
    LowHP,
    InFOVEnemy,
    OutFOVEnemy,
    HasNearbyAllies,
}

