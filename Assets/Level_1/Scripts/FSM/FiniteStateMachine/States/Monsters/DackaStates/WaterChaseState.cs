using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaterChaseState : FSMState
{
    //Act
    public float viewDistance;  
    public float viewAngle = 140f;
    private float stopDistance;//根据食物模型大小匹配
    private float recentFoodDistance;
    private Collider recentFood;
    private float speed = 1f;
    private GameObject Water;
    private float heightOverRiver;
    private float FloatSinAngle = 0f;
    private Vector3 offset;//根据鸭子模型大小矫正锚点缺陷
    private float rangefloat;
    //Reason
    private bool GoToLand = false;
    private float roleSize;//根据鸭子模型大小调整（还有锚点位置缺陷问题）
    private int IsEatID = Animator.StringToHash("IsEat");
    private int IsLandID = Animator.StringToHash("IsLand");
    public WaterChaseState(FSMSystem fsm, GameObject role)
        : base(fsm, role)
    {
        stateID = StateID.WaterChaseState;
        //参数
        rangefloat = 0.05f;
        stopDistance = 1.3f * role.transform.localScale.x;
        offset = new Vector3(0f, 1.8f, 0f) * role.transform.localScale.x;
        viewDistance = 8 * role.transform.localScale.x;
        heightOverRiver = 3.8f;
        roleSize = role.transform.localScale.x;
        //Act
        Water = GameObject.FindGameObjectWithTag("Water");
    }
    public override void Act(GameObject role)
    {
        //获得最近的水果
        Collider[] colliders = Physics.OverlapSphere(role.transform.position, viewDistance);
        int foodCount = 0;
        if (colliders.Length > 0)
        {
            recentFoodDistance = 1000f;
            recentFood = null;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != role.gameObject && collider.tag.Length >= "Food".Length && collider.tag.Substring(0, "Food".Length) == "Food")
                {
                    foodCount += 1;
                    float distance = Vector3.Distance(collider.transform.position, role.transform.position);
                    if (distance <= recentFoodDistance)
                    {
                        recentFoodDistance = distance;
                        recentFood = collider;
                    }
                }
            }
        }
        if (foodCount == 0)
        {
            recentFood = null;
        }
        //追最近的水果
        if (recentFood != null)
        {
            Vector3 foodDir = recentFood.transform.position - role.transform.position;
            if (Vector3.Distance(recentFood.transform.position, role.transform.position) > stopDistance)
            {
                float angle = Vector3.Angle(foodDir, role.transform.forward);
                if (angle <= viewAngle / 2)
                {
                    role.transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
            }
            else
            {
                //Debug.Log("在附近");
            }
            role.transform.rotation = Quaternion.Slerp(role.transform.rotation, Quaternion.LookRotation(foodDir.normalized), Time.deltaTime);
            role.transform.eulerAngles = new Vector3(0f, role.transform.eulerAngles.y, 0f);
        }
        FloatSinAngle += Time.deltaTime;
        role.transform.Translate(Vector3.Lerp(Vector3.zero, new Vector3(0f, Water.transform.position.y + heightOverRiver - role.transform.position.y - offset.y + rangefloat * Mathf.Sin(FloatSinAngle), 0f), Time.deltaTime));
    }
    public override void Reason(GameObject role)
    {
        //判断是否上岸
        GoToLand = false;
        Collider[] colliders = Physics.OverlapSphere(role.transform.position + offset, roleSize);    //模型锚点缺陷
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != role.gameObject && collider.tag.Length >= "Terrian".Length && collider.tag.Substring(0, "Terrian".Length) == "Terrian")
            {
                GoToLand = true;
            }
        }
        if (GoToLand == false)
        {
            if (recentFood == null)
            {
                fsm.PerformTransition(Transition.LostFood);
                role.GetComponent<Animator>().SetBool(IsEatID, false);
                role.GetComponent<Animator>().SetBool(IsLandID, false);
                //模型旋转缺陷
                role.transform.eulerAngles = role.transform.eulerAngles + new Vector3(0f, 180f, 0f);
                if (role.transform.Find("Roation")) role.transform.Find("Roation").transform.eulerAngles = role.transform.eulerAngles + new Vector3(0f, 0f, 0f);
            }
            else if (Vector3.Distance(recentFood.transform.position, role.transform.position + offset) <= stopDistance)
            {
                role.tag = "DuckWaterEat";
                if (!role.GetComponent<NavMeshObstacle>()) role.AddComponent<NavMeshObstacle>();
                role.GetComponent<NavMeshObstacle>().center = new Vector3(0f, 1.7f, 0f);

                fsm.PerformTransition(Transition.GetFood);
                role.GetComponent<Animator>().SetBool(IsEatID, true);
                role.GetComponent<Animator>().SetBool(IsLandID, false);
                role.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else
        {
            fsm.PerformTransition(Transition.GoToLand);
            role.GetComponent<Animator>().SetBool(IsEatID, false);
            role.GetComponent<Animator>().SetBool(IsLandID, true);
            //更改组件
            if (!role.GetComponent<Rigidbody>()) role.AddComponent<Rigidbody>();
            role.GetComponent<Rigidbody>().useGravity = true;
            role.GetComponent<Rigidbody>().angularDrag = 0.2f;
            role.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
