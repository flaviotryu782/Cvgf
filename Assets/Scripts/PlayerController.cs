using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 24f;
    [SerializeField] private float maxPlayerSpeed = 8f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private BallController ballController;
    [SerializeField] private float controlRange = 2.2f;
    [SerializeField] private float dribbleReduction = 0.7f;

    private CharacterController controller;
    private PlayerAnimationController animationController;
    private Vector3 currentVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animationController = GetComponentInChildren<PlayerAnimationController>();
        if (GetComponent<FootballContactPhysics>() == null)
            gameObject.AddComponent<FootballContactPhysics>();
    }

    private void Update()
    {
        Vector2 input = MobileInput.Instance != null ? MobileInput.Instance.Move : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        bool sprinting = input.sqrMagnitude > 0.01f && Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = sprinting ? sprintSpeed : speed;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            if (ballController != null && ballController.Owner == transform)
                targetSpeed *= dribbleReduction;
            currentVelocity = Vector3.MoveTowards(currentVelocity, direction * targetSpeed, acceleration * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (currentVelocity.magnitude > maxPlayerSpeed)
            currentVelocity = currentVelocity.normalized * maxPlayerSpeed;

        controller.Move(currentVelocity * Time.deltaTime);
        animationController?.SetLocomotion(currentVelocity.magnitude / Mathf.Max(sprintSpeed, 0.01f), sprinting);
        TryTakeBall();
    }

    private void TryTakeBall()
    {
        if (ball == null || ballController == null || ballSocket == null)
            return;
        if (Vector3.Distance(transform.position, ball.position) <= controlRange && (ballController.Owner == null || ballController.Owner == transform))
            ballController.TryControl(transform, ballSocket);
    }
}
