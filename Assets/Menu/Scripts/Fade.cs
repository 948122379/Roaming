using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fade : MonoBehaviour {
    public float time;
    public float time1;
    static public bool fade;
    public GameObject Sphere;
    public bool begin;
    public GameObject player;
    public GameObject Pos;

    public bool begin1;

    // Use this for initialization
    void Start () {
        time = 0;
        fade = false;
    }
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.M))
        //{
        //    fade = true;
        //}
		if(fade)
        {
            time = Time.time;
            fade = false;
            begin = true;
            //player.GetComponent<NavMeshAgent>().enabled = false;
            //player.transform.position = Pos.transform.position;
            //player.GetComponent<NavMeshAgent>().enabled = true;
        }
        if(begin)
        {
            time1 = Time.time - time;
            if (time1 > 1 && time1 < 3)
            {
                Sphere.GetComponent<Renderer>().material.SetFloat("_AlphaScale", (time1 - 1) / 2);
            }
            else if (time1 > 3 && time1 < 4)
            {
                Sphere.GetComponent<Renderer>().material.SetFloat("_AlphaScale", 1);

            }
            else if (time1 > 4)
            {
                Sphere.GetComponent<Renderer>().material.SetFloat("_AlphaScale", (6 - time1) / 2);
            }

            if(time1 > 3 && time1 < 4)
            {
                begin1 = true;
            }
            else if (time1 > 4)
            {
                begin1 = false;
            }

            if (time1 > 6)
            {
                begin = false;

            }
        }

        if (begin1)
        {
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = Pos.transform.position;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
