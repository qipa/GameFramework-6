using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIButtonSkill : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {

    public int index = 0;
	// Use this for initialization
    bool isPress = false;
    SkillBase skill = null;
    Text txt = null;
	void Start () {
        GetComponent<Button>().onClick.AddListener(onClick);
        txt = transform.FindChild("Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
        if(isPress && index == 0)
        {
            GameManager.MainPlayer.Skill.CastSkill(index);
        }
        if (skill == null)
            return;

        if(skill.m_fCDLeftTime > 0)
        {
            txt.text = ((int)skill.m_fCDLeftTime+1).ToString();
        }
        else
        {
            txt.text = skill.m_skillInfo.name;
        }
	}

    void onClick()
    {
        if (skill == null && index > 0)
            skill = GameManager.MainPlayer.Skill.GetSkillByIndex(index);

        if(GameManager.MainPlayer.Skill.CastSkill(index))
        {
            
        }
        
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
