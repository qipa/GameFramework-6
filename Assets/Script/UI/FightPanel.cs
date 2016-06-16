using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FightPanel : MonoBehaviour {

    Button _ChangeWeaponBtn = null;
	// Use this for initialization
    void Awake()
    {
        _ChangeWeaponBtn = transform.FindChild("ChangeWeaponBtn").GetComponent<Button>();
        if (_ChangeWeaponBtn != null)
            _ChangeWeaponBtn.onClick.AddListener(ChangeWeapon);
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ChangeWeapon()
    {
        if(GameManager.MainPlayer != null)
            GameManager.MainPlayer.Render.RandomChangeWeapon();
    }
}
