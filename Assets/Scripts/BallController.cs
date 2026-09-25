using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Ball movement")]
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float possessionDistance = 2.15f;
    [SerializeField] private float followSharpness = 22f;
    [SerializeField] private float dribbleForward = 1.2f;
    [SerializeField] private float drag = 0.8f;
    [SerializeField] private float ballLift = 0.15f;
    [SerializeField] private float controlReleaseTime = 0.12f;

    [Header("Dribble feel")]
    [SerializeField] private float dribbleSway = 3.5f;
    [SerializeField] private float possessionBias = 0.22f;
    [SerializeField] private float ballGroundOffset = 0.12f;

    [Header("Pressure and stealing")]
    [SerializeField] private float stealRadius = 1.5f;
    [SerializeField] private float stealBias = 0.8f;
    [SerializeField] private float pressurePush = 4f;

    private Rigidbody body;
    private Vector3 initialPosition;
    private Transform owner;
    private Transform controlSocket;
    private float lastKickTime = -10f;

    public Rigidbody Body => body;
    public Transform Owner => owner;
    public bool IsControlled => owner != null;
    public bool IsOwner(Transform candidate) => owner != null && owner == candidate;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.drag = drag;
    }

    private void FixedUpdate()
    {
        if (owner != null && controlSocket != null)
        {
            Vector3 target = controlSocket.position + owner.forward * dribbleForward;
            target.y = controlSocket.position.y + ballGroundOffset + Mathf.Sin(Time.time * dribbleSway) * ballLift;

            Vector3 delta = target - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude > 0.01f)
            {
                Vector3 followVelocity = delta * followSharpness;
                body.velocity = Vector3.Lerp(body.velocity, followVelocity, possessionBias);
            }
            else
            {
                body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.12f);
            }
        }
    }

    public bool TryControl(Transform newOwner, Transform socket)
    {
        if (newOwner == null || socket == null)
            return false;

        if (Time.time - lastKickTime < controlReleaseTime)
            return false;

        if (owner != null && owner != newOwner)
        {
            if (TrySteal(newOwner, 1f))
                return true;
            return false;
        }

        if (Vector3.Distance(transform.position, socket.position) > possessionDistance + 0.35f)
            return false;

        owner = newOwner;
        controlSocket = socket;
        return true;
    }

    public bool TrySteal(Transform challenger, float pressure = 1f)
    {
        if (challenger == null || owner == null)
            return false;

        if (Time.time - lastKickTime < controlReleaseTime)
            return false;

        float distanceToChallenger = Vector3.Distance(transform.position, challenger.position);
        float effectiveRadius = stealRadius + pressure * 0.5f;
        if (distanceToChallenger > effectiveRadius)
            return false;

        Vector3 attackerDirection = challenger.position - owner.position;
        Vector3 ballDirection = transform.position - owner.position;

        float challengeWeight = Vector3.Dot(attackerDirection.normalized, ballDirection.normalized);
        float pressureValue = Mathf.Clamp01(1f - (distanceToChallenger / effectiveRadius));

        if (challengeWeight > -0.3f && pressureValue > 0.2f)
        {
            owner = challenger;
            controlSocket = null;
            body.velocity += (challenger.position - transform.position).normalized * pressurePush * pressure;
            return true;
        }

        return false;
    }

    public void ReleaseControl()
    {
        owner = null;
        controlSocket = null;
    }

    public void Shoot(Vector3 direction, float power, ShotType type, float charge = 1f)
    {
        ReleaseControl();
        lastKickTime = Time.time;

        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
            direction = Vector3.forward;

        direction.Normalize();

        float shotPower = Mathf.Clamp(power, 0f, maxSpeed);
        Vector3 shotVelocity = direction * shotPower * charge;

        switch (type)
        {
            case ShotType.Lob:
                shotVelocity += Vector3.up * 2.5f;
                break;
            case ShotType.Curve:
                shotVelocity += new Vector3(direction.z, 0f, -direction.x) * 0.6f;
                break;
        }

        body.velocity = shotVelocity;
    }

    public void Kick(Vector3 direction, float power) => Shoot(direction, power, ShotType.Ground, 1f);

    public void Pass(Vector3 direction, float power)
    {
        AudioManager.Instance?.PlayPass();
        Shoot(direction, power, ShotType.Ground, 1f);
    }

    public void ResetBall(Vector3? position = null)
    {
        ReleaseControl();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = position ?? initialPosition;
        lastKickTime = -10f;
    }

    public enum ShotType
    {
        Ground,
        Lob,
        Curve
    }
}
