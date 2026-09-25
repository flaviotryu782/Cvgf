using UnityEngine;

public sealed class DribbleState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.Dribble;

    public DribbleState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
        context.AnimationController?.PlayDribble();
    }

    public void Tick()
    {
        if (!context.HasBall)
        {
            controller.ChangeState(AIStateId.ChaseBall);
            return;
        }

        AIAction action = controller.DecisionSystem.Decide();

        switch (action)
        {
            case AIAction.Shoot:
                controller.ChangeState(AIStateId.Shoot);
                break;
            case AIAction.Pass:
                controller.ChangeState(AIStateId.Pass);
                break;
            default:
                MoveForward();
                break;
        }
    }

    private void MoveForward()
    {
        Vector3 destination = context.Self.position + context.Self.forward * 4f;
        context.MoveTo(destination, controller.MoveSpeed);
    }

    public void Exit()
    {
    }
}
