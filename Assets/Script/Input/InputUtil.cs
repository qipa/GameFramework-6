using UnityEngine;
using System.Collections;


public class InputUtil
{
    
    static public bool has_touch_down()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return Input.GetMouseButtonDown(0);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Began;
            }
            else
            {
                return false;
            }
        }
    }

    static public bool has_touch_up()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return Input.GetMouseButtonUp(0);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Ended;
            }
            else
            {
                return false;
            }
        }
    }

    static public int get_touch_count()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return (Input.GetMouseButton(0) ? 1 : 0);
        }
        else
        {
            return Input.touchCount;
        }
    }

    static public Vector3 get_touch_pos()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return Input.mousePosition;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    static public Vector3 mousePosition
    {
        get { return get_touch_pos(); }
    }
}
