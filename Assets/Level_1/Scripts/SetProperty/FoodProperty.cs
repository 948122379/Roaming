using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProperty : MonoBehaviour
{
    private GameObject CanCatchThing ;
    public string tag = "Food";

    //预制体
    [HideInInspector]public GameObject hole;
    [HideInInspector]public Material FoodTrialMaterial;

    //相对水面的高度
    private GameObject Water;
    private float heightOverRiver = 3.8f;
    private float rangefloat = 0.03f;

    //是否在深水中
    private float FloatSinAngle = 0f;
    private bool IsInWater = false;
    //可以漂浮
    private bool IsFloating = false;
    private bool CanInWater = true;

    //是否在树上
    [HideInInspector]public bool IsOnTrees = false;
    private float ColliderR = 0.1f;

    //光晕组件 
    private GameObject Halo_Own;
	// Use this for initialization

    //音频组件
    void Awake()
    {
        //设置父物体
        if (!GameObject.Find("CanCatchThing"))
        {
            CanCatchThing = new GameObject();
            CanCatchThing.name = "CanCatchThing";
        }
        else
        {
            CanCatchThing = GameObject.Find("CanCatchThing");
        }
        transform.parent = CanCatchThing.transform;

        //设置标签
        gameObject.tag = tag;

        //挂上蒋临风写的扔的脚本
        if (!gameObject.GetComponent<Food>()) gameObject.AddComponent<Food>();

        //触发器
        if (!gameObject.GetComponent<SphereCollider>()) gameObject.AddComponent<SphereCollider>();
        //蘑菇上的果子Collioder不一样大
        if (gameObject.name.Length >= "fruit_mushroom".Length && gameObject.name.Substring(0, "fruit_mushroom".Length) == "fruit_mushroom")
        {
            gameObject.GetComponent<SphereCollider>().radius = 1.5f*ColliderR;
        }
        else
        {
            gameObject.GetComponent<SphereCollider>().radius = ColliderR;
        }

        //碰撞
        if (!gameObject.GetComponent<Rigidbody>()) gameObject.AddComponent<Rigidbody>();
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //gameObject.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
        gameObject.GetComponent<Rigidbody>().drag = 5f;
        gameObject.GetComponent<Rigidbody>().angularDrag = 5f;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        //音效
        /*if (!gameObject.GetComponent<AudioSource>()) gameObject.AddComponent<AudioSource>();
        gameObject.GetComponent<AudioSource>().playOnAwake = false;
        gameObject.GetComponent<AudioSource>().loop = false;*/
    }

    void Start()
    {
        //保证与水面高度一致
        Water = GameObject.FindGameObjectWithTag("Water");
        //光晕
        Halo_Own = Instantiate(hole);
        Halo_Own.transform.parent = transform;
        Halo_Own.transform.position = transform.position;
        //Halo.GetType().GetProperty("enabled").SetValue(Halo, false, null);//关闭已有的不可获取的组件
        //拖尾效果
        if (!gameObject.GetComponent<TrailRenderer>()) gameObject.AddComponent<TrailRenderer>();
        gameObject.GetComponent<TrailRenderer>().sharedMaterial = FoodTrialMaterial;
        gameObject.GetComponent<TrailRenderer>().time = 0.5f;
        gameObject.GetComponent<TrailRenderer>().widthMultiplier = 0.3f;
        gameObject.GetComponent<TrailRenderer>().startWidth = 0.3f;
        gameObject.GetComponent<TrailRenderer>().endWidth = 0f;
        gameObject.GetComponent<TrailRenderer>().startColor = new Color(241/255f,1f,0f,1f);
        gameObject.GetComponent<TrailRenderer>().endColor = new Color(1f, 1f, 1f, 0f);
    }

	// Update is called once per frame
	void Update () {
        if (IsFloating == true)
        {
            //控制果子在河里的位置（高度和离小岛的位置）
            transform.Translate(Vector3.Lerp(Vector3.zero, new Vector3(0f, (Water.transform.position.y + heightOverRiver - transform.position.y) + rangefloat * Mathf.Sin(FloatSinAngle), 0f), Time.deltaTime),Space.World);
            FloatSinAngle += Time.deltaTime;
        }
	}

    //掉水里(漂浮)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            //print("掉水里了");
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            
            IsFloating = true;
            PlayerCtrl.Throw_Out = false;
            
        }
        //用枪把果子打下来
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Mechanices")
        {
            //print("打掉");
            IsOnTrees = false;
            if (transform.GetComponent<Rigidbody>()) transform.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            print("掉到地上");
            IsFloating = false;
            GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
        }
    }
}
