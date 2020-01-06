using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCtrl : MonoBehaviour
{    
    static public bool JoystickControl;

    static public float music;
    static public float sound;
    static public float mouse_sensitivity;

    static public string music_num;
    static public string sound_num;
    static public string mouse_sensitivity_num;

	void Start ()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(1);

        JoystickControl = false;

        music = 0.5f;
        sound = 0.5f;
        mouse_sensitivity = 0.5f;
    }
	
	void Update ()
    {
        music_num = Mathf.FloorToInt(music * 100).ToString() + '%';
        sound_num = Mathf.FloorToInt(sound * 100).ToString() + '%';
        mouse_sensitivity_num = Mathf.FloorToInt(mouse_sensitivity * 100).ToString() + '%';

        this.GetComponent<AudioSource>().volume = music;

        //Test();
        if(Input.GetKeyDown(KeyCode.P))
        {
            print(JoystickControl);
            if(JoystickControl)
            {
                JoystickControl = false;
            }else
            {
                JoystickControl = true;
            }
        }
    }

    void Test()//测试手柄按键
    {
        if (Input.GetButtonDown("Button A"))
        {
            print("Button A pressed");
        }
        if (Input.GetButtonUp("Button A"))
        {
            print("Button A released");
        }
        if (Input.GetButtonDown("Button B"))
        {
            print("Button B pressed");
        }
        if (Input.GetButtonUp("Button B"))
        {
            print("Button B released");
        }
        if (Input.GetButtonDown("Button X"))
        {
            print("Button X pressed");
        }
        if (Input.GetButtonUp("Button X"))
        {
            print("Button X released");
        }
        if (Input.GetButtonDown("Button Y"))
        {
            print("Button Y pressed");
        }
        if (Input.GetButtonUp("Button Y"))
        {
            print("Button Y released");
        }

        if (Input.GetButtonDown("Button LB"))
        {
            print("Button LB pressed");
        }
        if (Input.GetButtonUp("Button LB"))
        {
            print("Button LB released");
        }
        if (Input.GetButtonDown("Button RB"))
        {
            print("Button RB pressed");
        }
        if (Input.GetButtonUp("Button RB"))
        {
            print("Button RB released");
        }

        if (Input.GetButtonDown("Button Back"))
        {
            print("Button Back pressed");
        }
        if (Input.GetButtonUp("Button Back"))
        {
            print("Button Back released");
        }
        if (Input.GetButtonDown("Button Start"))
        {
            print("Button Start pressed");
        }
        if (Input.GetButtonUp("Button Start"))
        {
            print("Button Start released");
        }

        if (Input.GetAxis("Button LT&RT") == 1)
        {
            print("Button LT pressed");
        }
        if (Input.GetAxis("Button LT&RT") == -1)
        {
            print("Button RT pressed");
        }

        if (Input.GetAxis("Horizontal_Left") == 1)
        {
            print("Left_Joystick Right");
        }
        if (Input.GetAxis("Horizontal_Left") == -1)
        {
            print("Left_Joystick Left");
        }
        if (Input.GetAxis("Vertical_Left") == 1)
        {
            print("Left_Joystick Up");
        }
        if (Input.GetAxis("Vertical_Left") == -1)
        {
            print("Left_Joystick Down");
        }

        if (Input.GetAxis("Horizontal_Right") == 1)
        {
            print("Right_Joystick Right");
        }
        if (Input.GetAxis("Horizontal_Right") == -1)
        {
            print("Right_Joystick Left");
        }
        if (Input.GetAxis("Vertical_Right") == -1)
        {
            print("Right_Joystick Up");
        }
        if (Input.GetAxis("Vertical_Right") == 1)
        {
            print("Right_Joystick Down");
        }

        if (Input.GetAxis("Button Left&Right") == 1)
        {
            print("Button Right pressed");
        }
        if (Input.GetAxis("Button Left&Right") == -1)
        {
            print("Button Left pressed");
        }

        if (Input.GetAxis("Button Up&Down") == 1)
        {
            print("Button Up pressed");
        }
        if (Input.GetAxis("Button Up&Down") == -1)
        {
            print("Button Down pressed");
        }

        if (Input.GetButtonDown("Button Joystick_Left"))
        {
            print("Button Joystick_Left pressed");
        }
        if (Input.GetButtonUp("Button Joystick_Left"))
        {
            print("Button Joystick_Left released");
        }
        if (Input.GetButtonDown("Button Joystick_Right"))
        {
            print("Button Joystick_Right pressed");
        }
        if (Input.GetButtonUp("Button Joystick_Right"))
        {
            print("Button Joystick_Right released");
        }
    }
}
