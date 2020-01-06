using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Fly : MonoBehaviour {
    //小鸭子的Perfeb
    public GameObject LittleDuck;

    public float start_time;
    public float time;
    
    //public GameObject Camera;
    public Vector3 Start_Pos;
    public Vector3 End_Pos;
    void Start () {
        start_time = Time.time;
        //Camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;
        Start_Pos = this.transform.position;
        End_Pos = new Vector3(50, 0, 10);
    }

    void Update()
    {
        time = Time.time;
        //if()
        //this.transform.position = Vector3.Lerp(Start_Pos, new Vector3(50,0,10), 0.1f);
        transform.Translate(0, 0.01f, 0.3f);
        if (time > start_time + 5)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FlyDucka")
        {
            if (GameObject.FindGameObjectsWithTag("Ducka").Length + GameObject.FindGameObjectsWithTag("DuckWaterEat").Length < 20)
            {
                GameObject Duck = Instantiate(LittleDuck, transform.position, LittleDuck.transform.rotation);
            }
        }
        Destroy(this.gameObject);
    }
}
