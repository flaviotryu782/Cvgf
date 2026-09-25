using UnityEngine;

public sealed class ChaseBallState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.ChaseBall;

    public ChaseBallState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Tick()
    {
        if (context.Ball == null)
            return;

        context.MoveTo(context.Ball.position, controller.MoveSpeed);

        if (context.TryTakePossession(controller.ControlRange))
        {
            controller.ChangeState(AIStateId.Dribble);
        }
    }

    public void Exit()
    {
    }
}
