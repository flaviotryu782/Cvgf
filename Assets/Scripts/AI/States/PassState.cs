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
        if (target == null)
        {
            controller.ChangeState(AIStateId.Dribble);
            return;
        }

        PassProfile profile = controller.DecisionSystem.BuildPassProfile(target);
        Vector3 targetPosition = controller.DecisionSystem.GetPassTargetPosition(target, profile);
        Vector3 direction = targetPosition - context.Self.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            controller.ChangeState(AIStateId.Dribble);
            return;
        }

        context.AnimationController?.PlayPass();
        context.BallController.Pass(direction.normalized, profile);
    }

    public void Tick() => controller.ChangeState(AIStateId.ChaseBall);
    public void Exit() { }
}
