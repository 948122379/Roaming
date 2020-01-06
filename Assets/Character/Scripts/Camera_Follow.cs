using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    private GameObject Player;

    private Vector3 CamerPlayerOffset = new Vector3(0f, 1f, 0f);
    private Vector3 FirstPersonCamera_Offset = new Vector3(0f, 0f, 0f);
    private Vector3 ThirdPersonCamera_Offset = new Vector3(-0.1f, 0.05f, -0.2f);

    private GameObject Camera_Y_Axis;
    private GameObject Camera_X_Axis;
    private GameObject MainCamera;
    private GameObject FirstPersonCameraPosition;
    private GameObject ThirdPersonCameraPosition;

    private float distance_back;//第三人称摄像头向后距离

    float mouse_sensitivity = 0.5f;
    float rotationX;
    float rotationX_delta;
    float rotationY;
    float rotationY_delta;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Camera_Y_Axis = GameObject.Find("Camera_Y_Axis");
        Camera_X_Axis = GameObject.Find("Camera_X_Axis");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;
        FirstPersonCameraPosition = GameObject.Find("FirstPersonCameraPosition");
        ThirdPersonCameraPosition = GameObject.Find("ThirdPersonCameraPosition");

        Camera_X_Axis.transform.position = Player.transform.position + Player.transform.TransformDirection(CamerPlayerOffset);
        distance_back = 0.8f;
    }

    void Update()
    {
        Camera_X_Axis.transform.position = Player.transform.position + Player.transform.TransformDirection(CamerPlayerOffset);

        Get_Axis();
        ChangeFirstPersonOrThirdPersonCamera();
        TheCamera_Follow();
    }

    void ChangeFirstPersonOrThirdPersonCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance_back < 2.0f)
            {
                distance_back += 0.1f;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance_back > 1.0f)
            {
                distance_back -= 0.1f;
            }
            else
            {
                distance_back = 0.8f;
            }
        }
    }

    void TheCamera_Follow()
    {
        if (distance_back > 1.0f)//第一人称
        {
            FirstPersonCamera_Follow();
        }
        else//第三人称
        {
            ThirdPersonCamera_Follow();
        }
    }

    void FirstPersonCamera_Follow()
    {
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, FirstPersonCameraPosition.transform.position, Time.deltaTime);
        //print(MainCamera.transform.position + "  " + FirstPersonCameraPosition.transform.position);

        //Vector3 camera_pos = new Vector3(Player.transform.position.x + 0.3f, Player.transform.position.y + 1.2f, Player.transform.position.z + 0.4f);
        //Camera_X_Axis.transform.position = Vector3.Lerp(this.transform.position, camera_pos, Time.deltaTime);

        Camera_X_Axis.transform.Rotate(0, rotationX - rotationX_delta, 0);
        Camera_Y_Axis.transform.Rotate(-(rotationY - rotationY_delta), 0, 0);

        if (Camera_Y_Axis.transform.localEulerAngles.x > 280 || Camera_Y_Axis.transform.localEulerAngles.x < 80)
        {
            Camera_Y_Axis.transform.Rotate(-(rotationY - rotationY_delta), 0, 0);
        }
        else if (Camera_Y_Axis.transform.localEulerAngles.x >= 180 && Camera_Y_Axis.transform.localEulerAngles.x <= 280)
        {
            Camera_Y_Axis.transform.localEulerAngles = new Vector3(280, 0, 0);
        }
        else if (Camera_Y_Axis.transform.localEulerAngles.x >= 80 && Camera_Y_Axis.transform.localEulerAngles.x <= 180)
        {
            Camera_Y_Axis.transform.localEulerAngles = new Vector3(80, 0, 0);
        }

    }

    void ThirdPersonCamera_Follow()
    {

        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, ThirdPersonCameraPosition.transform.position, Time.deltaTime);

        //if (!PlayerCtrl.isWalking)
        //{
        Camera_X_Axis.transform.Rotate(0, rotationX - rotationX_delta, 0);
        // }
        Camera_Y_Axis.transform.Rotate(-(rotationY - rotationY_delta), 0, 0);



        if (Camera_Y_Axis.transform.localEulerAngles.x > 345 || Camera_Y_Axis.transform.localEulerAngles.x < 30)
        {
            Camera_Y_Axis.transform.Rotate(-(rotationY - rotationY_delta), 0, 0);
        }
        else if (Camera_Y_Axis.transform.localEulerAngles.x >= 180 && Camera_Y_Axis.transform.localEulerAngles.x <= 345)
        {
            Camera_Y_Axis.transform.localEulerAngles = new Vector3(345, 0, 0);
        }
        else if (Camera_Y_Axis.transform.localEulerAngles.x >= 30 && Camera_Y_Axis.transform.localEulerAngles.x < 180)
        {
            Camera_Y_Axis.transform.localEulerAngles = new Vector3(30, 0, 0);
        }
    }

    void Get_Axis()
    {
        if (!GameCtrl.JoystickControl)
        {
            rotationX += Input.GetAxis("Mouse X") * mouse_sensitivity;
            rotationY += Input.GetAxis("Mouse Y") * mouse_sensitivity / 2;
        }
        else
        {
            rotationX += Input.GetAxis("Horizontal_Right") * mouse_sensitivity / 5;
            rotationY += Input.GetAxis("Vertical_Right") * -mouse_sensitivity / 10;
        }
        rotationX_delta = Mathf.Lerp(rotationX_delta, rotationX, 0.1f);
        rotationY_delta = Mathf.Lerp(rotationY_delta, rotationY, 0.1f);
    }
}
