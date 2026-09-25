using UnityEngine;

public sealed class AIPlayerContext
{
    private readonly TeamAIControllerStateDriven controller;
    private readonly AdvancedBallControl advancedControl;
    private Vector3 currentVelocity;

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
        advancedControl = controller.GetComponent<AdvancedBallControl>() ?? controller.gameObject.AddComponent<AdvancedBallControl>();
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = destination - Self.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.05f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, 24f * Time.deltaTime);
            CharacterController.Move(currentVelocity * Time.deltaTime);
            AnimationController?.SetLocomotion(0f, false);
            return;
        }

        direction.Normalize();
        currentVelocity = Vector3.MoveTowards(currentVelocity, direction * speed, 18f * Time.deltaTime);
        CharacterController.Move(currentVelocity * Time.deltaTime);
        Self.rotation = Quaternion.Slerp(Self.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
        AnimationController?.SetLocomotion(Mathf.Clamp01(currentVelocity.magnitude / Mathf.Max(speed, 0.01f)), false);
    }

    public bool TryTakePossession(float range)
    {
        if (Ball == null || BallController == null || BallSocket == null || DistanceToBall > range)
            return false;
        if (BallController.Owner != null && BallController.Owner != Self)
            return false;

        Vector3 intendedDirection = currentVelocity.sqrMagnitude > 0.01f ? currentVelocity.normalized : Self.forward;
        return advancedControl.TryControl(BallController, BallSocket, intendedDirection) || BallController.TryControl(Self, BallSocket);
    }
}
