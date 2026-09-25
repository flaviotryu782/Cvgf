using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeamAIController : MonoBehaviour
{
    public enum Role { Attacker, Midfielder, Defender }
    [SerializeField] private Role role = Role.Midfielder;
    [SerializeField] private Transform ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private Transform targetGoal;
    [SerializeField] private Transform homePosition;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private float moveSpeed = 4.2f;
    [SerializeField] private float chaseDistance = 18f;
    [SerializeField] private float controlRange = 2f;
    [SerializeField] private float shotPower = 10f;
    [SerializeField] private float supportDistance = 7f;
    [SerializeField] private float decisionInterval = .25f;

    private CharacterController controller;
    private float nextDecision;
    private void Awake() => controller = GetComponent<CharacterController>();

    private void Update()
    {
        if (ball == null || ballController == null) return;
        float distance = Vector3.Distance(transform.position, ball.position);
        bool canControl = distance <= controlRange && (ballController.Owner == null || ballController.Owner == transform);
        if (canControl && ballSocket != null) ballController.TryControl(transform, ballSocket);

        Vector3 destination = GetDestination(distance);
        Vector3 direction = destination - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > .08f)
        {
            direction.Normalize();
            controller.Move(direction * moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 7f * Time.deltaTime);
        }

        if (ballController.Owner == transform && Time.time >= nextDecision)
        {
            nextDecision = Time.time + decisionInterval;
            float distanceToGoal = targetGoal == null ? 99f : Vector3.Distance(transform.position, targetGoal.position);
            if (role == Role.Attacker && distanceToGoal < 10f)
                ballController.Shoot((targetGoal.position - ball.position).normalized, shotPower, BallController.ShotType.Ground, .8f);
        }
    }

    private Vector3 GetDestination(float ballDistance)
    {
        if (role == Role.Attacker && ballDistance <= chaseDistance) return ball.position;
        if (role == Role.Midfielder && ballDistance <= chaseDistance * .75f) return Vector3.Lerp(homePosition != null ? homePosition.position : transform.position, ball.position, .65f);
        if (role == Role.Defender && ballDistance <= supportDistance) return ball.position;
        return homePosition != null ? homePosition.position : transform.position;
    }
}
