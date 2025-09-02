using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static INode;

public class ConditionNode : INode
{
    private Func<bool> condition;
    private INode child;

    public ConditionNode(Func<bool> condition, INode child)
    {
        this.condition = condition;
        this.child = child;
    }

    public ENodeState Evaluate()
    {
        if (condition())
            return child.Evaluate();
        return ENodeState.Failure;
    }
}
