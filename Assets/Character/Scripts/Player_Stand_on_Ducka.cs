using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stand_on_Ducka : MonoBehaviour {

    static public bool isStandingOnDucka;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ducka_water")
        {
            isStandingOnDucka = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ducka_water")
        {
            isStandingOnDucka = false;
        }
    }
}
