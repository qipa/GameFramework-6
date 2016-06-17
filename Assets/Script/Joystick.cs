using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform handle;
    private Vector2 autoReturnSpeed = new Vector2(8.0f,8.0f);
    public float radius = 40.0f;

    public Action<Vector2> OnStartJoystickMovement;
    public Action<Vector2> OnJoystickMovement;
    public Action OnEndJoystickMovement;

    private RectTransform _canvas;
    bool _isDraging = false;
    bool _isReturn = false;
    Vector2 _handleOffset;
    public Vector2 Coordinates
    {
        get
        {
            if (handle.anchoredPosition.magnitude < radius)
                return handle.anchoredPosition / radius;
            return handle.anchoredPosition.normalized;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDraging = true;
        GetJoystickOffset(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDraging = true;
         GetJoystickOffset(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDraging = false;
        if (OnEndJoystickMovement != null)
            OnEndJoystickMovement();
    }

    private void GetJoystickOffset(PointerEventData eventData)
    {
        Vector3 globalHandle;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas, eventData.position, eventData.pressEventCamera, out globalHandle))
            handle.position = globalHandle;
        _handleOffset = handle.anchoredPosition;
        if (_handleOffset.magnitude > radius)
        {
            _handleOffset = _handleOffset.normalized * radius;
            handle.anchoredPosition = _handleOffset;
        }
    }

    private void Start()
    {
        _canvas = GetComponent<RectTransform>();
        RegistTouch();
    }

    private void Update()
    {
        int keyDownCount = 0;
        if (Input.GetKey(KeyCode.W))
        {
            keyDownCount++;
            handle.anchoredPosition += new Vector2(0, radius);
            if(handle.anchoredPosition.sqrMagnitude > radius * radius)
            {
                handle.anchoredPosition = handle.anchoredPosition.normalized * radius;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            keyDownCount++;
            handle.anchoredPosition += new Vector2(-radius, 0);
            if (handle.anchoredPosition.sqrMagnitude > radius * radius)
            {
                handle.anchoredPosition = handle.anchoredPosition.normalized * radius;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyDownCount++;
            handle.anchoredPosition += new Vector2(0, -radius);
            if (handle.anchoredPosition.sqrMagnitude > radius * radius)
            {
                handle.anchoredPosition = handle.anchoredPosition.normalized * radius;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyDownCount++;
            handle.anchoredPosition += new Vector2(radius, 0);
            if (handle.anchoredPosition.sqrMagnitude > radius * radius)
            {
                handle.anchoredPosition = handle.anchoredPosition.normalized * radius;
            }
        }

        //键盘模式
       if(keyDownCount > 0)
       {
           if (OnJoystickMovement != null)
               OnJoystickMovement(Coordinates);
           _isReturn = false;
       }
       else if (!_isReturn)
       {
           if (handle.anchoredPosition.magnitude > Mathf.Epsilon)
               handle.anchoredPosition -= new Vector2(handle.anchoredPosition.x * autoReturnSpeed.x, handle.anchoredPosition.y * autoReturnSpeed.y) * Time.deltaTime;
           else
               _isReturn = true;
       }

          
        //摇杆模式
        if (_isDraging)
        {
                handle.anchoredPosition = _handleOffset;
            if (OnJoystickMovement != null)
                OnJoystickMovement(Coordinates);
            _isReturn = false;
        }
        else if(!_isReturn)
        {
            if (handle.anchoredPosition.magnitude > Mathf.Epsilon)
                handle.anchoredPosition -= new Vector2(handle.anchoredPosition.x * autoReturnSpeed.x, handle.anchoredPosition.y * autoReturnSpeed.y) * Time.deltaTime;
            else
                _isReturn = true;
        }

        
    }


    void RegistTouch()
    {
        OnStartJoystickMovement = OnTouchBegin;
        OnJoystickMovement = OnTouchMove;
        OnEndJoystickMovement = OnTouchEnd;
    }

    public void OnTouchBegin(Vector2 move)
    {
        if (GameManager.MainPlayer == null)
            return;
    }

    float lastTime = 0f;
    public void OnTouchMove(Vector2 move)
    {
        if (GameManager.MainPlayer == null)
            return;
        if (Time.time - lastTime < 0.2f)
            return;

        Vector3 dir = new Vector3(move.x, 0, move.y).normalized;
        Quaternion q = Camera.main.transform.rotation;

        dir = q * dir;
        dir *= 3f;      //摇杆控制的角色行走距离为3

        Vector3 targetPos = new Vector3(GameManager.MainPlayer.Pos.x + dir.x, GameManager.MainPlayer.Pos.y, GameManager.MainPlayer.Pos.z + dir.z);
        GameManager.MainPlayer.Move.MoveTo(targetPos);
        lastTime = Time.time;
    }

    public void OnTouchEnd()
    {
        if (GameManager.MainPlayer == null)
            return;

        GameManager.MainPlayer.Move.StopMove();
    }


}