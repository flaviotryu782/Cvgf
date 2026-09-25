using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5.5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 12f;
    [Header("Ball")]
    [SerializeField] private Transform ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private Transform enemyGoal;
    [SerializeField] private float controlRange = 2.1f;
    [SerializeField] private float shotPower = 13f;
    [SerializeField] private float passPower = 8f;

    private CharacterController controller;
    private void Awake() => controller = GetComponent<CharacterController>();

    private void Update()
    {
        Vector2 input = MobileInput.Instance != null ? MobileInput.Instance.Move : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        if (direction.sqrMagnitude > 1f) direction.Normalize();
        bool sprint = MobileInput.Instance != null ? MobileInput.Instance.SprintHeld : Input.GetKey(KeyCode.LeftShift);
        controller.Move(direction * (sprint ? sprintSpeed : speed) * Time.deltaTime);
        if (direction.sqrMagnitude > .01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

        bool kick = MobileInput.Instance != null ? MobileInput.Instance.ConsumeKick() : Input.GetKeyDown(KeyCode.Space);
        bool pass = MobileInput.Instance != null ? MobileInput.Instance.ConsumePass() : Input.GetKeyDown(KeyCode.LeftControl);
        if (ball != null && Vector3.Distance(transform.position, ball.position) <= controlRange)
        {
            if (kick) ballController.Kick((enemyGoal.position - ball.position).normalized, shotPower);
            else if (pass) ballController.Kick(transform.forward, passPower);
        }
    }
}
