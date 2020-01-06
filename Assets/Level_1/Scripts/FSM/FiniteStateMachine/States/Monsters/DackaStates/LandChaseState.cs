using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LandChaseState : FSMState
{
    //Act
    public float viewDistance;  
    public float viewAngle = 140f;
    private float stopDistance;//根据食物模型大小匹配
    private float recentFoodDistance;
    private Collider recentFood;
    private float speed = 1.5f;
    private Vector3 offset;//根据鸭子模型大小矫正锚点缺陷
    //Reason
    private bool IsIntoWater = false;
    private float roleSize ;//根据鸭子模型大小调整（还有锚点位置缺陷问题）
    private int IsEatID = Animator.StringToHash("IsEat");
    private int IsLandID = Animator.StringToHash("IsLand");
    public LandChaseState(FSMSystem fsm, GameObject role):base(fsm,role)
    {
        stateID = StateID.LandChaseState;

        stopDistance = 1.3f*role.transform.localScale.x;
        offset = new Vector3(0f, 1.8f, 0f) * role.transform.localScale.x;
        viewDistance = 15 * role.transform.localScale.x;
        roleSize = role.transform.localScale.x;
    }
    public override void Act(GameObject role)
    {
        //获得最近的水果
        Collider[] colliders = Physics.OverlapSphere(role.transform.position + offset, viewDistance);
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
                    float distance = Vector3.Distance(collider.transform.position, role.transform.position + offset);
                    if (distance <= recentFoodDistance)
                    {
                        recentFoodDistance = distance;
                        recentFood = collider;
                    }
                }
            }
        }
        if(foodCount==0)
        {
            recentFood = null;
        }
        //追最近的水果
        if (recentFood != null)
        {
            Vector3 foodDir = recentFood.transform.position - role.transform.position + offset;
            if (Vector3.Distance(recentFood.transform.position, role.transform.position + offset) > stopDistance)
            {
                float angle = Vector3.Angle(foodDir, role.transform.forward);
                if (angle <= viewAngle / 2)
                {
                    Vector3 speedDir = Vector3.forward * speed * Time.deltaTime;
                    role.transform.Translate(Vector3.Lerp(speedDir / 2, speedDir, Time.deltaTime));
                }
            }
            else
            {
                //Debug.Log("在附近");
            }
            role.transform.rotation = Quaternion.Slerp(role.transform.rotation, Quaternion.LookRotation(foodDir.normalized), Time.deltaTime);
            role.transform.eulerAngles = new Vector3(0f, role.transform.eulerAngles.y, 0f);
        }
    }
    public override void Reason(GameObject role)
    {
        //判断是否落水
        IsIntoWater = false;
        Collider[] colliders = Physics.OverlapSphere(role.transform.position + offset, roleSize);    //模型锚点缺陷
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != role.gameObject && collider.tag.Length >= "Water".Length && collider.tag.Substring(0, "Water".Length) == "Water")
            {
                IsIntoWater = true;
            }
        }
        //判断路上状态
        if (IsIntoWater == false)
        {
            if (recentFood == null)
            {
                fsm.PerformTransition(Transition.LostFood);
                role.GetComponent<Animator>().SetBool(IsEatID, false);
                role.GetComponent<Animator>().SetBool(IsLandID, true);
                //模型旋转缺陷
                role.transform.eulerAngles = role.transform.eulerAngles + new Vector3(0f, 180f, 0f);
                if (role.transform.Find("Roation")) role.transform.Find("Roation").transform.eulerAngles = role.transform.eulerAngles + new Vector3(0f, 0f, 0f);
            }
            else if (Vector3.Distance(recentFood.transform.position, role.transform.position + offset) <= stopDistance)
            {
                fsm.PerformTransition(Transition.GetFood);
                role.GetComponent<Animator>().SetBool(IsEatID, true);
                role.GetComponent<Animator>().SetBool(IsLandID, true);
            }
        }
        else {
            fsm.PerformTransition(Transition.FallIntoWater);
            role.GetComponent<Animator>().SetBool(IsEatID, false);
            role.GetComponent<Animator>().SetBool(IsLandID, false);
            //更改组件
            if (!role.GetComponent<Rigidbody>()) role.AddComponent<Rigidbody>();
            role.GetComponent<Rigidbody>().useGravity = false;
            role.GetComponent<Rigidbody>().angularDrag = 0.2f;
            role.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezeRotationY;
        }
    }
}
