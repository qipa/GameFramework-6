using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FightPanel : MonoBehaviour {

    Button _ChangeWeaponBtn = null;
    Button _LoadEnemy = null;
    Button _ChangeScene = null;
	// Use this for initialization
    void Awake()
    {
        _ChangeWeaponBtn = transform.FindChild("ChangeWeaponBtn").GetComponent<Button>();
        if (_ChangeWeaponBtn != null)
            _ChangeWeaponBtn.onClick.AddListener(ChangeWeapon);
        _LoadEnemy = transform.FindChild("LoadEnemy").GetComponent<Button>();
        if (_LoadEnemy != null)
            _LoadEnemy.onClick.AddListener(LoadEnemy);
        _ChangeScene = transform.FindChild("ChangeScene").GetComponent<Button>();
        if (_ChangeScene != null)
            _ChangeScene.onClick.AddListener(ChangeScene);

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

    uint enemyID = 10;
    void LoadEnemy()
    {
        Entity enemy = EntityManager.Instance.Get(11, enemyID++);
        enemy.Pos = GameManager.MainPlayer.Pos + GameManager.MainPlayer.Forward*5;
        enemy.SetAI("Monster");
    }

    void ChangeScene()
    {
        SceneManager.Instance.LoadScene("101");
    }
}
