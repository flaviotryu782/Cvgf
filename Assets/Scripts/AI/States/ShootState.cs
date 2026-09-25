using UnityEngine;

public sealed class ShootState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.Shoot;

    public ShootState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
        if (!context.HasBall || context.TargetGoal == null)
        {
            controller.ChangeState(AIStateId.ChaseBall);
            return;
        }

        Vector3 direction = context.TargetGoal.position - context.Self.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            context.AnimationController?.PlayKick();
            context.BallController.Shoot(direction.normalized, controller.ShotPower, BallController.ShotType.Ground);
        }
    }

    public void Tick()
    {
        controller.ChangeState(AIStateId.ChaseBall);
    }

    public void Exit()
    {
    }
}
