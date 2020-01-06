using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//加上落到楼梯按钮上
public class Food : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Terrian" || other.gameObject.tag == "Mechanices" || other.gameObject.tag == "Water")
        {
            PlayerCtrl.Throw_Out = false;
        }

        if (other.gameObject.tag == "Mechanices")
        {
            this.gameObject.GetComponent<Rigidbody>().useGravity = true ;
        }
    }
}
