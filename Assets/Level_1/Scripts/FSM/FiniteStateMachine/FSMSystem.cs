using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理状态(状态ID-状态)

public class FSMSystem {
    private Dictionary<StateID, FSMState> states = new Dictionary<StateID, FSMState>();
    private StateID currentStateID;
    public FSMState currentState;
    public void Update(GameObject role)//统一更新FSMState子类的两个重新定义的函数
    {
        currentState.Act(role);
        currentState.Reason(role);
	}
    public void AddState(FSMState s)//添加状态
    {
        if (s == null)
        {
            Debug.LogError("FSMState不能为空"); return;
        }
        if (currentState == null)
        {
            currentState = s;
            currentStateID = s.ID;
        }
        if (states.ContainsKey(s.ID))
        {
            Debug.LogError("状态" + s.ID + "已经存在，无法重复添加"); return;
        }
        states.Add(s.ID, s);
    }
    public void DeleteState(StateID id)//删除状态
    {
        if (id == StateID.NullStateID)
        {
            Debug.LogError("无法删除空状态"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("无法删除不存在的状态" + id); return;
        }
        states.Remove(id);
    }
    public void PerformTransition(Transition trans)//执行状态转换
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("无法执行空的转换条件") ;return ;
        }

        StateID id = currentState.GetOutputState(trans);
        if (id == StateID.NullStateID)
        {
            Debug.LogWarning("当前状态" + currentStateID + "无法根据条件转换" + trans + "发生转换"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("在状态机里面不存在状态" + id + ",无法进行转换！"); return;
        }
        FSMState state = states[id];
        currentState.DoAfterLeaving();
        currentState = state;
        currentStateID = id; 
        currentState.DoBeforeEntering();
    }
}
