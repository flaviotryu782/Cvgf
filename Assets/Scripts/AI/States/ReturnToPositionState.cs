using UnityEngine;

public sealed class ReturnToPositionState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.ReturnToPosition;

    public ReturnToPositionState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Tick()
    {
        if (context.HomePosition == null)
            return;

        context.MoveTo(context.HomePosition.position, controller.MoveSpeed);
    }

    public void Exit()
    {
    }
}
