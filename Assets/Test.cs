using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {

    Dictionary<int, string> dic = new Dictionary<int, string>();
	// Use this for initialization
	void Start () {
	    for(int i = 0; i < 100; i++)
        {
            dic.Add(i, (i + 1000).ToString());
        }
	}
	
	void OnGUI()
    {       

        if(GUI.Button(new Rect(0,50,100,20),"打印"))
        {
            var e1 = dic.GetEnumerator();
            while(e1.MoveNext())
            {
                Log.Info(e1.Current.Key + " : " + e1.Current.Value);
            }
        }
    }
}
