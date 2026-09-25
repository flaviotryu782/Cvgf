using UnityEngine;

public sealed class PassState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.Pass;

    public PassState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
        if (!context.HasBall)
        {
            controller.ChangeState(AIStateId.ChaseBall);
            return;
        }

        Transform target = controller.DecisionSystem.FindBestPassTarget();
        Vector3 direction = controller.DecisionSystem.GetPassDirection(target);

        if (target == null || direction == Vector3.zero)
        {
            controller.ChangeState(AIStateId.Dribble);
            return;
        }

        context.AnimationController?.PlayPass();
        context.BallController.Pass(direction, controller.PassPower);
    }

    public void Tick()
    {
        controller.ChangeState(AIStateId.ChaseBall);
    }

    public void Exit()
    {
    }
}
