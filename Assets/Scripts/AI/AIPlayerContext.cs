using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5.5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 24f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Ball control")]
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
    }

    private void Update()
    {
        Vector2 input = MobileInput.Instance != null ? MobileInput.Instance.Move : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 desiredDirection = new Vector3(input.x, 0f, input.y);

        float targetSpeed = (input.sqrMagnitude > 0.01f) ? speed : 0f;
        bool sprinting = input.sqrMagnitude > 0.01f && (Input.GetKey(KeyCode.LeftShift) || (MobileInput.Instance != null && MobileInput.Instance.IsSprinting));

        if (sprinting)
            targetSpeed = sprintSpeed;

        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            desiredDirection.Normalize();
            Vector3 targetVelocity = desiredDirection * targetSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (ballController != null && ballController.Owner == transform)
        {
            currentVelocity *= dribbleReduction;
        }

        controller.Move(currentVelocity * Time.deltaTime);
        animationController?.SetLocomotion(currentVelocity.magnitude / Mathf.Max(sprintSpeed, 0.01f), sprinting);

        TryTakeBall();
    }

    private void TryTakeBall()
    {
        if (ball == null || ballController == null || ballSocket == null)
            return;

        if (Vector3.Distance(transform.position, ball.position) <= controlRange && (ballController.Owner == null || ballController.Owner == transform))
        {
            ballController.TryControl(transform, ballSocket);
        }
    }
}
