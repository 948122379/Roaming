using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckAI : MonoBehaviour {
    private FSMSystem fsm;
    public bool IsInWater;
	// Use this for initialization
	void Start () {
        InitFSM();
	}
    void InitFSM()
    {
        fsm = new FSMSystem();
        
        FSMState landPatrolState = new LandPatrolState(fsm,gameObject);
        FSMState landChaseState = new LandChaseState(fsm,gameObject);
        FSMState landEatState = new LandEatState(fsm, gameObject);
        FSMState waterPatrolState = new WaterPatrolState(fsm, gameObject);
        FSMState waterChaseState = new WaterChaseState(fsm, gameObject);
        FSMState waterEatState = new WaterEatState(fsm, gameObject);
        fsm.AddState(landPatrolState);
        fsm.AddState(landChaseState);
        fsm.AddState(landEatState);
        fsm.AddState(waterPatrolState);
        fsm.AddState(waterChaseState);
        fsm.AddState(waterEatState);

        //转换
        landPatrolState.AddTransition(Transition.LookFood, StateID.LandChaseState);//landPatrolState
        landPatrolState.AddTransition(Transition.FallIntoWater, StateID.WaterPatrolState);//陆地->水里
        landChaseState.AddTransition(Transition.LostFood, StateID.LandPatrolState);//landChaseState
        landChaseState.AddTransition(Transition.FallIntoWater, StateID.WaterChaseState);//陆地->水里
        landChaseState.AddTransition(Transition.GetFood, StateID.LandEatState);
        landEatState.AddTransition(Transition.LostFood, StateID.LandPatrolState);//landEatState
        landEatState.AddTransition(Transition.LookFood, StateID.LandChaseState);
        waterPatrolState.AddTransition(Transition.LookFood, StateID.WaterChaseState);//waterPatrolState
        waterPatrolState.AddTransition(Transition.GoToLand, StateID.LandPatrolState);//水里->陆地
        waterChaseState.AddTransition(Transition.LostFood, StateID.WaterPatrolState);//waterChaseState
        waterChaseState.AddTransition(Transition.GoToLand, StateID.LandChaseState);//水里->陆地
        waterChaseState.AddTransition(Transition.GetFood, StateID.WaterEatState);
        waterEatState.AddTransition(Transition.LostFood, StateID.WaterPatrolState);//waterEatState
        waterEatState.AddTransition(Transition.LookFood, StateID.WaterChaseState);
        
    }
	void Update () {
        if (fsm == null)
        {
            Debug.LogError(fsm + "找不到了");
            fsm = new FSMSystem();
        }
        fsm.Update(gameObject);
	}
}
