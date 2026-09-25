using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeamAIController : MonoBehaviour
{
    public enum Role { Attacker, Midfielder, Defender }
    [SerializeField] private Role role = Role.Midfielder;
    [SerializeField] private string teamId = "opponent";
    [SerializeField] private Transform ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private Transform targetGoal;
    [SerializeField] private Transform homePosition;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private float moveSpeed = 4.2f;
    [SerializeField] private float chaseDistance = 18f;
    [SerializeField] private float controlRange = 2f;
    [SerializeField] private float shotPower = 10f;
    [SerializeField] private float decisionInterval = .3f;
    [SerializeField] private float passRange = 9f;
    [SerializeField] private TeamAIController[] teammates;
    [SerializeField] private AdaptiveAIBrain brain;

    private CharacterController controller;
    private AdaptiveAIBrain.ActionType currentAction;
    private float nextDecision;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (brain == null) brain = GetComponent<AdaptiveAIBrain>();
        if (brain == null) brain = gameObject.AddComponent<AdaptiveAIBrain>();
        brain.SetProfile(teamId, role);
    }

    private void Update()
    {
        if (ball == null || ballController == null) return;
        float distance = Vector3.Distance(transform.position, ball.position);
        bool canControl = distance <= controlRange && (ballController.Owner == null || ballController.Owner == transform);
        if (canControl && ballSocket != null) ballController.TryControl(transform, ballSocket);

        if (Time.time >= nextDecision)
        {
            nextDecision = Time.time + decisionInterval;
            int state = distance < 5f ? 0 : (Vector3.Distance(transform.position, targetGoal.position) < 12f ? 1 : 2);
            currentAction = brain.Choose(state);
        }

        Vector3 destination = DecideDestination(distance);
        MoveTo(destination);

        if (ballController.Owner == transform) ExecuteWithBall();
    }

    private Vector3 DecideDestination(float distance)
    {
        if (currentAction == AdaptiveAIBrain.ActionType.Return || currentAction == AdaptiveAIBrain.ActionType.Mark) return homePosition != null ? homePosition.position : transform.position;
        if (currentAction == AdaptiveAIBrain.ActionType.Advance || (role == Role.Attacker && distance < chaseDistance)) return ball.position;
        if (role == Role.Midfielder && distance < chaseDistance * .8f) return Vector3.Lerp(homePosition != null ? homePosition.position : transform.position, ball.position, .55f);
        if (role == Role.Defender && distance < 7f) return ball.position;
        return homePosition != null ? homePosition.position : transform.position;
    }

    private void ExecuteWithBall()
    {
        float goalDistance = targetGoal == null ? 99f : Vector3.Distance(transform.position, targetGoal.position);
        if (currentAction == AdaptiveAIBrain.ActionType.Shoot && goalDistance < 16f)
        {
            ballController.Shoot((targetGoal.position - ball.position).normalized, shotPower, BallController.ShotType.Ground, .8f);
            brain.AddReward(.15f);
        }
        else if (currentAction == AdaptiveAIBrain.ActionType.Pass && TryPass()) brain.AddReward(.1f);
        else if (currentAction == AdaptiveAIBrain.ActionType.Advance && targetGoal != null && goalDistance < 8f)
            ballController.Shoot((targetGoal.position - ball.position).normalized, shotPower, BallController.ShotType.Ground, .7f);
    }

    private bool TryPass()
    {
        Transform best = null; float bestDistance = float.MaxValue;
        if (teammates != null) foreach (TeamAIController teammate in teammates)
        {
            if (teammate == null || teammate == this) continue;
            float d = Vector3.Distance(transform.position, teammate.transform.position);
            if (d < bestDistance && d <= passRange) { bestDistance = d; best = teammate.transform; }
        }
        if (best == null) return false;
        ballController.Pass((best.position - ball.position).normalized, 7.5f);
        return true;
    }

    private void MoveTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position; direction.y = 0f;
        if (direction.sqrMagnitude < .08f) return;
        direction.Normalize();
        controller.Move(direction * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 7f * Time.deltaTime);
    }
}
