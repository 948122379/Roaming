using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制各状态下的转换条件(转换条件-状态ID)

public enum Transition//转换条件
{
    NullTransition=0,
    //LitteDuck
    LookFood,           //LandPatrolState->LandChaseState
    GetFood,            //LandChaseState->LandEatState
    LostFood,           //LandEatState/LandChaseState->LandPatrolState
    FallIntoWater,      //LandChaseState->WaterChaseState
    GoToLand            //waterDuck
}
public enum StateID//状态名称
{
    NullStateID=0,
    //LitteDuck
    LandPatrolState,    //landDuck
    LandChaseState,
    LandEatState,
    WaterPatrolState,   //waterDuck
    WaterChaseState,
    WaterEatState
    //FlyDuck
}
public abstract class FSMState {
    protected StateID stateID;
    public StateID ID{get{return stateID;}}
    protected Dictionary<Transition ,StateID> map=new Dictionary<Transition,StateID>();
    
    protected FSMSystem fsm;
    public FSMState(FSMSystem fsm, GameObject role)
    {
        this.fsm=fsm;
    }//新建本类时获取到本类
    public void AddTransition(Transition trans,StateID id)//加入转换条件
    {
        if(trans==Transition.NullTransition){
            Debug.LogError("不允许NullTransition");return;
        }
        if(id==StateID.NullStateID){
            Debug.LogError("不允许NullStateID");return;
        }
        if(map.ContainsKey(trans)){
            Debug.LogError("添加转换条件时"+trans+"已存在与map中");return;
        }
        map.Add(trans,id);
    }
    public void DeleteTransition(Transition trans)//删除转换条件
    {
        if(trans==Transition.NullTransition){
            Debug.LogError("不允许NullTransition");return;
        }
        if(map.ContainsKey(trans)==false){
            Debug.LogError("删除转换条件时"+trans+"已不存在于map中");return;
        }
        map.Remove(trans);
    }
    public StateID GetOutputState(Transition trans)//获得相对应转换条件的状态
    {
        if(map.ContainsKey(trans)){
            return map[trans];
        }
        return StateID.NullStateID;
    }
    //公有可定义事件
    public virtual void DoBeforeEntering(){}
    public virtual void DoAfterLeaving(){}
    //每个继承本类的子类需要重新定义的函数
    public abstract void Act(GameObject role);
    public abstract void Reason(GameObject role);
}
