using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparer<Node>
{
    public float G; //实际代价，起点到当前点的代价
    public float H; //估计代价，当前点到终点的代价

    public bool bWalkable;  //是否可走
    public Node parent;
    public Vector3 position;
    public List<Node> neighborList = null;
    public Node()
    {
        G = 0f;
        H = 0f;
        bWalkable = true;
        parent = null;
        neighborList = new List<Node>();
    }


    public int Compare(Node x, Node y)
    {
        return x.G.CompareTo(y.G);
    }
}
