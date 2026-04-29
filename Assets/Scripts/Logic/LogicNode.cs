using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicNode : MonoBehaviour
{
    public LogicNode NextNode, PreviousNode;
    public bool IsNodeReady;
    public bool IsBuilded = true;
    [HideInInspector] public WorkerNode WorkerNode;

    public void ApplyWorkerNode(WorkerNode worker)
    {
        WorkerNode = worker;
    }
}