using System;
using Pathfinding;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    private void Start()
    {
        tree = tree.Clone();
        tree.Bind(GetComponent<Unit>());
    }

    private void Update()
    {
        tree.Update();
    }
}
