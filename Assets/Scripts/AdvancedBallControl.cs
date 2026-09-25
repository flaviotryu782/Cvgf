using UnityEngine;

/// <summary>
/// Adds a controlled first touch and dynamic dribble point without teleporting
/// the ball. BallController remains responsible for possession and physics.
/// </summary>
[DisallowMultipleComponent]
public sealed class AdvancedBallControl : MonoBehaviour
{
    [Header("First touch")]
    [SerializeField] private float firstTouchDistance = 0.75f;
    [SerializeField] private float firstTouchSpeedFactor = 0.08f;
    [SerializeField] private float firstTouchError = 0.35f;

    [Header("Dribbling")]
    [SerializeField] private float slowDribbleDistance = 0.85f;
    [SerializeField] private float fastDribbleDistance = 1.35f;
    [SerializeField] private float footOffset = 0.32f;
    [SerializeField] private float footSwitchRate = 5f;
    [SerializeField] private float controlSmoothness = 12f;
    [SerializeField] private float highSpeedEscape = 8.5f;

    [Header("Pressure")]
    [SerializeField] private float pressureRadius = 2.4f;
    [SerializeField] private float pressureError = 0.6f;
    [SerializeField] private float protectionOffset = 0.45f;
    [SerializeField] private LayerMask pressureMask = ~0;

    private Transform socket;
    private BallController controlledBall;
    private Transform baseSocket;
    private Vector3 desiredPosition;
    private Vector3 inputDirection;
    private bool leftFoot;

    public bool HasEnhancedControl => controlledBall != null && controlledBall.Owner == transform;

    private void Awake()
    {
        socket = new GameObject("DynamicBallControlSocket").transform;
        socket.SetParent(transform, false);
    }

    private void Update()
    {
        if (!HasEnhancedControl)
            return;

        UpdateDynamicTouchPoint();

        if (controlledBall == null || baseSocket == null)
            return;

        float movementSpeed = GetComponent<CharacterController>() != null
            ? GetComponent<CharacterController>().velocity.magnitude
            : 0f;

        float pressure = GetPressure();
        if (movementSpeed > highSpeedEscape && pressure > 0.7f)
        {
            Vector3 escapeDirection = (transform.forward + GetProtectionDirection() * 0.5f).normalized;
            controlledBall.Shoot(escapeDirection, Mathf.Clamp(movementSpeed * 0.35f, 2f, 6f), BallController.ShotType.Ground);
            ClearControl();
        }
    }

    public bool TryControl(BallController ball, Transform candidateSocket, Vector3 desiredDirection)
    {
        if (ball == null || candidateSocket == null)
            return false;

        if (ball.Owner != null && ball.Owner != transform)
            return false;

        controlledBall = ball;
        baseSocket = candidateSocket;
        inputDirection = Flatten(desiredDirection);
        if (inputDirection.sqrMagnitude < 0.01f)
            inputDirection = transform.forward;

        inputDirection.Normalize();
        leftFoot = Vector3.Dot(transform.right, inputDirection) < 0f;

        float pressure = GetPressure();
        Vector3 incoming = ball.Body != null ? ball.Body.velocity : Vector3.zero;
        Vector3 firstTouchDirection = Vector3.Slerp(inputDirection, incoming.normalized, 0.35f);
        if (firstTouchDirection.sqrMagnitude < 0.01f)
            firstTouchDirection = inputDirection;

        float error = firstTouchError + pressure * pressureError;
        Vector3 errorVector = new Vector3(
            Random.Range(-error, error),
            0f,
            Random.Range(-error, error));

        desiredPosition = candidateSocket.position
            + firstTouchDirection.normalized * firstTouchDistance
            + errorVector
            + GetProtectionDirection() * protectionOffset * pressure;

        socket.position = desiredPosition;
        socket.rotation = transform.rotation;

        if (!ball.TryControl(transform, socket))
        {
            ClearControl();
            return false;
        }

        return true;
    }

    private void UpdateDynamicTouchPoint()
    {
        Vector3 direction = inputDirection.sqrMagnitude > 0.01f ? inputDirection : transform.forward;
        float speed = GetComponent<CharacterController>() != null
            ? GetComponent<CharacterController>().velocity.magnitude
            : 0f;
        float normalizedSpeed = Mathf.Clamp01(speed / 8f);
        float distance = Mathf.Lerp(slowDribbleDistance, fastDribbleDistance, normalizedSpeed);
        float side = leftFoot ? -footOffset : footOffset;
        float pressure = GetPressure();

        Vector3 target = baseSocket.position
            + direction * distance
            + transform.right * side
            + GetProtectionDirection() * protectionOffset * pressure;
        target.y = baseSocket.position.y;

        socket.position = Vector3.Lerp(socket.position, target, controlSmoothness * Time.deltaTime);
        socket.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (Time.time % footSwitchRate < Time.deltaTime)
            leftFoot = !leftFoot;
    }

    private float GetPressure()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pressureRadius, pressureMask, QueryTriggerInteraction.Ignore);
        int opponents = 0;
        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform || hit.CompareTag("Teammate"))
                continue;
            opponents++;
        }

        return Mathf.Clamp01(opponents / 2f);
    }

    private Vector3 GetProtectionDirection()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pressureRadius, pressureMask, QueryTriggerInteraction.Ignore);
        Vector3 away = Vector3.zero;
        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform || hit.CompareTag("Teammate"))
                continue;
            Vector3 difference = transform.position - hit.transform.position;
            difference.y = 0f;
            if (difference.sqrMagnitude > 0.001f)
                away += difference.normalized / difference.sqrMagnitude;
        }

        return away.sqrMagnitude > 0.001f ? away.normalized : -transform.forward;
    }

    private Vector3 Flatten(Vector3 value)
    {
        value.y = 0f;
        return value;
    }

    private void ClearControl()
    {
        controlledBall = null;
        baseSocket = null;
    }

    private void OnDisable()
    {
        if (controlledBall != null && controlledBall.Owner == transform)
            controlledBall.ReleaseControl();
        ClearControl();
    }
}
