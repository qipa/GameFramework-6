using UnityEngine;  
using System.Collections;
using System.IO;
using System.Text;
public class GridManager : MonoBehaviour
{
    private static GridManager s_Instance = null;
    public static GridManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GridManager))
                        as GridManager;
                if (s_Instance == null)
                    Debug.Log("找不到GriaManager！！！");
            }
            return s_Instance;
        }
    }

    public int rows;
    public int columns;
    private float GridCellSize = 1f;
    public bool showGrid = true;
    public bool showObstacle = true;
    public bool CheckSlope = false;     //是否检测坡度
    public float maxSlope = 45f;        //坡度最大默认为45度
    public string fileName = "";        //保存路径的文件名
    private Vector3 origin = Vector3.zero;
    public Node[,] nodes = null;
    public Vector3 Origin
    {
        get { return origin; }
    }

    void Awake()
    {
        origin = transform.position;
        InitNodes();
    }
   
    void InitNodes()
    {
        if (nodes == null)
        {
            nodes = new Node[rows, columns];
            Foreach_Node((i,j,node) =>
                {
                    Vector3 cellPos = GetGridCenterPos(i, j);
                    nodes[i,j] = new Node();
                    nodes[i, j].position = cellPos;
                }
                );
        }
    }


    public Vector3 GetGridCenterPos(int row,int col)
    {
        float xPosInGrid = col * GridCellSize;
        float zPosInGrid = row * GridCellSize;
        Vector3 pos = Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
        float offset = GridCellSize * .5f;
        pos.x += offset;
        pos.z += offset;
        return pos;
    }


    public bool GetRowAndColByPos(Vector3 pos,out int row,out int col)
    {
        if (!IsValid(pos))
        {
            row = -1;
            col = -1;
            return false;
        }
        pos -= Origin;
        col = (int)(pos.x / GridCellSize);
        row = (int)(pos.z / GridCellSize);
        return true;
    }

    
    public bool IsValid(Vector3 pos)
    {
        float width = columns * GridCellSize;
        float depth = rows * GridCellSize;
        return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
                pos.z <= Origin.z + depth && pos.z >= Origin.z);
    }

  
    

    void OnDrawGizmos()
    {
        origin = transform.position;
        //绘制格子
        if (showGrid)
        {
            float width = (columns * GridCellSize);
            float depth = (rows * GridCellSize);

            for (int i = 0; i < rows + 1; i++)
            {
                Vector3 startPos = origin + i * GridCellSize * new Vector3(0.0f,0.0f, 1.0f);
                Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f,0.0f);
                Debug.DrawLine(startPos, endPos, Color.blue);
            }
           
            for (int i = 0; i < columns + 1; i++)
            {
                Vector3 startPos = origin + i * GridCellSize * new Vector3(1.0f,0.0f, 0.0f);
                Vector3 endPos = startPos + depth * new Vector3(0.0f, 0.0f,1.0f);
                Debug.DrawLine(startPos, endPos, Color.blue);
            }
        }

        //绘制GridManager
        Gizmos.DrawSphere(transform.position, 0.5f);

        //绘制障碍
        if (showObstacle && nodes != null)
        {
            Foreach_Node((i,j,node)=>
                {
                    if (node.bWalkable == false)
                    {
                        Vector3 pos = node.position;
                        pos.y += GridCellSize / 2;
                        Gizmos.DrawSphere(pos, GridCellSize / 2);
                    }
                }
                );
        }
    }
   
    void OnGUI()
    {
        if(GUILayout.Button("扫描生成障碍"))
        {
            Scan();
        }
        if(GUILayout.Button("保存地图信息"))
        {
            Save();
        }
        if(GUILayout.Button("加载地图信息"))
        {
            Load();
        }
    }


    void Scan()
    {
        Foreach_Node((i, j, node) =>
            {
                CheckWalkable(node);

            });

    }

    void CheckWalkable(Node node)
    {
        node.bWalkable = false;

        RaycastHit hitInfo = new RaycastHit();
        RaycastHit[] hits = Physics.RaycastAll(node.position + Vector3.up * 100, -Vector3.up, 100.005f);
        
        if( (node.position - Vector3.zero ).sqrMagnitude < 0.01f)
        {
            Debug.Log("");
        }

        if (hits.Length > 1)
        {
            float tmp = Mathf.Infinity;
            int iMax = 0;
            for (int index = 0; index < hits.Length; index++)
            {
                if (tmp > hits[index].distance)
                {
                    tmp = hits[index].distance;
                    iMax = index;
                }

                if (hits[index].collider.gameObject.layer == LayerMask.NameToLayer("NoWalk"))
                    node.bWalkable = false;
                else if(hits[index].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    node.bWalkable = true;
                    
            }
            hitInfo.point = hits[iMax].point;
            hitInfo.normal = hits[iMax].normal;
            hitInfo.distance = hits[iMax].distance;

            node.position = hitInfo.point;
        }
        else if(hits.Length == 1)
        {
            hitInfo = hits[0];
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("NoWalk"))
                node.bWalkable = false;
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                node.bWalkable = true;

            node.position = hitInfo.point;
        }
        else
        {
            node.bWalkable = false;
        }


        if (CheckSlope && hitInfo.normal != Vector3.zero)
        {
            //计算坡度的cos值
            float CosSlope = Vector3.Dot(hitInfo.normal.normalized, Vector3.up);

            //计算最大坡度的cos值
            float CosMax = Mathf.Cos(maxSlope * Mathf.Deg2Rad);

            //如果角度大于设定的最大坡度角度
            if (CosSlope < CosMax)
            {
                node.bWalkable = false;
            }
        }
    }
 

    void Save()
    {
        string filePath = Application.dataPath + "/Resources/PathInfo/" + fileName + ".bytes";
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            CStream s = new CStream(fs);
            s.WriteInt(rows);
            s.WriteInt(columns);
            s.WriteFloat(GridCellSize);
            s.WriteVector3(ref origin);

            Foreach_Node((i,j,node)=>
                {
                    s.WriteBool(node.bWalkable);
                    s.WriteVector3(ref node.position);
                }
                );
            s.Close();
        }
        Debug.Log("保存成功 ===> " + filePath);
    }

    void Load()
    {
        string filePath = Application.dataPath + "/Resources/PathInfo/" + fileName + ".bytes";
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            CStream s = new CStream(fs);
            rows = s.ReadInt();
            columns = s.ReadInt();
            s.ReadFloat(ref GridCellSize);
            s.ReadVector3(ref origin);

            nodes = null;
            InitNodes();

            Foreach_Node((i,j,node)=>
                {
                    node.bWalkable = s.ReadBool();
                    s.ReadVector3(ref node.position);
                }
                );
            s.Close();
        }
    }

    void Foreach_Node(System.Action<int,int,Node> action)
    {
        for(int i = 0; i < rows;i++)
        {
            for(int j = 0; j < columns;j++)
            {
                if(action != null)
                    action(i,j,nodes[i,j]);
            }
        }
    }
}