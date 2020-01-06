using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu_Ctrl : MonoBehaviour {

    public GameObject exitornot;
    public GameObject settings;
    public GameObject credits;

    public GameObject Music;
    public GameObject Sound;
    public GameObject Mouse_Sensitivity;

    public GameObject Music_Text;
    public GameObject Sound_Text;
    public GameObject Mouse_Sensitivity_Text;

    public GameObject Game_Button;
    public GameObject Settings_Button;
    public GameObject Credits_Button;
    public GameObject Exit_Button;

    // Use this for initialization
    void Start () {
        Game_Button = GameObject.Find("Game_Button");
        Settings_Button = GameObject.Find("Settings_Button");
        Credits_Button = GameObject.Find("Credits_Button");
        Exit_Button = GameObject.Find("Exit_Button");

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

        exitornot.SetActive(false);
        settings.SetActive(false);
        credits.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Music_Text.GetComponent<Text>().text = GameCtrl.music_num;
        Sound_Text.GetComponent<Text>().text = GameCtrl.sound_num;
        Mouse_Sensitivity_Text.GetComponent<Text>().text = GameCtrl.mouse_sensitivity_num;
    }
    /************************************进入游戏******************************************/
    public void Enter_Game()
    {
        SceneManager.LoadScene(2);
    }
    /************************************设置******************************************/
    public void Open_Settings()
    {
        settings.SetActive(true);
        Game_Button.GetComponent<Button>().enabled = false;
        Settings_Button.GetComponent<Button>().enabled = false;
        Credits_Button.GetComponent<Button>().enabled = false;
        Exit_Button.GetComponent<Button>().enabled = false;
    }

    public void Quit_Settings()
    {
        settings.SetActive(false);
        Game_Button.GetComponent<Button>().enabled = true;
        Settings_Button.GetComponent<Button>().enabled = true;
        Credits_Button.GetComponent<Button>().enabled = true;
        Exit_Button.GetComponent<Button>().enabled = true;
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
    /************************************鸣谢******************************************/
    public void Open_Credits()
    {
        credits.SetActive(true);
        Game_Button.GetComponent<Button>().enabled = false;
        Settings_Button.GetComponent<Button>().enabled = false;
        Credits_Button.GetComponent<Button>().enabled = false;
        Exit_Button.GetComponent<Button>().enabled = false;
    }

    public void Quit_Credits()
    {
        credits.SetActive(false);
        Game_Button.GetComponent<Button>().enabled = true;
        Settings_Button.GetComponent<Button>().enabled = true;
        Credits_Button.GetComponent<Button>().enabled = true;
        Exit_Button.GetComponent<Button>().enabled = true;
    }
    /************************************退出******************************************/
    public void ExitorNot()
    {
        exitornot.SetActive(true);
        Game_Button.GetComponent<Button>().enabled = false;
        Settings_Button.GetComponent<Button>().enabled = false;
        Credits_Button.GetComponent<Button>().enabled = false;
        Exit_Button.GetComponent<Button>().enabled = false;
    }

    public void DoExit()
    {
        Application.Quit();
    }

    public void DonotExit()
    {
        exitornot.SetActive(false);
        Game_Button.GetComponent<Button>().enabled = true;
        Settings_Button.GetComponent<Button>().enabled = true;
        Credits_Button.GetComponent<Button>().enabled = true;
        Exit_Button.GetComponent<Button>().enabled = true;
    }
}
