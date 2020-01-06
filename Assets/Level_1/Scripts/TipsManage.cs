using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManage : MonoBehaviour {
    private bool IsHaveCatch = false;       //是否已经拿起一个东西
    private GameObject BeCatchThing;        //记录主角所在的碰撞器内的可以被抓的物体
    private bool TipIsOnUp = true;
    private float PickTipOffsetY = 0.15f;
    private float JumpTipOffsetY = 0.5f;
    private float PlayerHeight = 1.22f;
    private Vector3 beCatchPositionInView;  //目标被抓物体在视野内的坐标
    //判断点是否在摄像机内beCatchPositionInView = MainCamera.GetComponent<Camera>().WorldToScreenPoint(other.transform.position);
    //if (beCatchPositionInView.y > 0 && beCatchPositionInView.y < Screen.height)

    private int DuckNum = 0;
    private int TipColliderLayer = 15;
    //state
    private bool IsPickup = false;
    private bool IsThrow = false;
    private bool IsJump = false;
    private bool IsOverriver = false;
    private bool IsFuel = false;
    //private bool IsShot = false;
    private bool IsUpstairs = false;//Operation static
    private bool IsTrain = false;
    private bool IsMechanices = false;
    private bool IsGetFinalyThing = false;
    private bool IsShowStairsTips = false;
    //UI提示
    private GameObject Pickup;
    private GameObject JumpTips;
    private GameObject OverriverTip;
    private GameObject ShotTips;
    private GameObject OperationTips;

    private GameObject mechanices;

    //引用
    private GameObject MainCamera;
    private GameObject Player;
    void Start () {
        Pickup = GameObject.Find("Tips/Canvas/Pickup");
        JumpTips = GameObject.Find("Tips/Canvas/JumpTips");
        OverriverTip = GameObject.Find("Tips/Canvas/OverriverTip");
        ShotTips = GameObject.Find("Tips/Canvas/ShotTips");
        OperationTips = GameObject.Find("Tips/Canvas/OperationTips");

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Player = GameObject.FindGameObjectWithTag("Player");

        DuckNum = GameObject.FindGameObjectsWithTag("Ducka").Length + GameObject.FindGameObjectsWithTag("DuckWaterEat").Length;
    }
	void Update () {
        //拾取
        if (Player.GetComponent<PlayerCtrl>().Fruits_in_range.Length > 0 && Player.GetComponent<PlayerCtrl>().Fruits_in_range[0])
        {
            Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(Player.GetComponent<PlayerCtrl>().Fruits_in_range[0].transform.position + new Vector3(0f, PickTipOffsetY, 0f));
            if (posInCamera != null)
            {
                IsPickup = true;
                if (IsShowStairsTips == false)//和楼梯重叠时，显示楼梯的提示
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "果子有轻有重，小鸭子似乎很喜欢吃";
                }

                Pickup.GetComponent<Image>().enabled = true;
                Pickup.GetComponent<Image>().transform.position = posInCamera;
                Pickup.transform.Find("Text").GetComponent<Text>().enabled = true;
                if (Player.GetComponent<PlayerCtrl>().Fruits_in_range[0].name.Length >= "fruit_mushroom".Length && Player.GetComponent<PlayerCtrl>().Fruits_in_range[0].name.Substring(0, "fruit_mushroom".Length) == "fruit_mushroom")
                {
                    Pickup.transform.Find("Text").GetComponent<Text>().text = "菇菇果 'E'";
                }
                else
                {
                    Pickup.transform.Find("Text").GetComponent<Text>().text = "灯笼果 'E'";
                }
            }
        }
        else
        {
            IsPickup = false;
            if (Pickup)
            {
                Pickup.GetComponent<Image>().enabled = false;
                Pickup.transform.Find("Text").GetComponent<Text>().enabled = false;
            }
        }

        //抛出
        if (Player.GetComponent<PlayerCtrl>().Fruit_in_hand)
        {
            IsThrow = true;
            OperationTips.transform.Find("Text").GetComponent<Text>().text = "按住'F'可通过视角上下调整抛出远近";
        }
        else
        {
            IsThrow = false;
        }

        //跳跃
        if (Player.GetComponent<PlayerCtrl>().Duckas_in_range.Length > 0 
            && Player.GetComponent<PlayerCtrl>().Duckas_in_range.Length>=Player.GetComponent<PlayerCtrl>().Targeted_Ducka
            && Player.GetComponent<PlayerCtrl>().Targeted_Ducka!=-1
            &&Player.GetComponent<PlayerCtrl>().Duckas_in_range[Player.GetComponent<PlayerCtrl>().Targeted_Ducka])
        {
            Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(Player.GetComponent<PlayerCtrl>().Duckas_in_range[Player.GetComponent<PlayerCtrl>().Targeted_Ducka].transform.position + new Vector3(0f, JumpTipOffsetY, 0f));
            if (posInCamera != null)
            {
                IsJump = true;
                if (IsShowStairsTips == false)//和楼梯重叠时，显示楼梯的提示
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "'Q'切换过河点，河中需要加个落脚点";
                }

                JumpTips.GetComponent<Image>().enabled = true;
                JumpTips.GetComponent<Image>().transform.position = posInCamera;
                JumpTips.transform.Find("Text").GetComponent<Text>().enabled = true;
            }
        }
        else
        {
            IsJump = false;
            if (JumpTips)
            {
                JumpTips.GetComponent<Image>().enabled = false;
                JumpTips.transform.Find("Text").GetComponent<Text>().enabled = false;
            }
        }

        //射击提示
        if (GameObject.FindGameObjectsWithTag("Ducka").Length + GameObject.FindGameObjectsWithTag("DuckWaterEat").Length > DuckNum)
        {
            DuckNum = GameObject.FindGameObjectsWithTag("Ducka").Length + GameObject.FindGameObjectsWithTag("DuckWaterEat").Length;
            ShotTips.GetComponent<Image>().enabled = true;
            ShotTips.transform.Find("Text").GetComponent<Text>().enabled = true;
            StartCoroutine(WaitAndCloseShotTips(ShotTips));
        }

        //上下楼梯
        /*Collider[] colliders = Physics.OverlapSphere(Player.transform.position, Player.GetComponent<CapsuleCollider>().radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "LadderBottom")
            {
                if (GameObject.Find("button_" + collider.transform.name.Substring(12, 1)).GetComponent<StairsSwitch>().IsOnSwitch == true)
                {
                    IsUpstairs = true;
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "'E'上楼";
                }
                else
                {
                    IsUpstairs = false;
                }
            }
            else if (collider.gameObject.tag == "TopBottom")
            {
                if (GameObject.Find("button_" + collider.transform.parent.name.Substring(12, 1)).GetComponent<StairsSwitch>().IsOnSwitch == true)
                {
                    IsUpstairs = true;
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "'E'下楼";
                }
                else
                {
                    IsUpstairs = false;
                }
            }
            else
            {
                IsUpstairs = false;
            }
        }*/

        //拾取燃料
        if (GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().HaveFuel == false)
        {
            if (GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().CanGetFuel == true)
            {
                IsFuel = true;
                OperationTips.transform.Find("Text").GetComponent<Text>().text = "该能源能为各种设备供能";

                Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().fuel.transform.position + new Vector3(0f, PickTipOffsetY, 0f));
                Pickup.GetComponent<Image>().enabled = true;
                Pickup.GetComponent<Image>().transform.position = posInCamera;
                Pickup.transform.Find("Text").GetComponent<Text>().enabled = true;
                Pickup.transform.Find("Text").GetComponent<Text>().text = "能源'E'";
            }
            else
            {
                IsFuel = false;
            }
        }
        else
        {
            IsFuel = false;
        }

        //显示OperationTips条
        Collider[] nearPlayer = Physics.OverlapSphere(Player.transform.position, PlayerHeight);
        int noTrain = 0;
        int noMechanices = 0;
        for (int i = 0; i < nearPlayer.Length;i++)
        {
            //Debug.Log(other.gameObject.name);
            //火车
            if (nearPlayer[i].tag == "Train")
            {
                //Debug.Log("火车");
                IsTrain = true;
                if (GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().IsOpenTrain == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "启动火车~";
                }
                else if (GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().CanOpenTrain == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "上车'E'";
                }
                else if (GameObject.Find("GameLogic").GetComponent<Level1GameLogic>().HaveFuel == true)
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "给火车装入能源'E'";
                }
                else
                {
                    OperationTips.transform.Find("Text").GetComponent<Text>().text = "火车无法启动，塔顶似乎有奇怪的亮光";
                }
            }
            else
            {
                noTrain++;
            }
            if (noTrain == nearPlayer.Length)
            {
                IsTrain = false;
            }

            //机关
            if (nearPlayer[i].tag == "Mechanices")
            {
                IsMechanices = true;
                mechanices = nearPlayer[i].gameObject;
                if (IsShowStairsTips==false)//和楼梯重叠时，显示楼梯的提示
                {
                    if (nearPlayer[i].gameObject.transform.GetComponent<StairsSwitch>().IsOnSwitch == false)
                    {
                        OperationTips.transform.Find("Text").GetComponent<Text>().text = "这个机关似乎和塔有关,也许可以压下";
                    }
                    else
                    {
                        OperationTips.transform.Find("Text").GetComponent<Text>().text = "此机关已经开启,塔那边有了反应";
                    }
                }
            }
            else
            {
                noMechanices++;
            }
            if (noMechanices == nearPlayer.Length)
            {
                IsMechanices = false;
            }
        }


        if (IsPickup == false && IsJump == false && IsOverriver == false && IsUpstairs == false && IsFuel == false &&IsTrain==false&& IsMechanices == false)
        {
            OperationTips.transform.Find("Text").GetComponent<Text>().text = "提示：水中的小鸭子也许能帮你过河";
        }
        else
        {
            OperationTips.GetComponent<Image>().enabled = true;
            OperationTips.transform.Find("Text").GetComponent<Text>().enabled = true;
        }
	}
    IEnumerator WaitAndCloseShotTips(GameObject Tips)//射击提示
    {
        yield return new WaitForSeconds(1f);
        Tips.GetComponent<Image>().enabled = false;
        Tips.transform.Find("Text").GetComponent<Text>().enabled = false;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "LadderBottom")
        {
            IsShowStairsTips = true;
            Debug.Log("R");
            if (GameObject.Find("button_1").GetComponent<StairsSwitch>().IsOnSwitch == true
                && GameObject.Find("button_2").GetComponent<StairsSwitch>().IsOnSwitch == true
                && GameObject.Find("button_3").GetComponent<StairsSwitch>().IsOnSwitch == true)
            {
                IsUpstairs = true;
                OperationTips.transform.Find("Text").GetComponent<Text>().text = "自动上楼'R'";
            }
            else
            {
                IsUpstairs = false;
                OperationTips.transform.Find("Text").GetComponent<Text>().text = "还有未开启的机关，展开所有楼梯才能上楼";
            }
        }
        else
        {
            IsShowStairsTips = true;
        }
        /*else if (collider.gameObject.tag == "TopBottom")
        {
            if (GameObject.Find("button_" + collider.transform.parent.name.Substring(12, 1)).GetComponent<StairsSwitch>().IsOnSwitch == true)
            {
                IsUpstairs = true;
                OperationTips.transform.Find("Text").GetComponent<Text>().text = "'E'下楼";
            }
            else
            {
                IsUpstairs = false;
            }
        }*/
        //else
        //{
           // IsUpstairs = false;
        //}
    }
}
