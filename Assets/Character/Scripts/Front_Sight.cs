using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Front_Sight : MonoBehaviour {

    public float ScreenHeight;
    public float ScreenWidth;

    public float scale_x;
    public float scale_y;

    public Image Up;
    public Image Down;
    public Image Left;
    public Image Right;

    public Vector3 Up_pos;
    public Vector3 Down_pos;
    public Vector3 Left_pos;
    public Vector3 Right_pos;

    public float time;
    public float speed;
    static public float recoil;

    static public Vector3 Target_Pos;

    public float offset;

    void Start()
    {
        scale_x = Screen.height / 100;
        scale_y = Screen.height / 20;
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;

        Up_pos = new Vector3(ScreenWidth / 2 + offset, ScreenHeight / 2 + scale_y * 0.8f + offset, 0);
        Down_pos = new Vector3(ScreenWidth / 2 + offset, ScreenHeight / 2 - scale_y * 0.8f + offset, 0);
        Left_pos = new Vector3(ScreenWidth / 2 - scale_y * 0.8f + offset, ScreenHeight / 2 + offset, 0);
        Right_pos = new Vector3(ScreenWidth / 2 + scale_y * 0.8f + offset, ScreenHeight / 2 + offset, 0);
        ////Up.rectTransform = new RectTransform(0, 0, 0);
        // Up.transform.localScale = new Vector3(scale_x, scale_y, 1);
        // Down.transform.localScale = new Vector3(scale_x, scale_y, 1);
        // Left.transform.localScale = new Vector3(scale_y, scale_x, 1);
        // Right.transform.localScale = new Vector3(scale_y, scale_x, 1);
        //
        Up.transform.position = Up_pos;
        Down.transform.position = Down_pos;
        Left.transform.position = Left_pos;
        Right.transform.position = Right_pos;
    }


    void Update()
    {
        //float scale = ((float)Screen.width / Screen.height) / (1920 / 1080);
        //if (scale < 1 && scale > 0)
        //{
        //    Camera.main.orthographicSize /= scale;
        //}
        speed = -(Up.transform.position.y - Up_pos.y) / 10+ recoil;
        Up.transform.Translate(0, speed, 0);
        Down.transform.Translate(0, -speed, 0);
        Left.transform.Translate(-speed, 0, 0);
        Right.transform.Translate(speed, 0, 0);

        if (recoil > 0)
        {
            recoil -= Time.deltaTime * 4;
        }

        float x = Random.Range(Left.transform.position.x, Right.transform.position.x);
        float y = Random.Range(Down.transform.position.y, Up.transform.position.y);
        Target_Pos = new Vector3(x, y, 0);
    }
}
