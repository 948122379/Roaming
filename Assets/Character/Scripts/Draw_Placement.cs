using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Placement : MonoBehaviour {

    static  public bool ReadytoThrow;
    public bool aaa;
    public float time;

    public float Throw_speed;

    // Use this for initialization
    void Start () {
        time = Time.time;
        Throw_speed = Time.time + 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(0, (Throw_speed - Time.time) * PlayerCtrl.Throw_speed_y, PlayerCtrl.Throw_speed_x);
        if (Time.time - time > 5)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrian")
        {
            if (Vector3.Distance(PlayerCtrl.Placement_Pos, this.gameObject.transform.position) > 0.1f)
            {
                PlayerCtrl.Placement_Pos = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.25f, this.gameObject.transform.position.z);
            }
            Destroy(this.gameObject);
        }
    }
}
