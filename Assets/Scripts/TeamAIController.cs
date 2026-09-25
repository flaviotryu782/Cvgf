using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeamAIController : MonoBehaviour
{
    public enum Role { Chaser, Support }
    [SerializeField] private Role role = Role.Chaser;
    [SerializeField] private Transform ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private Transform targetGoal;
    [SerializeField] private Transform homePosition;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private float moveSpeed = 4.2f;
    [SerializeField] private float chaseDistance = 18f;
    [SerializeField] private float controlRange = 2f;
    [SerializeField] private float shotPower = 10f;
    [SerializeField] private float dribbleDistance = 4f;

    private CharacterController controller;
    private void Awake() => controller = GetComponent<CharacterController>();
    private void Update()
    {
        if (ball == null || ballController == null) return;
        float distance = Vector3.Distance(transform.position, ball.position);
        bool canControl = distance <= controlRange && (ballController.Owner == null || ballController.Owner == transform);
        if (canControl) ballController.TryControl(transform, ballSocket);
        Vector3 destination = role == Role.Chaser && distance <= chaseDistance ? ball.position : (homePosition != null ? homePosition.position : transform.position);
        Vector3 direction = destination - transform.position; direction.y = 0f;
        if (direction.sqrMagnitude > .08f) { direction.Normalize(); controller.Move(direction * moveSpeed * Time.deltaTime); transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 7f * Time.deltaTime); }
        if (ballController.Owner == transform && Vector3.Distance(transform.position, targetGoal.position) <= dribbleDistance)
            ballController.Shoot((targetGoal.position - ball.position).normalized, shotPower, BallController.ShotType.Ground, .8f);
    }
}
