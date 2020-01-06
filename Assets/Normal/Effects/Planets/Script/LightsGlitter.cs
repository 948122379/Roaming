using UnityEngine;
using System.Collections;

public class LightsGlitter : MonoBehaviour {
    private bool isadd=true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        //GetComponent<SphereCollider>().radius=GetComponent<Light>().range;
        if (isadd == true)
        {
            GetComponent<Light>().range += Time.deltaTime/2;
            if (GetComponent<Light>().range > 1.2)
            {
                isadd = false;
            }
        }
        else if (isadd == false)
        {
            GetComponent<Light>().range -= Time.deltaTime/2;
            if (GetComponent<Light>().range < 0.8)
            {
                isadd = true;
            }
        }
    }
}
