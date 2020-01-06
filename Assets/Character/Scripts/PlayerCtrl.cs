using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//拾取那里有增加内容
//可以跳的鸭子标签：Ducka->DuckWaterEat
//标签需要获取Fruits_in_range、Fruit_in_hand、Duckas_in_range、Targeted_Ducka，所以改成public GameObject[] 
//添加上下楼梯OnTriggerStay
//player.transform.Translate(0, 0, speed);这行改成NavMesh移动
//控制碰撞盒大小的都删了，河水太窄，OnTriggerEnter->other.gameObject.tag == "Bank"->Player.GetComponent<CapsuleCollider>().radius = 1.2f;并且编辑器中组件CapsuleCollider->Center.y改成0.3，Height改成2.15

public class PlayerCtrl : MonoBehaviour
{
    const float PI = 3.1415926f;

    private Animator Player_Ctrl;
    private GameObject Player;
    private NavMeshAgent player;

    private GameObject Camera_Direction;

    private GameObject Camera_X_Axis;
    private GameObject Camera_Y_Axis;
    private GameObject Camera;

    static public bool isWalking;
    public bool isWalking1;
    private bool isRunning;

    private bool isJumping;
    private bool Jump;
    static public float Jump_speed;
    public float Jump_Height;
    public float Jump_Length;

    public float walking_speed;
    public float running_speed;
    public float accelerated_speed;

    private float max_speed;
    private float speed;

    float mouse_sensitivity = 0.5f;
    float rotationX;
    float rotationX_delta;

    public Quaternion Target_Rotation;
    public bool Is_Turning;
    public bool Is_Turning2;

    private int Duckas_in_range_num;
    public int Targeted_Ducka;
    private bool Ducka_is_in_range;
    private bool Turn_before_Jump;
    private bool Turn_and_Jump;
    private bool Jump_to_Ducka;
    private bool Jump_to_Bank;
    public GameObject[] Duckas_in_range;

    static public bool isShooting;
    static public bool isShooting_Hook;
    private float time;
    public GameObject Bullet;
    private GameObject Bullet_Start_Pos;
    static public RaycastHit hitInfo;
    Ray ray;
    private GameObject Hook_X_Axis;
    private GameObject Hook_Y_Axis;
    private GameObject Hook_Z_Axis;
    private GameObject Hook_Left;
    private GameObject Hook_Right;
    private GameObject Hook_Back;
    private Vector3 Hook_Start_Pos;
    private bool hook_fly;
    private bool hook_back;

    private bool isThrowing;
    private bool Get_Fruit;
    private bool Fruit_is_in_range;
    private int Fruits_in_range_num;
    public GameObject[] Fruits_in_range;
    public GameObject Fruit_in_hand;
    private GameObject LeftHand;
    private GameObject Fruit_Throw;
    private float Throw_speed;
    static public float Throw_speed_x;
    static public float Throw_speed_y;
    static public bool Throw_Out;

    public GameObject Placement_UI;
    public GameObject Placement;
    static public Vector3 Placement_Pos;
    /// <summary>
    /// ////////////////////////////////////////////////
    /// </summary>
    public AudioClip PickUp;

    public bool Turn_to_Ladder;
    public bool Walk_to_Ladder;
    public bool Start_to_Climb_Ladder;
    public bool Climb_Ladder;
    public bool Get_to_Top;
    public GameObject Ladder;
    public GameObject Ladder_Bottom;
    public GameObject Ladder_Top;

    public bool Go_Up;
    public bool Go_Down;
    /// <summary>
    /// /////////////////////////////////////////////
    /// </summary>
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<NavMeshAgent>();
        Player_Ctrl = Player.GetComponent<Animator>();
        Camera_Direction = GameObject.Find("Camera_Direction");
        Camera_X_Axis = GameObject.Find("Camera_X_Axis");
        Camera_Y_Axis = GameObject.Find("Camera_Y_Axis");
        Camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;

        Bullet_Start_Pos = GameObject.Find("Bullet_Start_Pos");

        Hook_X_Axis = GameObject.Find("Hook_X_Axis");
        Hook_Y_Axis = GameObject.Find("Hook_Y_Axis");
        Hook_Z_Axis = GameObject.Find("Hook_Z_Axis");

        Hook_Left = GameObject.Find("Hook_Left");
        Hook_Right = GameObject.Find("Hook_Right");
        Hook_Back = GameObject.Find("Hook_Back");
        LeftHand = GameObject.Find("LeftHand");

        Placement_UI= GameObject.Find("Placement_UI");

        max_speed = walking_speed;
        speed = 0;

        Target_Rotation = Quaternion.Euler(0, 0, 0);
        Is_Turning = false;
        isJumping = false;
        Jump = false;

        Duckas_in_range = new GameObject[10];
        Duckas_in_range_num = 0;
        Targeted_Ducka = 0;
        Turn_before_Jump = false;
        Turn_and_Jump = false;
        Jump_to_Ducka = false;

        isShooting = false;
        isThrowing = false;

        Fruits_in_range = new GameObject[10];
        Fruits_in_range_num = 0;
        Fruit_in_hand = null;
        Fruit_Throw = null;

        Throw_speed_y = 0.25f;
        //////////////////////////////////////////
        Turn_to_Ladder = false;
        Walk_to_Ladder = false;
        Start_to_Climb_Ladder = false;
        Climb_Ladder = false;
        Get_to_Top = false;

        Go_Up = false;
        Go_Down = false;
    }

    void Update()
    {
        if (!Jump)
        {
            if (player.enabled)
            {
                if (!isThrowing && !isShooting && !isShooting_Hook)
                {
                    //player.transform.Translate(0, 0, speed);
                    player.SetDestination(transform.position + transform.forward * speed * 4f);
                    Walk_ahead();
                    Walk_left();
                    Walk_right();
                    Walk_back();
                    Walk_leftfront();
                    Walk_leftback();
                    Walk_rightfront();
                    Walk_rightback();
                    Running();
                }
            }
        }
        Turning();
        Shoot();
        Catch_Fruit();
        Throw();
        Jumping();
        Climb();
        /*********************************************************************************************************************************/

        if (isWalking)
        {
            if (!isShooting && !isThrowing)
            {
                if (!GameCtrl.JoystickControl)
                {
                    rotationX += Input.GetAxis("Mouse X") * mouse_sensitivity;
                }
                rotationX_delta = Mathf.Lerp(rotationX_delta, rotationX, 0.1f);
                Player.transform.Rotate(0, rotationX - rotationX_delta, 0);
            }
        }
        else
        {
            Camera_Direction.transform.position = Camera_X_Axis.transform.position;
            Camera_Direction.transform.rotation = Quaternion.Euler(0, Camera_X_Axis.transform.eulerAngles.y, 0);
        }
        if (isShooting_Hook)
        {
            Camera_Direction.transform.position = Camera_X_Axis.transform.position;
            Camera_Direction.transform.rotation = Quaternion.Euler(0, Camera_X_Axis.transform.eulerAngles.y, 0);
        }
        if (isThrowing || isShooting)
        {
            isWalking = false;
        }
        isWalking1 = isWalking;
        if (isThrowing || isShooting)
        {
            if (!isWalking)
            {
                if (!GameCtrl.JoystickControl)
                {
                    rotationX += Input.GetAxis("Mouse X") * mouse_sensitivity;
                }
                rotationX_delta = Mathf.Lerp(rotationX_delta, rotationX, 0.1f);
                Player.transform.Rotate(0, rotationX - rotationX_delta, 0);
            }
        }
        ray = Camera.GetComponent<Camera>().ScreenPointToRay(Front_Sight.Target_Pos);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //划出射线，只有在scene视图中才能看到
            Debug.DrawLine(ray.origin, hitInfo.point);
            //print(hitInfo.point);
        }

        //Hook_Start_Pos = GameObject.Find("Hook_Start_Pos").transform.position;

        //Hook_Back.GetComponent<LineRenderer>().SetPosition(0, Hook_Back.transform.position);
        //Hook_Back.GetComponent<LineRenderer>().SetPosition(1, Hook_Start_Pos);

        if (Fruit_in_hand != null)
        {
            Fruit_in_hand.transform.position = LeftHand.transform.position;
            Fruit_in_hand.transform.rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
        }
        
        if(Camera.transform.eulerAngles.x>300)
        {
            Throw_speed_x = 0.08f - (Camera.transform.eulerAngles.x - 360) * 0.001f;
        }
        else
        {
            Throw_speed_x = 0.08f - (Camera.transform.eulerAngles.x) * 0.001f;
        }
        if (Placement_UI)
        {
            Placement_UI.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    /*****************************************************前进************************************************************************/
    void Walk_ahead()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            {
                isWalking = false;
                Player_Ctrl.SetBool("IsWalking", false);
                Player_Ctrl.SetBool("IsRunning", false);
            }
        }
    }
    /*****************************************************向左************************************************************************/
    void Walk_left()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                Player_Ctrl.SetBool("IsWalking", false);
                Player_Ctrl.SetBool("IsRunning", false);
                isWalking = false;
            }
        }
    }
    /*****************************************************向右************************************************************************/
    void Walk_right()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                Player_Ctrl.SetBool("IsWalking", false);
                Player_Ctrl.SetBool("IsRunning", false);
                isWalking = false;
            }
        }
    }
    /*****************************************************向后************************************************************************/
    void Walk_back()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 180, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
            {
                Player_Ctrl.SetBool("IsWalking", false);
                Player_Ctrl.SetBool("IsRunning", false);
                isWalking = false;
            }
        }
    }
    /*****************************************************左前************************************************************************/
    void Walk_leftfront()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
    }
    /*****************************************************左后************************************************************************/
    void Walk_leftback()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 180, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
    }
    /*****************************************************右前************************************************************************/
    void Walk_rightfront()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
    }
    /*****************************************************右后************************************************************************/
    void Walk_rightback()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 180, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y - 135, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 90, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y + 45, 0);
                Is_Turning = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }
    }
    /*****************************************************跑步************************************************************************/
    void Running()
    {
        if (isWalking)
        {
            speed = Mathf.Lerp(speed, max_speed, accelerated_speed);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
                max_speed = running_speed;
                Player_Ctrl.SetBool("IsRunning", true);
            }
            else
            {
                isRunning = true;
                max_speed = walking_speed;
                Player_Ctrl.SetBool("IsRunning", false);
            }
        }
        else
        {
            speed = Mathf.Lerp(speed, 0, accelerated_speed);
        }
    }
    /*****************************************************转身************************************************************************/
    void Turning()
    {
        if (Is_Turning)
        {
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Target_Rotation, 0.1f);
            if (Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y > -1)
            {
                Is_Turning = false;

                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
                {
                    Player_Ctrl.SetBool("IsWalking", false);
                    isWalking = false;
                }
            }
        }
    }
    /*****************************************************跳跃************************************************************************/
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!Ducka_is_in_range)
            {
                isWalking = false;
                isRunning = false;

                Player_Ctrl.SetBool("IsWalking", false);
                Player_Ctrl.SetBool("IsRunning", false);
                Player_Ctrl.SetBool("IsJumping", true);
                Player.GetComponent<NavMeshAgent>().enabled = false;
                Jump = true;
            }
            else
            {
                float x = Player.transform.position.x - Duckas_in_range[Targeted_Ducka].transform.position.x;
                float z = Player.transform.position.z - Duckas_in_range[Targeted_Ducka].transform.position.z;
                if (z > 0)
                {
                    float angle = Mathf.Atan(x / z) * 180 / PI;
                    Target_Rotation = Quaternion.Euler(0, angle + 180, 0);
                }
                else if (z < 0)
                {
                    float angle = Mathf.Atan(x / z) * 180 / PI;
                    Target_Rotation = Quaternion.Euler(0, angle, 0);
                }
                Is_Turning = true;
                Turn_before_Jump = true;
            }
        }

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    isJumping=false;
        //    Jump = false;
        //    Player_Ctrl.SetBool("IsJumping", false);
        //}
            if (Turn_before_Jump)
        {
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Target_Rotation, 0.1f);
            if (Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y > -1)
            {
                Turn_before_Jump = false;
                Turn_and_Jump = true;
                if (Duckas_in_range[Targeted_Ducka].tag == "DuckWaterEat")
                {
                    Jump_to_Ducka = true;
                    Jump_to_Bank = false;
                }
                else if (Duckas_in_range[Targeted_Ducka].tag == "Bank")
                {
                    Jump_to_Bank = true;
                    Jump_to_Ducka = false;
                }
            }
        }
        if (Turn_and_Jump)
        {
            isWalking = false;
            isRunning = false;

            Player_Ctrl.SetBool("IsWalking", false);
            Player_Ctrl.SetBool("IsRunning", false);
            Player_Ctrl.SetBool("IsJumping", true);
            Player.GetComponent<NavMeshAgent>().enabled = false;

            Vector3 a = new Vector3(Player.transform.position.x, 0, Player.transform.position.z);
            Vector3 b = new Vector3(Duckas_in_range[Targeted_Ducka].transform.position.x, 0, Duckas_in_range[Targeted_Ducka].transform.position.z);
            Jump_Length = Vector3.Distance(a, b) * Time.deltaTime * 2;
            Jump = true;
        }
        if (isJumping)
        {
            Player.transform.Translate(0, (Jump_speed - Time.time) * Jump_Height, Jump_Length);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (!Player_Ctrl.GetBool("IsJumping"))
            {
                isWalking = true;
                Player_Ctrl.SetBool("IsWalking", true);
            }
        }

        Count_Duckas_in_range();
    }

    void Count_Duckas_in_range()
    {
        for (int i = 1; i < Duckas_in_range.Length; i++)
        {
            if (Duckas_in_range[i] != null)
            {
                if (Duckas_in_range[i - 1] == null)
                {
                    Duckas_in_range[i - 1] = Duckas_in_range[i];
                    Duckas_in_range[i] = null;
                }
            }
        }
        for (int i = 1; i < Duckas_in_range.Length; i++)
        {
            if (Duckas_in_range[i] == null)
            {
                if (Duckas_in_range[i - 1] != null)
                {
                    Duckas_in_range_num = i;
                }
            }
        }
        if (Duckas_in_range_num > 0)
        {
            Ducka_is_in_range = true;
            if (Duckas_in_range_num == 1)
            {
                //Duckas_in_range[0].transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = true;
            }
            if (Duckas_in_range_num > 1)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {

                    if (Targeted_Ducka < Duckas_in_range_num - 1)
                    {
                        Targeted_Ducka += 1;
                        //Duckas_in_range[Targeted_Ducka].transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = true;
                        //Duckas_in_range[Targeted_Ducka - 1].transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        Targeted_Ducka = 0;
                        //Duckas_in_range[Targeted_Ducka].transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = true;
                        //Duckas_in_range[Duckas_in_range_num - 1].transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
        }
        else
        {
            Ducka_is_in_range = false;
            Targeted_Ducka = 0;
        }
    }

    void Man_Jump()
    {
        Player_Ctrl.SetBool("IsJumping", false);
        isJumping = true;
        Jump_speed = Time.time + 0.25f;
        Turn_and_Jump = false;
    }
    void Man_Land()
    {
        Player_Ctrl.SetBool("IsJumping", false);
        isJumping = false;
        Jump = false;
        if (Jump_to_Bank)
        {
            Player.GetComponent<NavMeshAgent>().enabled = true;
        }
        else if (!Jump_to_Ducka)
        {
            Player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
    /*****************************************************射击************************************************************************/
    void Shoot()
    {
        /*****************************************************子弹************************************************************************/
        if (Input.GetMouseButtonDown(0))
        {
            if (!isThrowing)
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);

                isShooting = true;
                Is_Turning = false;
                Is_Turning2 = true;
                isWalking = false;
            }
        }
        if(isShooting)
        {
            Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
            Player_Ctrl.SetBool("IsShooting", true);
            if (Player.transform.eulerAngles.y - Camera.transform.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Camera.transform.eulerAngles.y > -1)
            {
                Is_Turning2 = false;
            }
            if(Is_Turning2)
            {
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Target_Rotation, 0.1f);
            }
        }
        //if (Input.GetMouseButton(0))
        //{
        //    if (!isThrowing)
        //    {
        //        isShooting = true;
        //        Player_Ctrl.SetBool("IsShooting", true);
        //        Is_Turning = false;
        //        isWalking = false;
        //        Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Target_Rotation, 0.1f);
        //        //if (Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y > -1)
        //        //{
        //        //    Is_Turning = true;
        //        //}
        //    }
        //}
        if(Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            Player_Ctrl.SetBool("IsShooting", false);
        }
        /*****************************************************夹子************************************************************************/
        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (!isThrowing)
        //    {
        //        isShooting_Hook = true;
        //        Player_Ctrl.SetBool("IsShooting_Hook", true);
        //
        //        Target_Rotation = Quaternion.Euler(0, Camera_Direction.transform.eulerAngles.y, 0);
        //        Is_Turning = true;
        //    }
        //}
        //Hook_Flying();
    }

    void Bullet_Fly()
    {
        GameObject bullet = Instantiate(Bullet, Bullet_Start_Pos.transform.position, Camera.transform.rotation);
        Front_Sight.recoil += 2f;
    }

    void Hook_Fly()
    {
        Hook_Back.GetComponent<LineRenderer>().enabled = true;
        Hook_X_Axis.transform.parent = null;
        hook_fly = true;
        Hook_X_Axis.transform.rotation = Quaternion.Euler(Camera.transform.eulerAngles.x, Camera.transform.eulerAngles.y, Camera.transform.eulerAngles.z);
        Hook_Y_Axis.transform.localRotation = Quaternion.Euler(Camera.transform.eulerAngles.x+90, -90, 0);
        Hook_Z_Axis.transform.localRotation = Quaternion.Euler(0, Camera_X_Axis.transform.eulerAngles.y, 0);
    }

    void Hook_Flying()
    {
        if (hook_fly)
        {
            if (!hook_back)
            {
                if (Vector3.Distance(Hook_X_Axis.transform.position, Hook_Start_Pos) < 5)
                {
                    Hook_X_Axis.transform.Translate(-0.01f, 0.01f, 0.2f);
                }
                else
                {
                    hook_back = true;
                }
            }
            if (hook_back)
            {
                Hook_Start_Pos = GameObject.Find("Hook_Start_Pos").transform.position;
                Hook_X_Axis.transform.parent = GameObject.Find("GUN1GUN1").transform;
                Hook_X_Axis.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Hook_Y_Axis.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Hook_X_Axis.transform.localScale = new Vector3 (1, 1, 1);
                if (Vector3.Distance(Hook_X_Axis.transform.position, Hook_Start_Pos) < 0.01f)
                {
                    hook_fly = false;
                    hook_back = false;
                    Hook_X_Axis.transform.position = Hook_Start_Pos;
                    isShooting_Hook = false;
                    Hook_Back.GetComponent<LineRenderer>().enabled = false;
                    Player_Ctrl.SetBool("IsShooting_Hook", false);        
                }
                else
                {
                    Hook_X_Axis.transform.position = Vector3.MoveTowards(Hook_X_Axis.transform.position, Hook_Start_Pos, 0.1f);
                }
            }
        }
    }
    /*****************************************************拾取************************************************************************/
    void Catch_Fruit()
    {
        if (!Get_Fruit)
        {
            if (Fruit_is_in_range)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Fruit_in_hand= Fruits_in_range[0];

                   ////加入
                    if (Fruits_in_range[0])
                    {
                        Fruits_in_range[0].GetComponent<FoodProperty>().IsOnTrees = false;
                        if (Fruits_in_range[0].GetComponent<Rigidbody>()) Fruits_in_range[0].GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
                    }
                    //加入

                    Fruits_in_range[0] = null;
                    Get_Fruit = true;
                }
            }
        }

        Count_Fruits_in_range();
    }

    void Count_Fruits_in_range()
    {
        for (int i = 1; i < Fruits_in_range.Length; i++)
        {
            if (Fruits_in_range[i] != null)
            {
                if (Fruits_in_range[i - 1] == null)
                {
                    Fruits_in_range[i - 1] = Fruits_in_range[i];
                    Fruits_in_range[i] = null;
                }
            }
        }
        for (int i = 1; i < Fruits_in_range.Length; i++)
        {
            if (Fruits_in_range[i] == null)
            {
                if (Fruits_in_range[i - 1] != null)
                {
                    Fruits_in_range_num = i;
                }
            }
        }
        if (Fruits_in_range_num > 0)
        {
            Fruit_is_in_range = true;
            if (Fruits_in_range_num > 1)
            {
                for (int i = 0; i < Fruits_in_range_num - 1; i++)
                {
                    for (int j = 0; j < Fruits_in_range_num - 1 - i; j++)
                    {
                        if (Vector3.Distance(Fruits_in_range[j].transform.position, Player.transform.position)
                            > Vector3.Distance(Fruits_in_range[j + 1].transform.position, Player.transform.position))
                        {
                            GameObject temp = Fruits_in_range[j];
                            Fruits_in_range[j] = Fruits_in_range[j + 1];
                            Fruits_in_range[j + 1] = temp;
                        }
                    }
                }
            }
        }
        else
        {
            Fruit_is_in_range = false;
        }
    }
    /*****************************************************投掷************************************************************************/
    void Throw()
    {
        if (Get_Fruit)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isShooting)
                {
                    Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);

                    isThrowing = true;
                    Is_Turning = false;
                    Is_Turning2 = true;
                    isWalking = false;
                    Placement_UI.SetActive(true);
                }
            }
            if (isThrowing)
            {
                Target_Rotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);

                Player_Ctrl.SetBool("IsThrowing", true);
                if (Player.transform.eulerAngles.y - Camera.transform.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Camera.transform.eulerAngles.y > -1)
                {
                    Is_Turning2 = false;
                }
                if (Is_Turning2)
                {
                    Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Target_Rotation, 0.1f);
                }
            }
            //if (!isShooting)
            //{
            //    if (Input.GetKey(KeyCode.F))
            //    {
            //        isThrowing = true;
            //        Player_Ctrl.SetBool("IsThrowing", true);
            //
            //        Target_Rotation = Quaternion.Euler(0, Camera_Direction.transform.eulerAngles.y, 0);
            //        Is_Turning = true;
            //    }
            //}
            if (Input.GetKeyUp(KeyCode.F))
            {
                Player_Ctrl.speed = 1;
                isThrowing = false;
                Player_Ctrl.SetBool("IsThrowing", false);
                isWalking = false;
                isRunning = false;
                Fruit_Throw = Fruit_in_hand;
                Fruit_in_hand = null;
                Get_Fruit = false;
                Throw_Out = true;
                Throw_speed = Time.time + 0.25f;
                Placement_UI.SetActive(false);
            }

            Find_Placement();
        }
        if (Throw_Out)
        {
            if (Fruit_Throw)
            {
                Fruit_Throw.transform.Translate(0, (Throw_speed - Time.time) * Throw_speed_y, Throw_speed_x);
            }
        }
    }
    void ReadytoThrow()
    {
        if (Get_Fruit)
        {
            if (Input.GetKey(KeyCode.F))
            {
                Player_Ctrl.speed = 0;
                Draw_Placement.ReadytoThrow = true;
            }
        }
    }
    /*****************************************************落点************************************************************************/
    void Find_Placement()
    {
        if(Get_Fruit)
        {
            if(Input.GetKey(KeyCode.F))
            {
                if (time < Time.time)
                {
                  GameObject eee = Instantiate(Placement, LeftHand.transform.position, Fruit_in_hand.transform.rotation);
                  time += 0.02f;
                }
            }
        }
        Placement_UI.transform.position = Placement_Pos;
    }
    /*****************************************************爬梯************************************************************************/
    void Climb()
    {
        if (!Turn_to_Ladder)
        {
            if (Ladder_Bottom != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (GameObject.Find("button_1").GetComponent<StairsSwitch>().IsOnSwitch == true
                        &&GameObject.Find("button_2").GetComponent<StairsSwitch>().IsOnSwitch == true
                        && GameObject.Find("button_3").GetComponent<StairsSwitch>().IsOnSwitch == true)
                    {
                        Turn_to_Ladder = true;
                    }
                }
            }
        }
        if (Turn_to_Ladder)
        {
            Player_Ctrl.SetBool("IsWalking", false);
            Player_Ctrl.SetBool("IsRunning", false);
            Player_Ctrl.SetBool("IsJumping", false);

            float x = Player.transform.position.x - Ladder_Bottom.transform.position.x;
            float z = Player.transform.position.z - Ladder_Bottom.transform.position.z;
            if (z > 0)
            {
                float angle = Mathf.Atan(x / z) * 180 / PI;
                Target_Rotation = Quaternion.Euler(0, angle + 180, 0);
            }
            else if (z < 0)
            {
                float angle = Mathf.Atan(x / z) * 180 / PI;
                Target_Rotation = Quaternion.Euler(0, angle, 0);
            }
            Is_Turning = true;
            if (Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y > -1)
            {
                Turn_to_Ladder = false;
                Walk_to_Ladder = true;
            }
        }

        if (Walk_to_Ladder)
        {
            Player_Ctrl.SetBool("IsWalking", true);
            Vector3 a = new Vector3(Ladder_Bottom.transform.position.x, player.transform.position.y, Ladder_Bottom.transform.position.z);
            player.transform.position = Vector3.MoveTowards(player.transform.position, a, 0.1f);
            if (player.transform.position.x - Ladder_Bottom.transform.position.x < 0.1f &&
                player.transform.position.x - Ladder_Bottom.transform.position.x > -0.1f &&
                player.transform.position.z - Ladder_Bottom.transform.position.z < 0.1f &&
                player.transform.position.z - Ladder_Bottom.transform.position.z > -0.1f
                )
            {
                Walk_to_Ladder = false;
                Start_to_Climb_Ladder = true;
            }
        }

        if (Start_to_Climb_Ladder)
        {
            Player_Ctrl.SetBool("IsWalking", false);
            Player_Ctrl.SetBool("IsRunning", false);
            Player_Ctrl.SetBool("IsJumping", false);

            float x = Ladder_Bottom.transform.position.x - Ladder.transform.position.x;
            float z = Ladder_Bottom.transform.position.z - Ladder.transform.position.z;

            if (z > 0)
            {
                float angle = Mathf.Atan(x / z) * 180 / PI;
                Target_Rotation = Quaternion.Euler(0, angle + 180, 0);
            }
            else if (z < 0)
            {
                float angle = Mathf.Atan(x / z) * 180 / PI;
                Target_Rotation = Quaternion.Euler(0, angle, 0);
            }
            Is_Turning = true;

            if (Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y < 1 && Player.transform.eulerAngles.y - Target_Rotation.eulerAngles.y > -1)
            {
                Start_to_Climb_Ladder = false;
                Climb_Ladder = true;
            }
        }

        if (Climb_Ladder)
        {
            Player.GetComponent<NavMeshAgent>().enabled = false;

            Player_Ctrl.SetBool("IsWalking", false);
            Player_Ctrl.SetBool("IsClimbing_Ladder", true);

            player.transform.position = Vector3.MoveTowards(player.transform.position, Ladder_Top.transform.position, 0.05f);
            if (player.transform.position.x - Ladder_Top.transform.position.x < 0.1f &&
                player.transform.position.x - Ladder_Top.transform.position.x > -0.1f &&
                player.transform.position.z - Ladder_Top.transform.position.z < 0.1f &&
                player.transform.position.z - Ladder_Top.transform.position.z > -0.1f
                )
            {
                Climb_Ladder = false;
                Get_to_Top = true;
            }
        }
        if (Get_to_Top)
        {
            Player.GetComponent<NavMeshAgent>().enabled = true;
            Get_to_Top = false;
            Player_Ctrl.SetBool("IsClimbing_Ladder", false);
            Ladder = null;
            Ladder_Bottom = null;
            Ladder_Top = null;
        }
    }
    /*********************************************************************************************************************************/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DuckWaterEat" || other.gameObject.tag == "Bank")
        {
            Duckas_in_range[Duckas_in_range_num] = other.gameObject;
        }

        if (other.gameObject.tag == "Bank")
        {
            //Player.GetComponent<CapsuleCollider>().radius = 1f;
        }

        if (other.gameObject.tag == "Food")
        {
            Fruits_in_range[Fruits_in_range_num] = other.gameObject;
        }
        //if (other.gameObject.tag == "Ladder")
        //{
        //    Ladder = other.gameObject;
        //}
        //if (other.gameObject.name == "Ladder_Bottom")
        //{
        //    Ladder_Bottom = other.gameObject;
        //}
        //if (other.gameObject.name == "Ladder_Top")
        //{
        //    Ladder_Top = other.gameObject;
        //}
        if (other.gameObject.tag == "LadderBottom")
        {
            //if (Ladder_Bottom == null)
            //{
                Ladder_Bottom = other.gameObject;
                //if (Ladder_Top == null)
                //{
                    Ladder_Top = Ladder_Bottom.transform.Find("Top").gameObject;
                //}
                //if (Ladder == null)
                //{
                    Ladder = Ladder_Bottom.transform.Find("Ladder").gameObject;
                //}
            //}
        }


        //if (other.gameObject.tag == "TopBottom")
        //{
        //    Climb_Ladder = false;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        ////上楼梯
        //if (other.gameObject.tag == "LadderBottom")
        //{
        //    //if (GameObject.Find("button_" + other.transform.name.Substring(12, 1)).GetComponent<StairsSwitch>().IsOnSwitch == true)
        //    //{
        //        if (Input.GetKeyUp(KeyCode.R))
        //        {
        //            Climb_Ladder = true;
        //            Ladder = other.gameObject;
        //            //player.speed = 1f;
        //            //player.angularSpeed = 180f;
        //            //other.gameObject.GetComponent<OffMeshLink>().startTransform = other.gameObject.transform;
        //            player.SetDestination(other.transform.Find("Top").position);
        //            //player.speed = 7f;
        //            //player.angularSpeed = 30f;
        //            //other.gameObject.GetComponent<OffMeshLink>().startTransform = null;
        //            print("上楼");
        //        }
        //    //}
        //}
        //else if (other.gameObject.tag == "TopBottom")
        //{
        //    if (GameObject.Find("button_" + other.transform.parent.name.Substring(12, 1)).GetComponent<StairsSwitch>().IsOnSwitch == true)
        //    {
        //        if (Input.GetKeyUp(KeyCode.R))
        //        {
        //            Climb_Ladder = true;
        //            //player.speed = 0.1f;
        //            //player.angularSpeed = 180f;
        //            //other.gameObject.GetComponent<OffMeshLink>().startTransform = other.gameObject.transform.parent.transform;
        //            player.SetDestination(other.transform.parent.position);
        //            //player.speed = 7f;
        //            //player.angularSpeed = 30f;
        //            //other.gameObject.GetComponent<OffMeshLink>().startTransform = null;
        //            print("下楼");
        //        }
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "DuckWaterEat" || other.gameObject.tag == "Bank")
        {
            for (int i = 0; i < Duckas_in_range.Length; i++)
            {
                if (other.gameObject == Duckas_in_range[i])
                {
                    Duckas_in_range[i] = null;
                    Duckas_in_range_num--;
                    //other.transform.Find("Sphere").GetComponent<MeshRenderer>().enabled = false;
                }
            }
            if (Targeted_Ducka == Duckas_in_range_num)
            {
                Targeted_Ducka -= 1;
            }
        }

        if (other.gameObject.tag == "Bank")
        {
            //Player.GetComponent<CapsuleCollider>().radius = 0.6f;
        }

        if (other.gameObject.tag == "Food")
        {
            for (int i = 0; i < Fruits_in_range.Length; i++)
            {
                if (other.gameObject == Fruits_in_range[i])
                {
                    Fruits_in_range[i] = null;
                    Fruits_in_range_num--;
                }
            }
        }
        //if (other.gameObject.tag == "Ladder")
        //{
        //    Ladder = null;
        //}
        //
        //if (other.gameObject.name == "Ladder_Bottom")
        //{
        //    Ladder_Bottom = null;
        //}
        //
        //if (other.gameObject.name == "Ladder_Top")
        //{
        //    Ladder_Top = null;
        //}
    }
}