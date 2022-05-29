using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviorNodeStauts
{
    None,
    Running,
    Succeed,
    Failed,
    Cancel
}

public class BehaviorNode
{
    private BehaviorNode fatherNode=null;
    private List<BehaviorNode> SonBehaviorNodes=new List<BehaviorNode>();

    public void AddSonBehaviorNode(BehaviorNode fatherNode,BehaviorNode sonNode)
    {
        SonBehaviorNodes.Add(sonNode);
        this.fatherNode = fatherNode;
    }
}
