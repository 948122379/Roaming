using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEatState : FSMState
{
    //Act
    private float recentFoodDistance;
    private Collider recentFood;
    public float viewDistance;
    private Vector3 offset ;//根据鸭子模型大小矫正锚点缺陷
    //Reason
    private float stopDistance ;//根据食物模型大小匹配
    private int IsEatID = Animator.StringToHash("IsEat");
    private int IsLandID = Animator.StringToHash("IsLand");
    public LandEatState(FSMSystem fsm, GameObject role)
        : base(fsm, role)
    {
        stateID = StateID.LandEatState;

        stopDistance = 1.3f * role.transform.localScale.x;
        offset = new Vector3(0f, 1.8f, 0f) * role.transform.localScale.x;
        viewDistance = 8 * role.transform.localScale.x;
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
        if (foodCount == 0)
        {
            recentFood = null;
        }
        //看着食物
        if (recentFood != null)
        {
            Vector3 foodDir = recentFood.transform.position - role.transform.position + offset;
            role.transform.rotation = Quaternion.Slerp(role.transform.rotation, Quaternion.LookRotation(foodDir.normalized), Time.deltaTime);
            role.transform.eulerAngles = new Vector3(0f, role.transform.eulerAngles.y, 0f);
        }
    }
    public override void Reason(GameObject role)
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
        else
        {
            if (Vector3.Distance(recentFood.transform.position, role.transform.position + offset) > stopDistance + 0.1f)
            {
                fsm.PerformTransition(Transition.LookFood);
                role.GetComponent<Animator>().SetBool(IsEatID, false);
                role.GetComponent<Animator>().SetBool(IsLandID, true);
            }
        }
    }
}
