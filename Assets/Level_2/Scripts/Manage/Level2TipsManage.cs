using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level2TipsManage:MonoBehaviour
{
    private GameObject Player;
    private GameObject MainCamera;
    private GameObject Level2Logic;
    private GameObject Mirror;

    private GameObject MirrorTip;
    private GameObject StarTip;
    private GameObject OperationTips;

    private bool IsTrain = false;
    private float PlayerHeight = 1.22f;
    private float PickTipOffsetY = 0.5f;
	void Start () {
		Player = GameObject.FindGameObjectWithTag("Player");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Level2Logic = GameObject.Find("Level2Logic");
        Mirror = GameObject.Find("Mirror");

        MirrorTip = GameObject.Find("MirrorTip");
        StarTip = GameObject.Find("StarTip");
        OperationTips = GameObject.Find("OperationTips");
	}
	
	// Update is called once per frame
    void Update()
    {
        
        if (Player == null || OperationTips == null || Level2Logic == null||MainCamera==null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            OperationTips = GameObject.Find("Canvas/OperationTips");
            Level2Logic = GameObject.Find("Level2Logic");
        }
        else
        {
            Collider[] nearPlayer = Physics.OverlapSphere(Player.transform.position, PlayerHeight);
            OperationTipsLogic(nearPlayer);
        }
    }

    void MirrorTips()
    {
        Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(Mirror.transform.position + new Vector3(0f, PickTipOffsetY, 0f));;
        Vector3 posInCamera2 = MainCamera.GetComponent<Camera>().WorldToScreenPoint(Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.WallWillBePickPosition);;
        if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsOnWall == true)
        {
            if (posInCamera != null)
            {

                MirrorTip.GetComponent<Image>().enabled = true;
                MirrorTip.GetComponent<Image>().transform.position = posInCamera;
                MirrorTip.transform.Find("Text").GetComponent<Text>().enabled = true;
                MirrorTip.transform.Find("Text").GetComponent<Text>().text = "收起'F'/穿越'Q'";
            }
        }
        else
        {
            if (posInCamera2 != null)
            {
                if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsAbleWall == true)
                {
                    MirrorTip.GetComponent<Image>().enabled = true;
                    MirrorTip.GetComponent<Image>().transform.position = posInCamera2;
                    MirrorTip.transform.Find("Text").GetComponent<Text>().enabled = true;
                    MirrorTip.transform.Find("Text").GetComponent<Text>().text = "布置镜子 'F'";
                }
            }
        }
        if (posInCamera == null && posInCamera2 == null)
        {
            MirrorTip.GetComponent<Image>().enabled = false;
            MirrorTip.transform.Find("Text").GetComponent<Text>().enabled = false;
        }
    }

    void OperationTipsLogic(Collider[] nearPlayer)
    {
        //显示OperationTips条
        int noTrain = 0;
        for (int i = 0; i < nearPlayer.Length; i++)
        {
            //火车
            if (nearPlayer[i].tag == "Train")
            {
                IsTrain = true;
                if (Level2Logic.GetComponent<Level2LogicManage>().collectLogic.IsPlayerOverAnimation == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "启动火车~";
                }
                else if (Level2Logic.GetComponent<Level2LogicManage>().collectLogic.trainGetfuel == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "上车 'E'";
                }
                else if (Level2Logic.GetComponent<Level2LogicManage>().collectLogic.starEnough == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "给火车装入能源 'E'";
                }
                else
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "能源不足以到达下一站";
                }
            }
            else
            {
                noTrain++;
                if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsInSmallRoom == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "滚动滚轮切换到第一视角";
                }
                else if (nearPlayer[i].name == "Mirror")
                {
                    if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsOnWall == true)
                    {
                        if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsTooBigCantEnterRoom)
                        {
                            OperationTips.transform.Find("Text").GetComponent<Text>().text = "房间太小无法穿过去";
                        }
                    }
                }
                else if (nearPlayer[i].name == "PipelineToLowFloor" || nearPlayer[i].name == "MoveSwitch")
                {
                    if (Level2Logic.GetComponent<Level2LogicManage>().mirrorLogic.IsOnWall == true)
                    {
                        OperationTips.transform.Find("Text").GetComponent<Text>().text = "此机关还需要携带镜子开启";
                    }
                    else
                    {
                        OperationTips.transform.Find("Text").GetComponent<Text>().text = "提示：镜子能让你穿过镜子面对的墙";
                    }
                }
                else if (nearPlayer[i].name == "NoEnter")
                {
                    if (Level2Logic.GetComponent<Level2LogicManage>().collectLogic.StartNum < Level2Logic.GetComponent<Level2LogicManage>().collectLogic.AllStarNum)
                    {
                        OperationTips.transform.Find("Text").GetComponent<Text>().text = "能源还未寻找完全";
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        if (OperationTips.transform.Find("Text").GetComponent<Text>().text == "提示：镜子能让你穿过镜子正向面对的墙,'C'")
                        {
                            OperationTips.transform.Find("Text").GetComponent<Text>().text = "提示：小房间变小才能穿梭进去,'C'";
                        }
                        else if (OperationTips.transform.Find("Text").GetComponent<Text>().text == "提示：小房间变小才能穿梭进去,'C'")
                        {
                            OperationTips.transform.Find("Text").GetComponent<Text>().text = "提示：镜子能让你穿过镜子正向面对的墙,'C'";
                        }
                    }
                }
            }

            if (nearPlayer[i].tag == "FinalyThing")
            {
                Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(nearPlayer[i].transform.position+ new Vector3(0f, PickTipOffsetY, 0f));
                if (posInCamera != null)
                {
                    StarTip.GetComponent<Image>().enabled = true;
                    StarTip.GetComponent<Image>().transform.position = posInCamera;
                    StarTip.transform.Find("Text").GetComponent<Text>().enabled = true;
                    StarTip.transform.Find("Text").GetComponent<Text>().text = "获取能源 'E'";
                }
            }

            if (nearPlayer[i].name == "Mirror")
            {
                MirrorTips();
            }

            if (noTrain == nearPlayer.Length)
            {
                IsTrain = false;
            }
        }
    }

}
