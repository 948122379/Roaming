using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPatrolState : FSMState
{
    //Act
    private bool isEating;
    private bool isRotating;
    private bool isTurning;
    private bool is_idle;
    private float speed;

    private float start_rotation;
    private int random_direction;

    private bool turn;
    private float time;
    private float time_limit;
    private float time_limit2;
    private int random_time;
    private int random_time2;
    private int random_isIdle;
    private int random_isRotating;

    private GameObject Water;
    private float FloatSinAngle = 0f;
    private Vector3 offset;//根据鸭子模型大小矫正锚点缺陷
    private float heightOverRiver;
    private float rangefloat;
    //Reason
    public float viewDistance;
    public float viewAngle = 120f;
    private bool GoToLand = false;
    private float roleSize;//根据鸭子模型大小调整（还有锚点位置缺陷问题）
    private int IsEatID = Animator.StringToHash("IsEat");
    private int IsLandID = Animator.StringToHash("IsLand");
    public WaterPatrolState(FSMSystem fsm, GameObject role)
        : base(fsm, role)
    {
        stateID = StateID.WaterPatrolState;
        //参数
        rangefloat = 0.05f;
        offset = new Vector3(0f, 1.8f, 0f) * role.transform.localScale.x;
        viewDistance = 8 * role.transform.localScale.x;
        heightOverRiver = 3.8f;
        roleSize = role.transform.localScale.x;
        //参数
        is_idle = true;
        turn = false;
        speed = 0.015f;
        isEating = false;
        isRotating = false;
        isTurning = false;
        time_limit = 0.0f;
        time_limit2 = 1.0f;
        //Act
        Water = GameObject.FindGameObjectWithTag("Water");
    }
    public override void Act(GameObject role)
    {
        Ducka_AI_Ctrl(role);
        FloatSinAngle += Time.deltaTime;
        //role.transform.Translate(Vector3.Lerp(Vector3.zero, new Vector3(0f, Water.transform.position.y + heightOverRiver - role.transform.position.y - offset.y + rangefloat * Mathf.Sin(FloatSinAngle), 0f), Time.deltaTime));
        role.transform.Translate(new Vector3(0f, Water.transform.position.y + heightOverRiver - role.transform.position.y - offset.y + rangefloat * Mathf.Sin(FloatSinAngle), 0f));
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
            Collider[] collidersMore = Physics.OverlapSphere(role.transform.position + offset, viewDistance);
            if (collidersMore.Length > 0)
            {
                foreach (Collider collider in collidersMore)
                {
                    if (collider.tag.Length >= "Food".Length && collider.tag.Substring(0, "Food".Length) == "Food")
                    {
                        fsm.PerformTransition(Transition.LookFood);
                        role.GetComponent<Animator>().SetBool(IsEatID, false);
                        role.GetComponent<Animator>().SetBool(IsLandID, false);
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

    //Act
    void Ducka_AI_Ctrl(GameObject role)
    {
        Ducka_wandering(role);
        Turning(role);
    }
    void Ducka_wandering(GameObject role)
    {
        time = Time.time;
        if (time >= time_limit2)
        {
            random_isIdle = Random.Range(0, 2);
            random_time = Random.Range(20, 30);
            time_limit2 = time + random_time;
            if (random_isIdle == 0)
            {
                is_idle = true;
            }
            else
            {
                is_idle = false;
            }
        }
        if (!is_idle)
        {
            if (!isTurning)
            {
                if (time >= time_limit)
                {
                    random_time = Random.Range(1, 5);
                    time_limit = time + random_time;
                    isRotating = false;
                }
                if (!isRotating)
                {
                    random_isRotating = Random.Range(0, 360);
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
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("碰到陆地了");
        if (collision.gameObject.tag == "Terrain")
        {
            if (!turn)
            {
                isTurning = true;
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
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
                random_direction = Random.Range(0, 2);
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
