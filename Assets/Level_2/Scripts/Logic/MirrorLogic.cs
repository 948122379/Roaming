using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MirrorLogic
{
    private RoomRecorder roomRecorder;

    private GameObject Player;
    private GameObject camera_Y_Axis;
    private GameObject Mirror;
    private GameObject MirrorVirtual;
    private float rotationY;
    private string nowPlayerForward;
    public string nowTip;
    public int nowRoom = 0;
    public bool IsOnWall = false;
    private float MirrorSize;
    public bool IsAbleWall = false;
    private bool IsAbleNextWall = false;
    public string wallHaveMirror = "";
    private float playerOutWallDistance = 1f;
    private Vector3 playerOutWallPosition = new Vector3(0f, 0f, 0f);
    private int nextRoom = 0;
    private float distancefloat = 0.2f;
    private bool IsStopGetDir = false;
    public bool IsInSmallRoom = false;
    public bool IsTooBigCantEnterRoom = false;
    public Vector3 WallWillBePickPosition = Vector3.zero;

    private KeyCode pickMirrorKey = KeyCode.F;
    public KeyCode throughtMirror = KeyCode.Q;
    private float PlayerMinSize = 0.5f;//SwitchLogic也有
    private float PlayerMaxSize = 1f;//SwitchLogic也有
    private float viewAngle = 80f;//确定前后左右方向视野角度 
    private float maxDistance = 1.5f;//在附近的距离
    private float playerHeight = 1.2f;//主角高度
    private Vector3 mirrorSize = new Vector3(0.8f, 0.01f, 1.22f);//镜子的宽厚高，高度最好与人的身高一致
    public void Init(RoomRecorder Level2LogicManage_roomRecorder)
    {
        roomRecorder = Level2LogicManage_roomRecorder;
        Player = GameObject.FindGameObjectWithTag("Player");
        camera_Y_Axis = GameObject.Find("Camera_Y_Axis");
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
        Debug.Log(Mirror + "" + MirrorVirtual + "" + Player);
        
        
    }
    public void Update()
    {
        if (Player == null || camera_Y_Axis == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            camera_Y_Axis = GameObject.Find("Camera_Y_Axis");

            //把主角和镜子放在开始位置的墙上,不要与下面测试代码一起定义初始位置
            nowRoom = 0;
            nowPlayerForward = nowTip = "down";
            PutMirror("right");
        }
        else
        {
            //用来测试：搜索主角位置,确定当前房间
            for (int i = 0; i < roomRecorder.rooms.Length; i++)
            {
                if (Player.transform.position.x > roomRecorder.rooms[i].position.x - roomRecorder.rooms[i].roomSize.x / 2
                    && Player.transform.position.x < roomRecorder.rooms[i].position.x + roomRecorder.rooms[i].roomSize.x / 2
                    && Player.transform.position.y > roomRecorder.rooms[i].position.y - roomRecorder.rooms[i].roomSize.y / 2
                    && Player.transform.position.y < roomRecorder.rooms[i].position.y + roomRecorder.rooms[i].roomSize.y / 2
                    && Player.transform.position.z > roomRecorder.rooms[i].position.z - roomRecorder.rooms[i].roomSize.z / 2
                    && Player.transform.position.z < roomRecorder.rooms[i].position.z + roomRecorder.rooms[i].roomSize.z / 2)
                {
                    nowRoom = i;//可能会和下面nowRoom赋值冲突
                    if (roomRecorder.rooms[i].scale > roomRecorder.minsize - 0.01f
                      && roomRecorder.rooms[i].scale < roomRecorder.minsize + 0.01f)
                    {
                        Player.transform.localScale = new Vector3(PlayerMinSize, PlayerMinSize, PlayerMinSize);
                        IsInSmallRoom = true;
                    }
                    else
                    {
                        IsInSmallRoom = false;
                    }
                    break;
                }
            }

            PickupMirror();
        }
    }
    void PickupMirror()
    {
        GetNowForward();
        if (IsOnWall == false)
        {
            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
            foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
            {
                meshRender.enabled = false;
            }
            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
            {
                meshRender.enabled = false;
            }

            Mirror.transform.position = Player.transform.position;

            if (IsAbleWall == true)
            {
                if (Input.GetKeyDown(pickMirrorKey))
                {
                    //Debug.Log("放镜子");
                    PutMirror(nowTip);
                }
            }
        }
        else
        {
            if (Vector3.Distance(Mirror.transform.position, Player.transform.position) < maxDistance)
            {
                //提示可以拿下或者穿越
                nowTip = "pick or througt";
                //if (nowPlayerForward == wallHaveMirror)//会导致拿不下来
                //{
                if (Input.GetKeyDown(pickMirrorKey))
                {
                    //IsStopGetDir = false;
                    IsOnWall = false;
                    IsAbleNextWall = false;
                    wallHaveMirror = "";
                }
                else if (Input.GetKeyUp(throughtMirror))
                {
                    //IsStopGetDir = true;
                    if (IsAbleNextWall == true)
                    {
                        Player.GetComponent<NavMeshAgent>().enabled = false;
                        Player.transform.position = playerOutWallPosition;
                        Player.GetComponent<NavMeshAgent>().enabled = true;
                        nowRoom = nextRoom;
                        PutMirror(wallHaveMirror);//用当前方向记录搜寻是否出现虚拟镜子
                    }
                }
                //}
            }
        }
    }
    void GetNowForward()
    {
        //Debug.Log(roomRecorder.walls[nowRoom].downwallOffset);
        //if (Vector3.Distance(Player.transform.position, roomRecorder.walls[nowRoom].downwallOffset) < maxDistance * roomRecorder.rooms[nowRoom].scale / 2)
        if (camera_Y_Axis.transform.eulerAngles.x > 29
            && camera_Y_Axis.transform.eulerAngles.x < 80)
        {
            nowPlayerForward = "down";
        }
        else
        {
            float playerRotationY = Player.transform.eulerAngles.y;
            if (playerRotationY > 0)
            {
                playerRotationY = playerRotationY % 360;
            }
            else
            {
                playerRotationY = 360 + playerRotationY % 360;
            }

            if (playerRotationY > 0 && playerRotationY < viewAngle / 2
                || playerRotationY < 360 && playerRotationY > 360 - viewAngle / 2)//forward
            {
                nowPlayerForward = "forward";
            }
            else if (playerRotationY > 180 - viewAngle / 2 && playerRotationY < 180 + viewAngle / 2)//back
            {
                nowPlayerForward = "back";
            }
            else if (playerRotationY > 90 - viewAngle / 2 && playerRotationY < 90 + viewAngle / 2)//right
            {
                nowPlayerForward = "right";
            }
            else if (playerRotationY > 270 - viewAngle / 2 && playerRotationY < 270 + viewAngle / 2)//left
            {
                nowPlayerForward = "left";
            }
            else
            {
                nowPlayerForward = "";
            }
        }
        GetCanPutWallDir();
    }
    void GetCanPutWallDir()
    {
        switch (nowPlayerForward)
        {
            case "forward":
                {
                    if (Mathf.Abs(Player.transform.position.z - roomRecorder.walls[nowRoom].forwardwallOffset.z) < maxDistance)
                    {
                        nowTip = "forward";
                        WallWillBePickPosition = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, roomRecorder.walls[nowRoom].forwardwallOffset.z);
                    }
                    break;
                }
            case "back":
                {
                    if (Mathf.Abs(Player.transform.position.z - roomRecorder.walls[nowRoom].backwallOffset.z) < maxDistance)
                    {
                        nowTip = "back";
                        WallWillBePickPosition = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, roomRecorder.walls[nowRoom].backwallOffset.z);
                    }
                    break;
                }
            case "right":
                {
                    if (Mathf.Abs(Player.transform.position.x - roomRecorder.walls[nowRoom].rightwallOffset.x) < maxDistance)
                    {
                        nowTip = "right";
                        WallWillBePickPosition = new Vector3(roomRecorder.walls[nowRoom].rightwallOffset.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, Player.transform.position.z);
                    }
                    break;
                }
            case "left":
                {
                    if (Mathf.Abs(Player.transform.position.x - roomRecorder.walls[nowRoom].leftwallOffset.x) < maxDistance)
                    {
                        nowTip = "left";
                        WallWillBePickPosition = new Vector3(roomRecorder.walls[nowRoom].leftwallOffset.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, Player.transform.position.z);
                    }
                    break;
                }
            case "down":
                {
                    if (Mathf.Abs(Player.transform.position.y - roomRecorder.walls[nowRoom].downwallOffset.y) < maxDistance)
                    {
                        nowTip = "down";
                        WallWillBePickPosition = new Vector3(Player.transform.position.x, roomRecorder.walls[nowRoom].downwallOffset.y, Player.transform.position.z);
                    }
                    break;
                }
            case "":
                {
                    nowTip = "";
                    break;
                }
        }
        if (nowTip != "")
        {
            IsAbleWall = true;
        }
        else
        {
            IsAbleWall = false;
        }
    }
    void PutMirror(string Dir)
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            IsAbleNextWall = false;
            
            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
            {
                meshRender.enabled = false;
            }

            nextRoom = nowRoom;
            switch (Dir)
            {
                case "forward":
                    {
                        Mirror.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, roomRecorder.walls[nowRoom].forwardwallOffset.z);
                        Mirror.transform.eulerAngles = new Vector3(90f, roomRecorder.WallRotation[0], 0f);
                        Mirror.transform.localScale = new Vector3(mirrorSize.x * Player.transform.localScale.x, mirrorSize.y, mirrorSize.z * Player.transform.localScale.x);
                        wallHaveMirror = "forward";
                        foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshRender.enabled = true;
                        }

                        //穿出后二次搜索
                        if (roomRecorder.rooms[nowRoom].backroom != -1)
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = true;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = true;
                            }
                            
                            float MirrorVirtualScale = 1f;
                            if (roomRecorder.rooms[roomRecorder.rooms[nowRoom].backroom].scale > roomRecorder.minsize - 0.01f
                                && roomRecorder.rooms[roomRecorder.rooms[nowRoom].backroom].scale < roomRecorder.minsize + 0.01f)
                            {
                                MirrorVirtualScale = PlayerMinSize;
                            }
                            else
                            {
                                MirrorVirtualScale = PlayerMaxSize;
                            }
                            MirrorVirtual.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * MirrorVirtualScale, roomRecorder.walls[roomRecorder.rooms[nowRoom].backroom].forwardwallOffset.z);
                            MirrorVirtual.transform.eulerAngles = Mirror.transform.eulerAngles;
                            MirrorVirtual.transform.localScale = new Vector3(mirrorSize.x * MirrorVirtualScale, mirrorSize.y, mirrorSize.z * MirrorVirtualScale);
                            if (MirrorVirtualScale > Player.transform.localScale.x - 0.01)
                            {
                                IsTooBigCantEnterRoom = false;
                                if (MirrorVirtual.transform.position.x <= roomRecorder.walls[roomRecorder.rooms[nowRoom].backroom].forwardwallOffset.x + roomRecorder.rooms[roomRecorder.rooms[nowRoom].backroom].roomSize.x / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.x >= roomRecorder.walls[roomRecorder.rooms[nowRoom].backroom].forwardwallOffset.x - roomRecorder.rooms[roomRecorder.rooms[nowRoom].backroom].roomSize.x / 2 + mirrorSize.x * MirrorVirtualScale / 2 - +distancefloat)
                                {
                                    playerOutWallPosition = MirrorVirtual.transform.position - new Vector3(0f, playerHeight / 2 * MirrorVirtualScale, 0f) + new Vector3(0f, 0f, -playerOutWallDistance * MirrorVirtualScale);
                                    nextRoom = roomRecorder.rooms[nowRoom].backroom;
                                    IsAbleNextWall = true;
                                }
                                else
                                {
                                    //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                                    {
                                        meshRender.enabled = false;
                                    }
                                }
                            }
                            else
                            {
                                //提示太大了进不去
                                IsTooBigCantEnterRoom = true;
                            }
                        }
                        break;
                    }
                case "back":
                    {
                        Mirror.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, roomRecorder.walls[nowRoom].backwallOffset.z);
                        Mirror.transform.eulerAngles = new Vector3(90f, roomRecorder.WallRotation[1], 0f);
                        Mirror.transform.localScale = new Vector3(mirrorSize.x * Player.transform.localScale.x, mirrorSize.y, mirrorSize.z * Player.transform.localScale.x);
                        wallHaveMirror = "back";
                        foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshRender.enabled = true;
                        }
                        //穿出后二次搜索
                        if (roomRecorder.rooms[nowRoom].forwardroom != -1)
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = true;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = true;
                            }
                            
                            float MirrorVirtualScale = 1f;
                            if (roomRecorder.rooms[roomRecorder.rooms[nowRoom].forwardroom].scale > roomRecorder.minsize - 0.01f
                                && roomRecorder.rooms[roomRecorder.rooms[nowRoom].forwardroom].scale < roomRecorder.minsize + 0.01f)
                            {
                                MirrorVirtualScale = PlayerMinSize;
                            }
                            else
                            {
                                MirrorVirtualScale = PlayerMaxSize;
                            }
                            MirrorVirtual.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + playerHeight / 2 * MirrorVirtualScale, roomRecorder.walls[roomRecorder.rooms[nowRoom].forwardroom].backwallOffset.z);
                            MirrorVirtual.transform.eulerAngles = Mirror.transform.eulerAngles;
                            MirrorVirtual.transform.localScale = new Vector3(mirrorSize.x * MirrorVirtualScale, mirrorSize.y, mirrorSize.z * MirrorVirtualScale);
                            if (MirrorVirtualScale > Player.transform.localScale.x - 0.01)
                            {
                                IsTooBigCantEnterRoom = false;
                                if (MirrorVirtual.transform.position.x <= roomRecorder.walls[roomRecorder.rooms[nowRoom].forwardroom].backwallOffset.x + roomRecorder.rooms[roomRecorder.rooms[nowRoom].forwardroom].roomSize.x / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.x >= roomRecorder.walls[roomRecorder.rooms[nowRoom].forwardroom].backwallOffset.x - roomRecorder.rooms[roomRecorder.rooms[nowRoom].forwardroom].roomSize.x / 2 + mirrorSize.x * MirrorVirtualScale / 2 - +distancefloat)
                                {
                                    playerOutWallPosition = MirrorVirtual.transform.position - new Vector3(0f, playerHeight / 2 * MirrorVirtualScale, 0f) + new Vector3(0f, 0f, playerOutWallDistance * MirrorVirtualScale);
                                    nextRoom = roomRecorder.rooms[nowRoom].forwardroom;
                                    IsAbleNextWall = true;
                                }
                                else
                                {
                                    //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                                    {
                                        meshRender.enabled = false;
                                    }
                                }
                            }
                            else
                            {
                                //提示太大了进不去
                                IsTooBigCantEnterRoom = true;
                            }
                        }
                        break;
                    }
                case "right":
                    {
                        Mirror.transform.position = new Vector3(roomRecorder.walls[nowRoom].rightwallOffset.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, Player.transform.position.z);
                        Mirror.transform.eulerAngles = new Vector3(90f, roomRecorder.WallRotation[2], 0f);
                        Mirror.transform.localScale = new Vector3(mirrorSize.x * Player.transform.localScale.x, mirrorSize.y, mirrorSize.z * Player.transform.localScale.x);
                        wallHaveMirror = "right";
                        foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshRender.enabled = true;
                        }
                        //穿出后二次搜索
                        if (roomRecorder.rooms[nowRoom].leftroom != -1)
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = true;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = true;
                            }
                            
                            float MirrorVirtualScale = 1f;
                            if (roomRecorder.rooms[roomRecorder.rooms[nowRoom].leftroom].scale > roomRecorder.minsize - 0.01f
                                && roomRecorder.rooms[roomRecorder.rooms[nowRoom].leftroom].scale < roomRecorder.minsize + 0.01f)
                            {
                                MirrorVirtualScale = PlayerMinSize;
                            }
                            else
                            {
                                MirrorVirtualScale = PlayerMaxSize;
                            }
                            MirrorVirtual.transform.position = new Vector3(roomRecorder.walls[roomRecorder.rooms[nowRoom].leftroom].rightwallOffset.x, Player.transform.position.y + playerHeight / 2 * MirrorVirtualScale, Player.transform.position.z);
                            MirrorVirtual.transform.eulerAngles = Mirror.transform.eulerAngles;
                            MirrorVirtual.transform.localScale = new Vector3(mirrorSize.x * MirrorVirtualScale, mirrorSize.y, mirrorSize.z * MirrorVirtualScale);
                            if (MirrorVirtualScale > Player.transform.localScale.x - 0.01)
                            {
                                IsTooBigCantEnterRoom = false;
                                if (MirrorVirtual.transform.position.z <= roomRecorder.walls[roomRecorder.rooms[nowRoom].leftroom].rightwallOffset.z + roomRecorder.rooms[roomRecorder.rooms[nowRoom].leftroom].roomSize.z / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.z >= roomRecorder.walls[roomRecorder.rooms[nowRoom].leftroom].rightwallOffset.z - roomRecorder.rooms[roomRecorder.rooms[nowRoom].leftroom].roomSize.z / 2 + mirrorSize.x * MirrorVirtualScale / 2 - +distancefloat)
                                {
                                    playerOutWallPosition = MirrorVirtual.transform.position - new Vector3(0f, playerHeight / 2 * MirrorVirtualScale, 0f) - new Vector3(playerOutWallDistance * MirrorVirtualScale, 0f, 0f);
                                    nextRoom = roomRecorder.rooms[nowRoom].leftroom;
                                    IsAbleNextWall = true;
                                }
                                else
                                {
                                    //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                                    {
                                        meshRender.enabled = false;
                                    }
                                }
                            }
                            else
                            {
                                //提示太大了进不去
                                IsTooBigCantEnterRoom = true;
                            }
                        }
                        break;
                    }
                case "left":
                    {
                        Mirror.transform.position = new Vector3(roomRecorder.walls[nowRoom].leftwallOffset.x, Player.transform.position.y + playerHeight / 2 * Player.transform.localScale.x, Player.transform.position.z);
                        Mirror.transform.eulerAngles = new Vector3(90f, roomRecorder.WallRotation[3], 0f);
                        Mirror.transform.localScale = new Vector3(mirrorSize.x * Player.transform.localScale.x, mirrorSize.y, mirrorSize.z * Player.transform.localScale.x);
                        wallHaveMirror = "left";
                        foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshRender.enabled = true;
                        }
                        //穿出后二次搜索
                        if (roomRecorder.rooms[nowRoom].rightroom != -1)
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = true;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = true;
                            }
                            
                            float MirrorVirtualScale = 1f;
                            if (roomRecorder.rooms[roomRecorder.rooms[nowRoom].rightroom].scale > roomRecorder.minsize - 0.01f
                                && roomRecorder.rooms[roomRecorder.rooms[nowRoom].rightroom].scale < roomRecorder.minsize + 0.01f)
                            {
                                MirrorVirtualScale = PlayerMinSize;
                            }
                            else
                            {
                                MirrorVirtualScale = PlayerMaxSize;
                            }
                            MirrorVirtual.transform.position = new Vector3(roomRecorder.walls[roomRecorder.rooms[nowRoom].rightroom].leftwallOffset.x, Player.transform.position.y + playerHeight / 2 * MirrorVirtualScale, Player.transform.position.z);
                            MirrorVirtual.transform.eulerAngles = Mirror.transform.eulerAngles;
                            MirrorVirtual.transform.localScale = new Vector3(mirrorSize.x * MirrorVirtualScale, mirrorSize.y, mirrorSize.z * MirrorVirtualScale);
                            if (MirrorVirtualScale > Player.transform.localScale.x - 0.01)
                            {
                                IsTooBigCantEnterRoom = false;
                                if (MirrorVirtual.transform.position.z <= roomRecorder.walls[roomRecorder.rooms[nowRoom].rightroom].leftwallOffset.z + roomRecorder.rooms[roomRecorder.rooms[nowRoom].rightroom].roomSize.z / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.z >= roomRecorder.walls[roomRecorder.rooms[nowRoom].rightroom].leftwallOffset.z - roomRecorder.rooms[roomRecorder.rooms[nowRoom].rightroom].roomSize.z / 2 + mirrorSize.x * MirrorVirtualScale / 2 - distancefloat)
                                {
                                    playerOutWallPosition = MirrorVirtual.transform.position - new Vector3(0f, playerHeight / 2 * MirrorVirtualScale, 0f) + new Vector3(playerOutWallDistance * MirrorVirtualScale, 0f, 0f);
                                    nextRoom = roomRecorder.rooms[nowRoom].rightroom;
                                    IsAbleNextWall = true;
                                }
                                else
                                {
                                    //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                                    {
                                        meshRender.enabled = false;
                                    }
                                }
                            }
                            else
                            {
                                //提示太大了进不去
                                IsTooBigCantEnterRoom = true;
                            }
                        }
                        else
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = false;
                            }
                        }
                        break;
                    }
                case "down":
                    {
                        Mirror.transform.position = new Vector3(Player.transform.position.x, roomRecorder.walls[nowRoom].downwallOffset.y, Player.transform.position.z);
                        Mirror.transform.eulerAngles = new Vector3(90f + roomRecorder.WallRotation[4], 0f, 0f);
                        Mirror.transform.localScale = new Vector3(mirrorSize.x * Player.transform.localScale.x, mirrorSize.y, mirrorSize.z * Player.transform.localScale.x);
                        wallHaveMirror = "down";
                        foreach (MeshRenderer meshRender in Mirror.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshRender.enabled = true;
                        }
                        //穿出后二次搜索
                        if (roomRecorder.rooms[nowRoom].uproom != -1)
                        {
                            //MirrorVirtual.GetComponent<MeshRenderer>().enabled = true;
                            foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                            {
                                meshRender.enabled = true;
                            }
                            float MirrorVirtualScale = 1f;
                            if (roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].scale > roomRecorder.minsize - 0.01f
                                && roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].scale < roomRecorder.minsize + 0.01f)
                            {
                                MirrorVirtualScale = PlayerMinSize;
                            }
                            else
                            {
                                MirrorVirtualScale = PlayerMaxSize;
                            }
                            MirrorVirtual.transform.position = new Vector3(Player.transform.position.x, roomRecorder.walls[roomRecorder.rooms[nowRoom].uproom].downwallOffset.y, Player.transform.position.z);
                            MirrorVirtual.transform.eulerAngles = Mirror.transform.eulerAngles;
                            MirrorVirtual.transform.localScale = new Vector3(mirrorSize.x * MirrorVirtualScale, mirrorSize.y, mirrorSize.z * MirrorVirtualScale);
                            if (MirrorVirtualScale > Player.transform.localScale.x - 0.01)
                            {
                                IsTooBigCantEnterRoom = false;
                                if (MirrorVirtual.transform.position.x <= roomRecorder.walls[roomRecorder.rooms[nowRoom].uproom].downwallOffset.x + roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].roomSize.x / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.x >= roomRecorder.walls[roomRecorder.rooms[nowRoom].uproom].downwallOffset.x - roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].roomSize.x / 2 + mirrorSize.x * MirrorVirtualScale / 2 - +distancefloat
                                    && MirrorVirtual.transform.position.z <= roomRecorder.walls[roomRecorder.rooms[nowRoom].uproom].downwallOffset.z + roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].roomSize.z / 2 - mirrorSize.x * MirrorVirtualScale / 2 + distancefloat
                                    && MirrorVirtual.transform.position.z >= roomRecorder.walls[roomRecorder.rooms[nowRoom].uproom].downwallOffset.z - roomRecorder.rooms[roomRecorder.rooms[nowRoom].uproom].roomSize.z / 2 + mirrorSize.x * MirrorVirtualScale / 2 - +distancefloat)
                                {
                                    playerOutWallPosition = MirrorVirtual.transform.position + new Vector3(0f, playerOutWallDistance * MirrorVirtualScale, 0f);
                                    nextRoom = roomRecorder.rooms[nowRoom].uproom;
                                    IsAbleNextWall = true;
                                }
                                else
                                {
                                    //MirrorVirtual.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (MeshRenderer meshRender in MirrorVirtual.GetComponentsInChildren<MeshRenderer>())
                                    {
                                        meshRender.enabled = false;
                                    }
                                    
                                }
                            }
                            else
                            {
                                //提示太大了进不去
                                IsTooBigCantEnterRoom = true;
                            }
                        }
                        break;
                    }
            }
            IsOnWall = true;
        }
    }
}
