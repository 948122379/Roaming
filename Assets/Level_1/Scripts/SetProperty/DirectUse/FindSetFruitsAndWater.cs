using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//放在包含果子等的父物体上
public class FindSetFruitsAndWater : MonoBehaviour
{
    //获取预制体
    public GameObject hole;
    public Material FoodTrialMaterial;

    //设置所有果子可拾取
    private ArrayList Fruits;
    private Transform[] AllChilds;
    private GameObject Halo;
    
    //物体
    private GameObject Water;

    //设置标签
    private string Terriantag = "Terrian";
    private string WaterTag = "Water";

	// Use this for initialization
	void Awake () {
        //搜索水果，并加组件，名字：fruit_mushroom/fruit_plant/
        Fruits = new ArrayList();
        AllChilds = GetComponentsInChildren<Transform>();
        foreach (Transform child in AllChilds)
        {
            //模型Mesh用的总场景的，这里要更改
            if (child.gameObject.name.Length >= "fruit_mushroom".Length)
            {
                if (child.gameObject.name.Substring(0, "fruit_mushroom".Length) == "fruit_mushroom")
                {
                    Fruits.Add(child);
                    child.gameObject.AddComponent<FoodProperty>();
                }
                else if (child.gameObject.name.Substring(0, "fruit_plant".Length) == "fruit_plant")
                {
                    Fruits.Add(child);
                    child.gameObject.AddComponent<FoodProperty>();
                }
            }
            else if (child.gameObject.name.Length >= "fruit_plant".Length)
            {
                if (child.gameObject.name.Substring(0, "fruit_plant".Length) == "fruit_plant")
                {
                    Fruits.Add(child);
                    child.gameObject.AddComponent<FoodProperty>();
                }
            }
        }

        /*(可能比较费性能)
        //设置水面的标签和组件
        Water = GameObject.Find("WaterCollider");
        Water.tag = WaterTag;
        if (!Water.GetComponent<MeshCollider>()) Water.AddComponent<MeshCollider>();
        Water.GetComponent<MeshCollider>().convex = true;
        Water.GetComponent<MeshCollider>().isTrigger = true;//不管用？？？
        //设置地形标签
        GameObject.Find("TowerLand").tag = Terriantag;
        GameObject.Find("Terrian").tag = Terriantag;*/
	}

    void Start()
    {
        foreach (Transform child in Fruits)//设置水果的组件
        {
            //因为在树上不能受重力影响
            //child.gameObject.GetComponent<Rigidbody>().useGravity = false;
            child.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            //设置是否在树上的标识
            child.gameObject.GetComponent<FoodProperty>().IsOnTrees = true;

            //设置碰撞水果层
            child.gameObject.layer = LayerMask.NameToLayer("Food");

            //预制体赋值
            child.GetComponent<FoodProperty>().hole = hole;
            child.GetComponent<FoodProperty>().FoodTrialMaterial = FoodTrialMaterial;

            //蘑菇上的果子Collioder不一样大
            if (child.gameObject.name.Length > "fruit_mushroom".Length && child.gameObject.name.Substring(0, "fruit_mushroom".Length) == "fruit_mushroom")
            {
                child.GetComponent<SphereCollider>().radius *= 5f;
            }
        }
    }
}
