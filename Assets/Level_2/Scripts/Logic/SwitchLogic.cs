using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SwitchLogic{
    private RoomRecorder roomRecorder;

    public string nowTip;
    private GameObject Player;
    private GameObject Mirror;
    private GameObject MirrorVirtual;
    private float triggerDistance = 1f;
    private List<GameObject> nearSwitch;
    public KeyCode switchKey = KeyCode.R;
    private Vector3 PlayerOriginSize = new Vector3(1f, 1f, 1f);
    private float PlayerMinSize = 0.5f;//MirrorLogic也有
    private float PlayerMaxSize = 1f;//MirrorLogic也有
    private float durationSwitch = 1f;
    private PlayState oldState = PlayState.Paused;

    private GameObject moveSwitch;
    private GameObject bigSwitch;
    private GameObject smallSwitch;
    private GameObject pipeline;
    private int[] pipelineLineRooms = { 6, 7 };

    public void Init(RoomRecorder Level2LogicManage_roomRecorder) 
    {
        nearSwitch = new List<GameObject>();
        roomRecorder = Level2LogicManage_roomRecorder;

        Player = GameObject.FindGameObjectWithTag("Player");
        moveSwitch = GameObject.Find("MoveSwitch");
        bigSwitch = GameObject.Find("BigSwitch");
        smallSwitch = GameObject.Find("SmallSwitch");
        pipeline = GameObject.Find("Pipeline");
        GameObject[] mirrors = GameObject.FindGameObjectsWithTag("Mirror");
        foreach (GameObject mirror in mirrors)
        {
            if (mirror.name == "Mirror")
            {
                Mirror = mirror;
            }
            else
            {
                MirrorVirtual = mirror;
            }
        }
	}
    public void Update(bool IsOnWall)
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            Collider[] nearColliders = Physics.OverlapSphere(Player.transform.position, triggerDistance);
            if (nearSwitch!=null)
            {
                nearSwitch.Clear();
            }
            if (nearColliders.Length > 0)
            {
                foreach (Collider collder in nearColliders)
                {
                    if (collder.gameObject.tag == "Mechanices")
                    {
                        nearSwitch.Add(collder.gameObject);
                    }
                }
                for (int i = 0; i < nearSwitch.Count; i++)
                {
                    if (nearSwitch[i].name == "BigSwitch")
                    {
                        nowTip = "BigSwitch";
                        nearSwitch[i].GetComponentInChildren<Animation>().Play();
                        //Debug.Log("变大");
                        PlayerChangeBig();
                    }
                    else if (nearSwitch[i].name == "SmallSwitch")
                    {
                        nowTip = "SmallSwitch";
                        if (Input.GetKeyDown(switchKey))
                        {
                            //Debug.Log("变小");
                            nearSwitch[i].GetComponentInChildren<Animation>().Play();
                            PlayerChangeSmall();
                        }
                    }
                    else if (nearSwitch[i].name == "RotateSwitch")
                    {
                        nowTip = "RotateSwitch";
                        if (Input.GetKeyDown(switchKey))
                        {
                            Debug.Log("旋转");
                            nearSwitch[i].GetComponentInChildren<Animation>().Play();
                            RotateSwitch();
                        }
                    }
                    else if (nearSwitch[i].name == "SizeSwitch")
                    {
                        nowTip = "SizeSwitch";
                        if (Input.GetKeyDown(switchKey))
                        {
                            Debug.Log("交换");
                            nearSwitch[i].GetComponentInChildren<Animation>().Play();
                            SizeSwitch();
                        }
                    }
                    else if (nearSwitch[i].name == "MoveSwitch")
                    {
                        nowTip = "MoveSwitch";
                        if (Input.GetKeyDown(switchKey))
                        {
                            if (IsOnWall == true)
                            {
                                nowTip = "需要镜子来开启机关";
                            }
                            else
                            {
                                Debug.Log("移动");
                                Player.GetComponent<NavMeshAgent>().enabled = false;
                                Player.GetComponent<CapsuleCollider>().enabled = false;
                                nearSwitch[i].GetComponent<PlayableDirector>().Play();
                            }
                        }
                    }
                    else if (nearSwitch[i].name == "PipelineToLowFloor")
                    {
                        if (Input.GetKeyDown(switchKey))
                        {
                            if (IsOnWall == true)
                            {
                                nowTip = "需要镜子来开启机关";
                            }
                            else
                            {
                                nowTip = "PipelineToLowFloor";
                                PipelineToLowFloor();
                            }
                            Debug.Log(nowTip);
                        }
                        else
                        {
                            nowTip = "PipelineToLowFloor";
                        }
                    }
                }
            }
        }


        if (oldState == PlayState.Playing
            && moveSwitch.GetComponent<PlayableDirector>().state == PlayState.Paused
            && Player.GetComponent<NavMeshAgent>().enabled == false)
        {
            Player.GetComponent<NavMeshAgent>().enabled = true;
            Player.GetComponent<CapsuleCollider>().enabled = true;
        }
        oldState = moveSwitch.GetComponent<PlayableDirector>().state;
    }
        

    void PlayerChangeBig()
    {
        if (Player.transform.localScale != PlayerOriginSize *PlayerMaxSize)
        {
            Player.transform.localScale = PlayerOriginSize * Mathf.SmoothStep(PlayerMinSize, PlayerMaxSize, durationSwitch);
        }
    }
    void PlayerChangeSmall()
    {
        if (Player.transform.localScale != PlayerOriginSize * PlayerMinSize)
        {
            Player.transform.localScale = PlayerOriginSize * Mathf.SmoothStep(PlayerMaxSize, PlayerMinSize, durationSwitch);
        }
    }
    void SizeSwitch()
    {
        Vector3 midPosition = smallSwitch.transform.position;
        smallSwitch.transform.position = bigSwitch.transform.position;
        bigSwitch.transform.position = midPosition;
    }
    void RotateSwitch()
    {
        GameObject roomNeedRotate = GameObject.Find("Room" + "8");
        roomNeedRotate.transform.eulerAngles = new Vector3(roomNeedRotate.transform.eulerAngles.x + 180f, roomNeedRotate.transform.eulerAngles.y,roomNeedRotate.transform.eulerAngles.z);
    }
    void PipelineToLowFloor()
    {
        Player.GetComponent<NavMeshAgent>().enabled = false;
        Player.transform.position = roomRecorder.rooms[7].position;
        Player.GetComponent<NavMeshAgent>().enabled = true;
    }
}
