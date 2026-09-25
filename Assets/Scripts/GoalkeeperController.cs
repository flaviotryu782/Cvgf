using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GoalkeeperController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ball;
    [SerializeField] private Transform goalCenter;
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Positioning")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float xLimit = 4.5f;
    [SerializeField] private float zLimit = 1.5f;
    [SerializeField] private float predictionTime = 0.45f;
    [SerializeField] private float goalLineOffset = 0.45f;
    [SerializeField] private float cross anticipation = 0.35f;

    [Header("Reaction")]
    [SerializeField] private float saveDistance = 1.3f;
    [SerializeField] private float reactionDelay = 0.12f;
    [SerializeField] private float saveCooldown = 0.6f;
    [SerializeField] private float minimumThreatSpeed = 4f;
    [SerializeField] private LayerMask visionObstacleMask;

    [Header("Save and rebound")]
    [SerializeField] private float catchSpeedLimit = 10f;
    [SerializeField] private float deflectForce = 7f;
    [SerializeField] private float reboundSideForce = 2.5f;

    private CharacterController controller;
    private Rigidbody ballBody;
    private float nextReactionTime;
    private float nextSaveTime;
    private Vector3 predictedBallPosition;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animationController == null)
            animationController = GetComponentInChildren<PlayerAnimationController>();
    }

    private void Update()
    {
        if (ball == null || goalCenter == null)
            return;

        if (ballBody == null)
            ballBody = ball.GetComponent<Rigidbody>();

        if (Time.time >= nextReactionTime)
        {
            nextReactionTime = Time.time + reactionDelay;
            predictedBallPosition = PredictBallPosition();
        }

        bool threat = IsShotThreat();
        Vector3 target = threat ? GetSavePosition(predictedBallPosition) : GetSetPosition(ball.position);
        MoveTo(target, threat ? moveSpeed * 1.15f : moveSpeed);

        if (threat)
            TrySave();
    }

    private Vector3 PredictBallPosition()
    {
        if (ballBody == null)
            return ball.position;

        Vector3 velocity = ballBody.velocity;
        Vector3 predicted = ball.position + velocity * Mathf.Clamp(predictionTime, 0.05f, 1.2f);
        Vector3 goalNormal = goalCenter.forward;
        float denominator = Vector3.Dot(velocity, goalNormal);

        if (Mathf.Abs(denominator) > 0.01f)
        {
            float timeToGoalLine = Vector3.Dot(goalCenter.position - ball.position, goalNormal) / denominator;
            if (timeToGoalLine > 0f && timeToGoalLine < predictionTime)
                predicted = ball.position + velocity * timeToGoalLine;
        }

        // Anticipate a cross by moving slightly toward the projected delivery side.
        if (Mathf.Abs(velocity.x) > 3f)
            predicted.x += Mathf.Sign(velocity.x) * crossAnticipation;

        return predicted;
    }

    private bool IsShotThreat()
    {
        if (ballBody == null || ballBody.velocity.magnitude < minimumThreatSpeed)
            return false;

        Vector3 toGoal = goalCenter.position - ball.position;
        if (Vector3.Dot(ballBody.velocity.normalized, toGoal.normalized) < 0.45f)
            return false;

        if (visionObstacleMask.value != 0)
        {
            Vector3 origin = transform.position + Vector3.up * 0.6f;
            Vector3 toBall = ball.position - origin;
            if (Physics.Raycast(origin, toBall.normalized, out RaycastHit hit, toBall.magnitude, visionObstacleMask, QueryTriggerInteraction.Ignore) && hit.transform != ball)
                return false;
        }

        Vector3 local = goalCenter.InverseTransformPoint(predictedBallPosition);
        return Mathf.Abs(local.x) <= xLimit + 1f && Mathf.Abs(local.z) <= zLimit + 2f;
    }

    private Vector3 GetSavePosition(Vector3 predicted)
    {
        Vector3 local = goalCenter.InverseTransformPoint(predicted);
        local.x = Mathf.Clamp(local.x, -xLimit, xLimit);
        local.z = Mathf.Clamp(local.z, -zLimit, zLimit);
        local.y = 0f;
        return goalCenter.TransformPoint(local) + goalCenter.forward * goalLineOffset;
    }

    private Vector3 GetSetPosition(Vector3 ballPosition)
    {
        Vector3 local = goalCenter.InverseTransformPoint(ballPosition);
        local.x = Mathf.Clamp(local.x, -xLimit, xLimit);
        local.z = Mathf.Clamp(local.z, -zLimit, zLimit);
        local.y = 0f;
        return goalCenter.TransformPoint(local) + goalCenter.forward * goalLineOffset;
    }

    private void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        bool moving = direction.sqrMagnitude > 0.04f;

        if (moving)
        {
            direction.Normalize();
            controller.Move(direction * speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        }

        animationController?.SetLocomotion(moving ? Mathf.Clamp01(speed / Mathf.Max(moveSpeed, 0.01f)) : 0f, speed > moveSpeed);
    }

    private void TrySave()
    {
        if (ballBody == null || Time.time < nextSaveTime || Vector3.Distance(transform.position, predictedBallPosition) > saveDistance)
            return;

        nextSaveTime = Time.time + saveCooldown;
        Vector3 incoming = ballBody.velocity;
        float speed = incoming.magnitude;
        Vector3 awayFromGoal = transform.position - goalCenter.position;
        awayFromGoal.y = 0f;
        awayFromGoal.Normalize();
        Vector3 side = Vector3.Cross(Vector3.up, incoming.normalized);

        animationController?.PlayDive();
        animationController?.PlaySave();
        AudioManager.Instance?.PlaySave();

        if (speed <= catchSpeedLimit && Vector3.Distance(transform.position, predictedBallPosition) < saveDistance * 0.65f)
        {
            ballBody.velocity = Vector3.zero;
            ballBody.angularVelocity = Vector3.zero;
            ballBody.position = transform.position + transform.forward * 0.35f;
            return;
        }

        Vector3 rebound = (awayFromGoal + side * 0.35f).normalized;
        ballBody.velocity = Vector3.zero;
        ballBody.AddForce(rebound * deflectForce + side * reboundSideForce, ForceMode.VelocityChange);
    }

    public void PlayPenaltySave()
    {
        animationController?.PlayDive();
        animationController?.PlaySave();
    }
}
