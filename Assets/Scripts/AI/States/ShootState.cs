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

        Vector3 toGoal = context.TargetGoal.position - context.Self.position;
        toGoal.y = 0f;

        if (toGoal.sqrMagnitude <= 0.01f)
        {
            controller.ChangeState(AIStateId.ChaseBall);
            return;
        }

        float distance = toGoal.magnitude;
        float pressure = Mathf.Clamp01(1f - (distance / controller.MaxShootDistance));
        float power = Mathf.Lerp(controller.MinShotPower, controller.MaxShotPower, Mathf.InverseLerp(controller.MinShootDistance, controller.MaxShootDistance, distance));
        float accuracy = Mathf.Clamp01(1f - (pressure * 0.35f) - (distance / controller.MaxShootDistance) * 0.15f);
        float angle = Mathf.Clamp((controller.TargetGoal.position.x - context.Self.position.x) * 0.25f, -22f, 22f);
        ShotTechnique technique = distance < 8f ? ShotTechnique.Place : (distance > 18f ? ShotTechnique.Power : ShotTechnique.Ground);

        if (controller.Role == TeamAIControllerStateDriven.Role.Attacker && distance > 12f)
            technique = ShotTechnique.Curve;

        context.AnimationController?.PlayKick();
        context.BallController.Shoot(
            toGoal.normalized,
            power,
            BallController.ShotType.Ground,
            accuracy,
            angle,
            technique,
            pressure,
            false
        );
    }

    public void Tick()
    {
        controller.ChangeState(AIStateId.ChaseBall);
    }

    public void Exit()
    {
    }
}
