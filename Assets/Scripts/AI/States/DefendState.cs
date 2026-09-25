using UnityEngine;

public sealed class DefendState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.Defend;

    public DefendState(AIPlayerContext context, TeamAIControllerStateDriven controller)
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
        {
            controller.ChangeState(AIStateId.ReturnToPosition);
            return;
        }

        if (context.BallController == null)
        {
            controller.ChangeState(AIStateId.ReturnToPosition);
            return;
        }

        Transform ballOwner = context.BallController.Owner;
        if (ballOwner == null || ballOwner == context.Self)
        {
            controller.ChangeState(AIStateId.ChaseBall);
            return;
        }

        Vector3 defendTarget;
        if (controller.TargetGoal != null)
        {
            defendTarget = Vector3.Lerp(context.Ball.position, controller.TargetGoal.position, 0.55f);
        }
        else if (controller.HomePosition != null)
        {
            defendTarget = controller.HomePosition.position;
        }
        else
        {
            defendTarget = context.Self.position;
        }

        context.MoveTo(defendTarget, controller.MoveSpeed * 0.9f);

        if (context.TryTakePossession(controller.ControlRange + 0.8f))
        {
            controller.ChangeState(AIStateId.Dribble);
        }
    }

    public void Exit()
    {
    }
}
