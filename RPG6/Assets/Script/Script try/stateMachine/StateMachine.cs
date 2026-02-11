using UnityEngine;

public class StateMachine
{
    public EntityState currentState {  get; private set; }
    public bool canChangeState;

    public void Initialize(EntityState startState)//初始化状态机
    {
        canChangeState = true;
        currentState = startState;
        if (currentState != null)
            currentState.Enter();
        else
            Debug.LogWarning("StateMachine.Initialize called with null startState.");
    }

    public void ChangeState(EntityState newState)//切换状态
    {
        if (canChangeState == false)
            return;

        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }

    public void UpdateActiveState()//更新当前状态
    {
        if (currentState != null)
            currentState.Update();
        else
            Debug.LogWarning("StateMachine.UpdateActiveState called but currentState is null."+currentState);
    }

    public void SwitchOffStateMachine() => canChangeState = false;//关闭状态机

}
