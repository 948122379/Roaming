using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPatrolState : FSMState
{
    //Act
    private GameObject LandMonster;
    private bool isEating;
    private bool isRotating;
    private bool isTurning;
    private float speed;

    private float start_rotation;
    private int random_direction;

    private bool turn;
    private float time;
    private float time_limit;
    private int random_time;
    private int random_isRotating;
    private Vector3 offset = new Vector3(0f, 1.8f, 0f);//根据鸭子模型大小矫正锚点缺陷
    //Reason
    private bool IsIntoWater = false;
    private float roleSize ;//根据鸭子模型大小调整（还有锚点位置缺陷问题）
    public float viewDistance = 5f;
    public float viewAngle = 120f;
    private int IsEatID = Animator.StringToHash("IsEat");
    private int IsLandID = Animator.StringToHash("IsLand");
    public LandPatrolState(FSMSystem fsm, GameObject role):base(fsm, role)//初始化
    {
        //设定状态
        stateID = StateID.LandPatrolState;

        //参数
        offset = new Vector3(0f, 1.8f, 0f) * role.transform.localScale.x;
        viewDistance = 8 * role.transform.localScale.x;
        roleSize = role.transform.localScale.x;

        //组件
        if (!role.GetComponent<Rigidbody>()) role.AddComponent<Rigidbody>();
        role.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        if (!role.GetComponent<SphereCollider>()) role.AddComponent<SphereCollider>();
        role.GetComponent<SphereCollider>().center = new Vector3(0f, 1.8f, 0f);             //模型锚点缺陷
        role.GetComponent<SphereCollider>().radius = 0.7f;
        //参数
        turn = false;
        speed = 0.02f;
        isEating = false;
        isRotating = false;
        isTurning = false;
        time_limit = 2.0f;

    }
    public override void Act(GameObject role)
    {
        Gravity(role);
        Ducka_AI_Ctrl(role);
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
        if (IsIntoWater == false)
        {
            Collider[] collidersMore = Physics.OverlapSphere(role.transform.position + offset, viewDistance);
            if (collidersMore.Length > 0)
            {
                foreach (Collider collider in collidersMore)
                {
                    if (collider.tag.Length >= "Food".Length && collider.tag.Substring(0, "Food".Length) == "Food")
                    {
                        fsm.PerformTransition(Transition.LookFood);
                        role.GetComponent<Animator>().SetBool(IsEatID, false);
                        role.GetComponent<Animator>().SetBool(IsLandID, true);
                        //模型旋转缺陷
                        role.transform.eulerAngles = role.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f);
                        if (role.transform.Find("Roation")) role.transform.Find("Roation").transform.eulerAngles = role.transform.eulerAngles + new Vector3(0f, 180f, 0f);
                        return;
                    }
                }
            }
        }
        else
        {
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

    //Act
    void Ducka_AI_Ctrl(GameObject role)
    {
        Ducka_wandering(role);
        Turning(role);
    }
    void Ducka_wandering(GameObject role)
    {
        time = Time.time;
        if (!isTurning)
        {
            if (time >= time_limit)
            {
                random_time = UnityEngine.Random.Range(1, 5);
                time_limit = time + random_time;
                isRotating = false;
            }
            if (!isRotating)
            {
                random_isRotating = UnityEngine.Random.Range(0, 360);
                isRotating = true;
            }
            else
            {
                if (random_isRotating < 30)
                {
                    role.transform.Rotate(0, -1, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.4f);
                }
                else if (random_isRotating > 330)
                {
                    role.transform.Rotate(0, 1, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.4f);
                }
                else if (random_isRotating >= 30 && random_isRotating < 60)
                {
                    role.transform.Rotate(0, -0.5f, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.6f);
                }
                else if (random_isRotating <= 330 && random_isRotating > 300)
                {
                    role.transform.Rotate(0, 0.5f, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.6f);
                }
                else if (random_isRotating >= 60 && random_isRotating < 120)
                {
                    role.transform.Rotate(0, -0.1f, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.8f);
                }
                else if (random_isRotating <= 300 && random_isRotating > 240)
                {
                    role.transform.Rotate(0, 0.1f, 0);
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x * 0.8f);
                }
                else
                {
                    role.transform.Translate(0, 0, -speed * role.transform.localScale.x);
                }
            }
        }
    }
    void Gravity(GameObject role)
    {
        role.GetComponent<Rigidbody>().AddForce(0, -1, 0);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Barries")
        {
            if (!turn)
            {
                isTurning = true;
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Barries")
        {
            isTurning = false;
            turn = false;
        }
    }
    void Turning(GameObject role)
    {
        if (isTurning)
        {
            if (!turn)
            {
                turn = true;
                start_rotation = role.transform.eulerAngles.y;
                random_direction = UnityEngine.Random.Range(0, 2);
            }
        }

        if (turn)
        {
            if (random_direction == 0)
            {
                Quaternion target_rotation = Quaternion.Euler(role.transform.rotation.x, start_rotation + 180, role.transform.rotation.z);
                role.transform.rotation = Quaternion.RotateTowards(role.transform.rotation, target_rotation, 1);
                if (role.transform.eulerAngles.y >= start_rotation + 175 && role.transform.eulerAngles.y <= start_rotation + 185)
                {
                    isTurning = false;
                    turn = false;
                }
            }
            else
            {
                Quaternion target_rotation = Quaternion.Euler(role.transform.rotation.x, start_rotation - 180, role.transform.rotation.z);
                role.transform.rotation = Quaternion.RotateTowards(role.transform.rotation, target_rotation, 1);
                if (role.transform.eulerAngles.y >= start_rotation - 185 && role.transform.eulerAngles.y <= start_rotation - 175)
                {
                    isTurning = false;
                    turn = false;
                }
            }
        }
    }
}
