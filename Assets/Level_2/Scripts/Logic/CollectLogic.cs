using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class CollectLogic
{
    private GameObject Player;
    private GameObject MainCamera;

    private GameObject fuel_Icon;
    private GameObject fuel_Num;
    private GameObject iconLight;
    private GameObject fuel_IconAnima;
    private GameObject[] stars;

    private bool playGetAniam = false;
    public bool starEnough = false;
    public bool trainGetfuel = false;
    public bool IsPlayerOverAnimation = false;
    private PlayState addFuelLastPlaystate = PlayState.Paused;
    private PlayState lastPlaystate = PlayState.Paused;

    private float layerHeight = 1.22f;
    private KeyCode pikeUpKey = KeyCode.E;
    private KeyCode getOnTrainKey = KeyCode.E;
    public int StartNum = 0;
    private int addFuelNum = 0;
    public int AllStarNum = 10;
    // Use this for initialization
    public void Init()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        fuel_Icon = GameObject.Find("Fuel_Icon");
        fuel_Num = GameObject.Find("fuel_Num");
        iconLight = GameObject.Find("IconLight");
        fuel_IconAnima = GameObject.Find("Fuel_IconAnima");

        stars = GameObject.FindGameObjectsWithTag("FinalyThing");
    }

    // Update is called once per frame
    public void Update()
    {
        if (StartNum == AllStarNum)
        {
            GameObject.Destroy(GameObject.Find("NoEnter"));
        }

        if (Player == null || MainCamera == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        else
        {
            Collider[] nearPlayer = Physics.OverlapSphere(Player.transform.position, layerHeight);
            foreach (Collider thing in nearPlayer)
            {
                
                if (thing.tag == "FinalyThing")
                {
                    Vector3 posInCamera = MainCamera.GetComponent<Camera>().WorldToScreenPoint(thing.transform.position);
                    if (posInCamera != null)
                    {
                        //提示可以拿
                        if (Input.GetKeyDown(pikeUpKey))
                        {
                            playGetAniam = false;
                            StartNum++;
                            Object.Destroy(thing.gameObject);
                            fuel_IconAnima.GetComponent<Image>().enabled = true;
                            iconLight.GetComponent<Image>().enabled = true;
                            fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                            playGetAniam = true;
                        }
                    }
                    break;
                }

                if (thing.tag == "Train")
                {
                    if (starEnough == true)
                    {
                        if (trainGetfuel == false)
                        {
                            /////提示加油
                            if (Input.GetKeyDown(getOnTrainKey))
                            {
                                //加油动画
                                GameObject.Find("AddFuelAnimation").GetComponent<PlayableDirector>().Play();
                                fuel_Num.GetComponent<Text>().text = (StartNum - (addFuelNum + 1)).ToString();
                            }
                            if (addFuelNum < stars.Length)
                            {
                                if (addFuelLastPlaystate == PlayState.Playing
                                     && GameObject.Find("AddFuelAnimation").GetComponent<PlayableDirector>().state == PlayState.Paused)
                                {
                                    addFuelNum++;
                                    if (addFuelNum < stars.Length)
                                    {
                                        GameObject.Find("AddFuelAnimation").GetComponent<PlayableDirector>().Play();
                                        fuel_Num.GetComponent<Text>().text = (StartNum - (addFuelNum + 1)).ToString();
                                    }
                                }
                                else
                                {
                                    addFuelLastPlaystate = GameObject.Find("AddFuelAnimation").GetComponent<PlayableDirector>().state;
                                }
                            }
                            else
                            {
                                trainGetfuel = true;
                            }
                        }
                        else
                        {
                            /////提示上车
                            if (Input.GetKeyDown(getOnTrainKey))
                            {
                                GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().Play();
                            }
                            if (lastPlaystate == PlayState.Playing
                                && GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().state == PlayState.Paused)
                            {
                                //回到开始界面
                                Debug.Log("结束");
                                IsPlayerOverAnimation = true;
                                //SceneManager.LoadScene("level_2");
                            }
                            else
                            {
                                lastPlaystate = GameObject.Find("OverAnimation").GetComponent<PlayableDirector>().state;
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }

            if (playGetAniam == true)
            {
                fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition, fuel_Icon.GetComponent<RectTransform>().anchoredPosition, Time.deltaTime);
                if (Vector2.Distance(fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition, fuel_Icon.GetComponent<RectTransform>().anchoredPosition) < 10)
                {
                    if (Mathf.Abs(fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition.x - fuel_Icon.GetComponent<RectTransform>().anchoredPosition.x) > 8
                        && Mathf.Abs(fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition.x - fuel_Icon.GetComponent<RectTransform>().anchoredPosition.x) < 12)
                    {
                        iconLight.GetComponent<Image>().enabled = false;
                        fuel_Num.GetComponent<Text>().text = StartNum.ToString();
                        playGetAniam = false;
                    }
                }
                iconLight.transform.rotation = new Quaternion(0f, 0f, iconLight.transform.rotation.y - Mathf.Sin(Time.time), 1);
            }
            else
            {
                if (iconLight)
                {
                    iconLight.GetComponent<Image>().enabled = false;
                }
                fuel_IconAnima.GetComponent<RectTransform>().anchoredPosition = fuel_Icon.GetComponent<RectTransform>().anchoredPosition;
            }

            if (StartNum == stars.Length)
            {
                starEnough = true;
            }
            else
            {
                starEnough = false;
            }
        }
    }
}
