using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTreeNode : MonoBehaviour
{
    private System.Func<bool> condition;
    private System.Action action;
    private DecisionTreeNode trueNode;
    private DecisionTreeNode falseNode;

    public DecisionTreeNode(System.Func<bool> condition, System.Action action = null)
    {
        this.condition = condition;
        this.action = action;
    }

    public void SetTrueNode(DecisionTreeNode node) => trueNode = node;
    public void SetFalseNode(DecisionTreeNode node) => falseNode = node;

    public void Evaluate()
    {
        if (condition.Invoke())
        {
            action?.Invoke();
        }
        else
        {
            falseNode?.Evaluate();
        }
    }
}
