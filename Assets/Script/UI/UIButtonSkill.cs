using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIButtonSkill : MonoBehaviour {

    public int index = 0;
	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(onClick);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void onClick()
    {
        GameManager.MainPlayer.Skill.CastSkill(index);
    }
}
