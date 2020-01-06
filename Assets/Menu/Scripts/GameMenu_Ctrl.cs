using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu_Ctrl : MonoBehaviour {

    public GameObject exitornot;

    public GameObject settings;
    public bool settings_open;
    public GameObject Settings_Open;
    public GameObject Settings_Close;

    public Image Settings_Sound;
    public Image Settings_Music;

    public float time;

    public GameObject tips;
    public bool tips_open;
    public GameObject Tips_Open;
    public GameObject Tips_Close;

    public GameObject Music;
    public GameObject Sound;
    public GameObject Mouse_Sensitivity;

    public GameObject Music_Text;
    public GameObject Sound_Text;
    public GameObject Mouse_Sensitivity_Text;

    public GameObject Mini_Map_Camera;
    public int Mini_Map_Camera_Height;

    public GameObject Front_Sight;

    // Use this for initialization
    void Start () {
        Music = GameObject.Find("Music");
        Sound = GameObject.Find("Sound");
        Mouse_Sensitivity = GameObject.Find("Mouse_Sensitivity");

        Music_Text = GameObject.Find("Music_Text");
        Sound_Text = GameObject.Find("Sound_Text");
        Mouse_Sensitivity_Text = GameObject.Find("Mouse_Sensitivity_Text");

        Music.GetComponent<Slider>().value = GameCtrl.music;
        Sound.GetComponent<Slider>().value = GameCtrl.sound;
        Mouse_Sensitivity.GetComponent<Slider>().value = GameCtrl.mouse_sensitivity;

        Music_Text.GetComponent<Text>().text = GameCtrl.music_num;
        Sound_Text.GetComponent<Text>().text = GameCtrl.sound_num;
        Mouse_Sensitivity_Text.GetComponent<Text>().text = GameCtrl.mouse_sensitivity_num;

        Mini_Map_Camera = GameObject.Find("Mini_Map_Camera");
        Mini_Map_Camera_Height = 11;

        Front_Sight = GameObject.Find("Front_Sight");
    }
	
	// Update is called once per frame
	void Update () {

        Music_Text.GetComponent<Text>().text = GameCtrl.music_num;
        Sound_Text.GetComponent<Text>().text = GameCtrl.sound_num;
        Mouse_Sensitivity_Text.GetComponent<Text>().text = GameCtrl.mouse_sensitivity_num;

        if (settings_open)
        {
            settings.transform.localPosition = Vector3.Lerp(settings.transform.localPosition, new Vector3(-860, -1300, 0), 0.1f);
        }
        else
        {
            settings.transform.localPosition = Vector3.Lerp(settings.transform.localPosition, new Vector3(-520, -1300, 0), 0.1f);
        }

        if (tips_open)
        {
            tips.transform.localPosition = Vector3.Lerp(tips.transform.localPosition, new Vector3(-860, -960, 0), 0.1f);
        }
        else
        {
            tips.transform.localPosition = Vector3.Lerp(tips.transform.localPosition, new Vector3(-520, -960, 0), 0.1f);
        }

        Mini_Map_Camera.transform.localPosition = new Vector3(0, Mini_Map_Camera_Height, 0);
    }

    /*************************************设置*******************************************/
    public void Open_Settings()
    {
        settings_open = true;
        Settings_Open.SetActive(false);
        Settings_Close.SetActive(true);
        //time = Time.time;
    }

    public void Quit_Settings()
    {
        settings_open = false;
        Settings_Open.SetActive(true);
        Settings_Close.SetActive(false);
        //time = Time.time;
    }

    public void Music_Ctrl()
    {
        GameCtrl.music = Music.GetComponent<Slider>().value;
    }
    public void Sound_Ctrl()
    {
        GameCtrl.sound = Sound.GetComponent<Slider>().value;
    }
    public void Mouse_Sensitivity_Ctrl()
    {
        GameCtrl.mouse_sensitivity = Mouse_Sensitivity.GetComponent<Slider>().value;
    }
    /************************************任务提示******************************************/
    public void Open_Tips()
    {
        tips_open = true;
        Tips_Open.SetActive(false);
        Tips_Close.SetActive(true);
    }

    public void Close_Tips()
    {
        tips_open = false;
        Tips_Open.SetActive(true);
        Tips_Close.SetActive(false);
    }
    /*************************************退出*******************************************/
    public void ExitorNot()
    {
        exitornot.SetActive(true);
        Front_Sight.SetActive(false);
    }

    public void DoExit()
    {
        SceneManager.LoadScene(1);
    }

    public void DonotExit()
    {
        exitornot.SetActive(false);
        Front_Sight.SetActive(true);
    }
    /************************************小地图*******************************************/
    public void MiniMap_add()
    {
        print("+");
        if(Mini_Map_Camera_Height > 7)
        {
            Mini_Map_Camera_Height--;
        }
    }

    public void MiniMap_minus()
    {
        print("-");
        if (Mini_Map_Camera_Height < 15)
        {
            Mini_Map_Camera_Height++;
        }
    }
}
