using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook_Fly : MonoBehaviour {

    public GameObject Hook;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!PlayerCtrl.isShooting_Hook)
        {
            
        }
        else
        {
            Hook.transform.parent = null;
            Hook.transform.Translate(-0.03f, 0, 0);
        }
	}
}
