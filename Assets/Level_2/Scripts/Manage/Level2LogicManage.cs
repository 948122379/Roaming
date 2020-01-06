using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2LogicManage : MonoBehaviour
{
    private RoomRecorder roomRecorder;
    public MirrorLogic mirrorLogic;
    public SwitchLogic switchLogic;
    public CollectLogic collectLogic;

	// Use this for initialization
	void Start () {
        roomRecorder = new RoomRecorder();
        roomRecorder.Init();

        mirrorLogic = new MirrorLogic();
        switchLogic = new SwitchLogic();
        mirrorLogic.Init(roomRecorder);
        switchLogic.Init(roomRecorder);

        collectLogic = new CollectLogic();
        collectLogic.Init();
	}
	
	// Update is called once per frame
	void Update () {
        mirrorLogic.Update();
        switchLogic.Update(mirrorLogic.IsOnWall);
        collectLogic.Update();
	}
}
