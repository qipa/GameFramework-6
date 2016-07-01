using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;
    public float m_dist = 15;
    public float m_pitchAngle = 45;
    public float m_yawAngle = 0;
    public float m_height = 1;

    //相机震动参数
    public float verticalShake;
    public float shakeAmount = 1.7f;
    public static float shakeDis = 1.0f;
    public float decreaseFactor = 1.0f;


    public Entity LookTarget = null;
	// Use this for initialization
    void Awake()
    {
        Instance = this;
    }
	void Start () {
	
	}

    void Update()
    {
//         if (GameManager.MainPlayer != null && InputUtil.has_touch_down())
//         {
//             CheckClickMove();
//        
    }

    //检测点击移动角色
    void CheckClickMove()
    {
        Ray r = Camera.main.ScreenPointToRay(InputUtil.get_touch_pos());
        RaycastHit hitInfo;
        if (!Physics.Raycast(r, out hitInfo, 100f, ~LayerMask.NameToLayer("Ground")))
            return;

        GameManager.MainPlayer.Move.MoveTo(hitInfo.point);
    }


    void LateUpdate()
    {
        if (LookTarget == null)
            return;
#if UNITY_EDITOR
        float scroll_wheel = Input.GetAxis("Mouse ScrollWheel");
        m_dist += scroll_wheel * -6.0f;
        if (m_dist <= 2)
        {
            m_dist = 2;
        }
#endif

        Vector3 targetPos = LookTarget.Pos;
        targetPos.y += m_height;

        Vector3 cameraPos = targetPos;

        float r = m_dist * Mathf.Cos(m_pitchAngle * Mathf.Deg2Rad);

        cameraPos.z += Mathf.Sin((m_yawAngle) * Mathf.Deg2Rad) * r;
        cameraPos.x += Mathf.Cos((m_yawAngle) * Mathf.Deg2Rad) * r;

        cameraPos.y += m_dist * Mathf.Sin(m_pitchAngle * Mathf.Deg2Rad);

        if (null == Camera.main) return;

        Camera.main.transform.position = cameraPos;
        Camera.main.transform.LookAt(LookTarget.Pos);

        if (verticalShake > 0)
        {
            Camera.main.transform.position += Vector3.up * Random.Range(0.0f, 0.5f) * shakeAmount * verticalShake * shakeDis;
            verticalShake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            verticalShake = 0f;
        }
    }

    public void Shake()
    {
        verticalShake = 0.5f;
    }
}
