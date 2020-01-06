using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StairsSwitch : MonoBehaviour {
    //对应的台阶
    private GameObject Ladder;
    private Animation Ladder_Animation;
    private Transform openPartical;

    private bool IsNeedChange = false;
    private bool IsNeedUpMove = false;
    private bool IsChangeOver = false;

    private Transform Player;
    private NavMeshAgent Player_agent;

    private bool IsBeforeOnOffMeshLink = true;
    public bool IsOnSwitch = false;

    private Transform Stair_BeMatch;
    private Vector3 Stair_OffsetPosition;
    private Vector3 Switch_OffsetPosition;
    private Vector3 Stair_MoveHeight = new Vector3(0, -0.3f, 0);
    private Vector3 Switch_MoveHeight = new Vector3(0, -0.3f, 0); 
	// Use this for initialization
    void Start()
    {
        //获取机关名字中的字符串
        string switchNum = System.Text.RegularExpressions.Regex.Replace(transform.name, @"[^0-9]+", "");
        //print(switchNum);
        switch (switchNum)
        {
            case "1":
                Ladder = GameObject.Find("tower_ladder_1");
                break;
            case "2":
                Ladder = GameObject.Find("tower_ladder_2");
                break;
            case "3":
                Ladder = GameObject.Find("tower_ladder_3");
                break;
        }
        if (!Ladder.GetComponent<Animation>()) Ladder.AddComponent<Animation>();
        Ladder_Animation = Ladder.GetComponent<Animation>();

        openPartical = transform.Find("SwitchParticle");

        //两行调试
        //Ladder_Animation.Play();
        //IsOnSwitch = true;

        /*
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Player_agent = Player.GetComponent<NavMeshAgent>();

        GameObject[] Stairs = GameObject.FindGameObjectsWithTag("Stairs");
        foreach (GameObject Stair in Stairs)
        {
            if (Stair.name == transform.name.Substring(3, 1))
            {
                Stair_BeMatch = Stair.transform;
                Stair_OffsetPosition = Stair.transform.position;
            }
        }
        */
        Switch_OffsetPosition = transform.position;
    }
	
	
	// Update is called once per frame
    void Update()
    {
        //根据玩家状态判断开关是否被打开，将被删掉
        /*if (Player_agent.isOnOffMeshLink == false)
        {
            IsBeforeOnOffMeshLink = true;
        }
        if (IsBeforeOnOffMeshLink == true)
        {
            if (Player_agent.isOnOffMeshLink == true)
            { 
                IsBeforeOnOffMeshLink = false;
                IsOnSwitch = !IsOnSwitch;
            }
        }*/

        //按下了按钮
        if (IsOnSwitch == true)
        {
            transform.position = Vector3.Lerp(transform.position, Switch_OffsetPosition + Switch_MoveHeight, Time.deltaTime);
            if (Vector3.Distance(transform.position , (Switch_OffsetPosition + Switch_MoveHeight)) < 0.05f)
            {
                openPartical.GetComponent<ParticleSystem>().Play();
            }
            //Ladder_Animation.Play();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Switch_OffsetPosition, Time.deltaTime);
            if (openPartical.GetComponent<ParticleSystem>().isPlaying)
            {
                openPartical.GetComponent<ParticleSystem>().Stop();
                openPartical.GetComponent<ParticleSystem>().Clear();
            }
            
            //Ladder_Animation.Rewind();
        }

        //按钮按到底，梯子下来
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Player" || (other.gameObject.tag.Length >= "Food".Length && other.gameObject.tag.Substring(0, "Food".Length) == "Food"))
        {
            print("碰撞开关");
            IsOnSwitch = true;
            Ladder_Animation["Take 001"].speed = 1f;
            Ladder_Animation.Play();
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Player" || (other.gameObject.tag.Length >= "Food".Length && other.gameObject.tag.Substring(0, "Food".Length) == "Food"))
        {
            //print("关闭开关");
            IsOnSwitch = false;
            Ladder_Animation["Take 001"].speed = -1f;
            Ladder_Animation.Play();
        }
    }

}
