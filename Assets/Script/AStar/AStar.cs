using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class AStar
{
    static int rows;  //地图格子总行数
    static int cols;    //地图格子总列数
    static float GridCellSize;  //一个格子的大小
    static Vector3 origin;      //格子地图坐标原点

    static Node[,] nodes = null;
    public static List<Vector3> m_pathList = new List<Vector3>();
    static HashSet<Node> m_OpenSet = new HashSet<Node>();
    static HashSet<Node> m_CloseSet = new HashSet<Node>();
    static PriorityQueue<float,Node> m_BinaryHeap = new PriorityQueue<float,Node>();
    public static int MaxNodes = 1024*1024;
    public static bool bHasDynamicObstacle = false;      //是否存在动态障碍

    public static void LoadPathInfo(string path)
    {
        if (nodes != null)
            nodes = null;

        TextAsset asset = ResManager.Load<TextAsset>(path, ".bytes", "PathInfo/PathInfo.unity3d");
        if(asset != null)
        {
            CStream s = new CStream(asset.bytes);
            rows = s.ReadInt();
            cols = s.ReadInt();

            nodes = new Node[rows, cols];

            s.ReadFloat(ref GridCellSize);
            s.ReadVector3(ref origin);

            for(int i =0; i < rows;i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Node node = new Node();
                    node.bWalkable = s.ReadBool();
                    s.ReadVector3(ref node.position);
                    nodes[i, j] = node;
                }
            }

            if (!bHasDynamicObstacle)
            {
                //预先计算好每个节点的邻居，以免在寻路的时候进行计算，提高效率
                InitNeighbors();
            }
            s.Close();            
        }
        else
        {
            Debug.LogError("请检查地图信息是否存在: " + path);
        }
    }

    static void InitNeighbors()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Node node = nodes[i, j];
                if (!node.bWalkable)
                    continue;

                for (int m = -1; m <= 1; m++)
                {                    
                    for (int n = -1; n <= 1; n++)
                    {
                        if (m == 0 && n == 0)   //考察周围八个方向，但不考察自己
                            continue;

                        int SearchRow = i + m;
                        int SearchCol = j + n;
                        if (SearchRow >= 0 && SearchRow < rows && SearchCol >= 0 && SearchCol < cols)
                        {
                            Node SearchNode = nodes[SearchRow, SearchCol];
                            if (SearchNode.bWalkable)
                                node.neighborList.Add(SearchNode);
                        }
                    }
                }
            }
        }
    }

    public static void Clear()
    {
        nodes = null;
    }
    public static bool IsValid(Vector3 pos)
    {
        float width = cols * GridCellSize;
        float depth = rows * GridCellSize;
        return (pos.x >= origin.x && pos.x <= origin.x + width &&
                pos.z <= origin.z + depth && pos.z >= origin.z);
    }
    public static Vector3 GetGridCenterPos(int row, int col)
    {
        float xPosInGrid = col * GridCellSize;
        float zPosInGrid = row * GridCellSize;
        Vector3 pos = origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
        float offset = GridCellSize * .5f;
        pos.x += offset;
        pos.z += offset;
        return pos;
    }


    public static bool GetRowAndColByPos(Vector3 pos, out int row, out int col)
    {
        if (!IsValid(pos))
        {
            row = -1;
            col = -1;
            return false;
        }
        pos -= origin;
        col = (int)(pos.x / GridCellSize);
        row = (int)(pos.z / GridCellSize);
        return true;
    }

    public static List<Vector3> FindPath(Vector3 start,Vector3 end)
    {
        m_OpenSet.Clear();
        m_CloseSet.Clear();
        m_BinaryHeap.Clear();

        int startRow,startCol,endRow,endCol;
        if (!GetRowAndColByPos(start, out startRow, out startCol))
        {
            Debug.LogError("寻路起点不合法");
            return null;
        }
        if (!GetRowAndColByPos(end, out endRow, out endCol))
        {
            Debug.LogError("寻路终点不合法");
            return null;
        }

        //注意，这里将起点、终点交换，因为找到路径的时候需要根据终点向前回溯得到路径，而回溯的路径是反的
        Node StartNode = nodes[endRow, endCol];
        Node EndNode = nodes[startRow, startCol];

        StartNode.G = 0;
        StartNode.H = ManhattanDistance(StartNode, EndNode);

        m_OpenSet.Add(StartNode);
        m_BinaryHeap.Push(StartNode.G,StartNode);

        int nodeCount = 0;
        while(m_OpenSet.Count > 0)
        {
            Node curNode = m_BinaryHeap.Pop();
            
            if(curNode == EndNode)      //找到终点
            {
                GetPath(StartNode, EndNode);
                return m_pathList;
            }
            m_OpenSet.Remove(curNode);
            m_CloseSet.Add(curNode);

            List<Node> neighbors = GetNeighborList(curNode);
            if (neighbors != null)
            {
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Node SearchNode = neighbors[i];
                    if (SearchNode == null)
                        continue;
                    if (m_CloseSet.Contains(SearchNode))
                        continue;

                    nodeCount++;
                    float g = curNode.G;
                    if (curNode.parent == null)
                        g += ManhattanDistance(curNode, SearchNode);
                    else
                        g += ManhattanDistancePenalty(curNode.parent, curNode, SearchNode);

                    float h = ManhattanDistance(SearchNode, EndNode);

                    if(!m_OpenSet.Contains(SearchNode))
                    {                       
                        SearchNode.G = g;
                        SearchNode.H = h;
                        SearchNode.parent = curNode;
                        m_OpenSet.Add(SearchNode);
                        m_BinaryHeap.Push(SearchNode.G, SearchNode);
                    }
                    else if(g < SearchNode.G)
                    {
                        SearchNode.G = g;
                        SearchNode.H = h;
                        SearchNode.parent = curNode;
                    }
                }
            }
            
            if (nodeCount > MaxNodes)
            {
                Debug.LogError("搜寻的节点太多了，地图做的太大了吧？！");
                break;
            }
        }

        return null;
    }

    static int ManhattanDistance(Node n1,Node n2)
    {
        return (int)(Math.Abs(n1.position.x - n2.position.x) + Math.Abs(n1.position.z - n2.position.z));
    }

    static int ManhattanDistancePenalty(Node inPre, Node inStart, Node inEnd)
    {
        float diffX = inStart.position.x - inEnd.position.x;
        float diffY = inStart.position.z - inEnd.position.z;

        float diffXPre = inPre.position.x - inStart.position.x;
        float diffYPre = inPre.position.z - inStart.position.z;

        diffX = Math.Abs(diffX) + Math.Abs(diffXPre - diffX);
        diffY = Math.Abs(diffY) + Math.Abs(diffYPre - diffY);

        return (int)(diffX + diffY);
    }

    static List<Node> GetNeighborList(Node node)
    {
        if (!bHasDynamicObstacle)
            return node.neighborList;

        int row, col;
        GetRowAndColByPos(node.position, out row, out col);

        for (int m = -1; m <= 1; m++)
        {
            for (int n = -1; n <= 1; n++)
            {
                if (m == 0 && n == 0)   //考察周围八个方向，但不考察自己
                    continue;

                int SearchRow = row + m;
                int SearchCol = col + n;
                if (SearchRow >= 0 && SearchRow < rows && SearchCol >= 0 && SearchCol < cols)
                {
                    Node SearchNode = nodes[SearchRow, SearchCol];
                    if (SearchNode.bWalkable)
                        node.neighborList.Add(SearchNode);
                }
            }
        }

        return node.neighborList;
    }

    static List<Vector3> GetPath(Node start,Node end)
    {
        m_pathList.Clear();
        Node cur = end;
        while(cur != start)
        {
            m_pathList.Add(cur.position);
            cur = cur.parent;
        }
        return m_pathList;
    }
}