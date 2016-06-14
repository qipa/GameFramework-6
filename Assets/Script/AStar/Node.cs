using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable
{
    public float G; //实际代价
    public float H; //估计代价
    public bool bWalkable;  //是否可走
    public Node parent;
    public Vector3 position;

    public Node()
    {
        G = 1.0f;
        H = 0f;
        bWalkable = true;
        parent = null;
    }

    public Node(Vector3 pos)
    {
        G = 1.0f;
        H = 0f;
        bWalkable = true;
        parent = null;
        position = pos;
    }


    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        if (G < node.G)
            return -1;
        if (G > node.G)
            return 1;
        return 0;
    }
}
