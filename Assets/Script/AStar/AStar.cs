using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    static int rows;
    static int cols;
    static float GridCellSize;
    static Node[,] nodes = null;
    public static void LoadPathInfo(string path)
    {
        if (nodes != null)
            nodes = null;

        TextAsset asset = ResManager.Instance.Load(path) as TextAsset;
        if(asset == null)
        {
            CStream s = new CStream(asset.bytes);
            rows = s.ReadInt();
            cols = s.ReadInt();
            s.ReadFloat(ref GridCellSize);

            for(int i =0; i < rows;i++)
            {
                for(int j = 0; j < cols;j++)
                {
                    Node node = new Node();
                    node.bWalkable = s.ReadBool();
                    s.ReadVector3(ref node.position);

                    nodes[i, j] = node;
                }
            }
            s.Close();
            
        }
    }

    public static void Clear()
    {
        nodes = null;
    }
}