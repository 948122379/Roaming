using UnityEngine;
using System.Collections;

public class autorotation : MonoBehaviour {
    private string axis_R = "";
	// Use this for initialization
	void Start () {
        float type_i = Random.value * 100;
        if (type_i >= 0 && type_i < 33)
        {
            axis_R = "x";
        }
        else if (type_i >= 34 && type_i < 66)
        {
            axis_R = "y";
        }
        else if (type_i >= 66 && type_i <= 100)
        {
            axis_R = "z";
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (axis_R == "x")
        {
            transform.Rotate(new Vector3( Random.value/2,0.2f, 0));
        }
        else if (axis_R == "y")
        {
            transform.Rotate(new Vector3(0.2f, Random.value/2 ,0));
        }
        else if (axis_R == "z")
        {
            transform.Rotate(new Vector3(Random.value/2, 0,0.2f ));
        }
	}
}
