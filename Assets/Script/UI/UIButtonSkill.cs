using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIButtonSkill : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {

    public int index = 0;
	// Use this for initialization
    bool isPress = false;
	void Start () {
        GetComponent<Button>().onClick.AddListener(onClick);
        
	}
	
	// Update is called once per frame
	void Update () {
	
        if(isPress && index == 0)
        {
            GameManager.MainPlayer.Skill.CastSkill(index);
        }
	}

    void onClick()
    {
        GameManager.MainPlayer.Skill.CastSkill(index);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPress = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPress = false;
    }
}
