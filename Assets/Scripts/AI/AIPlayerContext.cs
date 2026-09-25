using UnityEngine;

public sealed class AIPlayerContext
{
    private readonly TeamAIControllerStateDriven controller;

    public TeamAIControllerStateDriven Controller => controller;
    public Transform Self => controller.transform;
    public Transform Ball => controller.Ball;
    public BallController BallController => controller.BallController;
    public Transform TargetGoal => controller.TargetGoal;
    public Transform HomePosition => controller.HomePosition;
    public Transform BallSocket => controller.BallSocket;
    public CharacterController CharacterController => controller.CharacterController;
    public PlayerAnimationController AnimationController => controller.AnimationController;

    public bool HasBall => BallController != null && BallController.Owner == Self;
    public float DistanceToBall => Ball == null ? float.MaxValue : Vector3.Distance(Self.position, Ball.position);

    public AIPlayerContext(TeamAIControllerStateDriven controller)
    {
        this.controller = controller;
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = destination - Self.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.05f)
        {
            AnimationController?.SetLocomotion(0f, false);
            return;
        }

        direction.Normalize();
        CharacterController.Move(direction * speed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Self.rotation = Quaternion.Slerp(Self.rotation, targetRotation, 8f * Time.deltaTime);

        AnimationController?.SetLocomotion(1f, false);
    }

    public bool TryTakePossession(float range)
    {
        if (Ball == null || BallController == null || BallSocket == null)
            return false;

        if (BallController.Owner != null && BallController.Owner != Self)
            return false;

        if (DistanceToBall > range)
            return false;

        return BallController.TryControl(Self, BallSocket);
    }
}
