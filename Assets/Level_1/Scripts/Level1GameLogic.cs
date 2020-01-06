using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Level1GameLogic : MonoBehaviour {
    private GameObject Player;
    public bool HaveFuel = false;
    public bool CanGetFuel = false;
    public bool TrainHaveFuel = false;
    public bool CanOpenTrain = false;
    public bool IsOpenTrain = false;
    private bool HaveIconNoFuel = false;
    

    private GameObject fuel_Icon;
    private GameObject iconLight;
    public GameObject fuel;

    private PlayState lastPlaystate = PlayState.Paused;
    private SceneManager sceneManager;

    private float layerHeight = 1.22f;
    private KeyCode pikeUpKey = KeyCode.E;
    private KeyCode getOnTrainKey = KeyCode.E;
	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        fuel_Icon = GameObject.Find("Fuel_Icon");
        iconLight = GameObject.Find("IconLight");
        sceneManager = new SceneManager();
	}
	
	// Update is called once per frame
	void Update () {
        Collider[] nearPlayer = Physics.OverlapSphere(Player.transform.position, layerHeight);
        if (HaveFuel == false)
        {
            CanGetFuel = false;
            foreach (Collider thing in nearPlayer)
            {
                if (thing.tag == "FinalyThing")
                {
                    //提示可以拿
                    CanGetFuel = true;
                    fuel = thing.gameObject;
                    break;
                }
            }
            if (CanGetFuel == true)
            {
                if (Input.GetKeyDown(pikeUpKey))
                {
                    HaveFuel = true;
                    Fade.fade = true;
                    iconLight.GetComponent<Image>().enabled = true;
                    fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Destroy(fuel);
                }
            }
        }
        foreach (Collider thing in nearPlayer)
        {
            if (thing.tag != "Train")
            {
                if (HaveFuel == false)
                {
                    fuel_Icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.25f);
                }
                else
                {
                    if (fuel_Icon.GetComponent<Image>().enabled == false)
                    {
                        fuel_Icon.GetComponent<Image>().enabled = true;
                    }
                    fuel_Icon.GetComponent<Image>().color = Color.Lerp(fuel_Icon.GetComponent<Image>().color, new Color(1f, 1f, 1f, 1f), Time.deltaTime);
                    if (TrainHaveFuel == false)
                    {
                        HaveIconNoFuel = true;
                        //fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_Icon.GetComponent<RectTransform>().anchoredPosition, new Vector2(817f, 415f), Time.deltaTime);
                    }
                }
            }
            else
            {
                if (HaveFuel == false)
                {
                    if (fuel_Icon.GetComponent<Image>().enabled == false)
                    {
                        fuel_Icon.GetComponent<Image>().enabled = true;
                    }
                    else
                    {
                        fuel_Icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, (0.25f + 0.1f * Mathf.Sin(Time.time * 5)));
                    }
                    if (TrainHaveFuel == false)
                    {
                        HaveIconNoFuel = true;
                        //fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_Icon.GetComponent<RectTransform>().anchoredPosition, new Vector2(817f, 415f), Time.deltaTime);
                    }
                }
                else
                {
                    //提示可以放下
                    //闪烁
                    fuel_Icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, (0.9f + 0.1f * Mathf.Sin(Time.time * 5)));

                    //光效
                    iconLight.GetComponent<Image>().enabled = true;
                    iconLight.transform.rotation = new Quaternion(0f, 0f, iconLight.transform.rotation.y - Mathf.Sin(Time.time), 1);
                    if (Input.GetKeyUp(pikeUpKey))
                    {
                        TrainHaveFuel = true;
                        break;
                    }
                }
            }
        }

        //移动图标
        if (TrainHaveFuel == false)
        {
            if (HaveFuel == true)
            {
                if (fuel_Icon.GetComponent<RectTransform>().anchoredPosition != new Vector2(-817f, 415f))
                {
                    fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_Icon.GetComponent<RectTransform>().anchoredPosition, new Vector2(-817f, 415f), Time.deltaTime);
                    iconLight.transform.rotation = new Quaternion(0f, 0f, iconLight.transform.rotation.y - Mathf.Sin(Time.time), 1);

                    if (iconLight.GetComponent<Image>().enabled == true)
                    {
                        if (fuel_Icon.GetComponent<RectTransform>().anchoredPosition.x > -817f / 4 * 3
                            && fuel_Icon.GetComponent<RectTransform>().anchoredPosition.x < -817f / 3 * 2)
                        {
                            Debug.Log("关闭光线");
                            iconLight.GetComponent<Image>().enabled = false;
                        }
                    }
                }
            }
            else if (HaveIconNoFuel == true)
            {
                if (fuel_Icon.GetComponent<RectTransform>().anchoredPosition != new Vector2(-817f, 415f))
                {
                    fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_Icon.GetComponent<RectTransform>().anchoredPosition, new Vector2(-817f, 415f), Time.deltaTime);
                }
                else
                {
                    HaveIconNoFuel = false;
                }
            }
        }
        //发动小火车
        else if (TrainHaveFuel == true)
        {
            if (fuel_Icon.GetComponent<RectTransform>().anchoredPosition.x > 10f&&
                fuel_Icon.GetComponent<RectTransform>().anchoredPosition.y > 10f)
            {
                fuel_Icon.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_Icon.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, Time.deltaTime);
            }
            //播放烟雾粒子、或者光效、还有声音
            else
            {
                CanOpenTrain = true;
                fuel_Icon.GetComponent<Image>().enabled = false;
                iconLight.GetComponent<Image>().enabled = false;
                //火车灯亮
            }
        }

        if (CanOpenTrain == true)
        {
            if (Input.GetKeyDown(getOnTrainKey))
            {
                //主角上车，播放小火车向前走的动画，播放完加载下一关
                GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().Play();
            }
            if (lastPlaystate == PlayState.Playing
                && GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().state == PlayState.Paused)
            {
                //进入下一关
                SceneManager.LoadScene("level_2");
            }
            else
            {
                lastPlaystate = GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().state;
            }
        }
	}
    
}
