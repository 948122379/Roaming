using UnityEngine;
using System.Collections;

public class Mini_Map : MonoBehaviour
{
    public GameObject Mini_Map_Camera;
    public GameObject Mini_Map_Player;

    void Start()
    {
        Mini_Map_Camera = GameObject.Find("Mini_Map_Camera");
        Mini_Map_Player = GameObject.Find("Mini_Map_Player");
    }

    void Update()
    {
        Mini_Map_Player.transform.localPosition = new Vector3(Mini_Map_Camera.transform.localPosition.x, Mini_Map_Camera.transform.localPosition.y - 7, Mini_Map_Camera.transform.localPosition.z);
    }
}
