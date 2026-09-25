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
    [SerializeField] private float groundFriction = 1.2f;
    [SerializeField] private float airDrag = 0.08f;
    [SerializeField] private float groundBounce = 0.35f;
    [SerializeField] private float maxBounceSpeed = 7f;

    [Header("Dribble feel")]
    [SerializeField] private float dribbleSway = 3.5f;
    [SerializeField] private float possessionBias = 0.22f;
    [SerializeField] private float ballGroundOffset = 0.12f;

    [Header("Pressure and stealing")]
    [SerializeField] private float stealRadius = 1.5f;
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
        body.maxAngularVelocity = 35f;
    }

    private void FixedUpdate()
    {
        if (owner != null && controlSocket != null)
        {
            Vector3 target = controlSocket.position + owner.forward * dribbleForward;
            target.y = controlSocket.position.y + ballGroundOffset + Mathf.Sin(Time.time * dribbleSway) * ballLift;
            Vector3 delta = target - transform.position;
            delta.y = 0f;
            body.velocity = delta.sqrMagnitude > 0.01f
                ? Vector3.Lerp(body.velocity, delta * followSharpness, possessionBias)
                : Vector3.Lerp(body.velocity, Vector3.zero, 0.12f);
            return;
        }

        bool airborne = transform.position.y > initialPosition.y + 0.22f || Mathf.Abs(body.velocity.y) > 0.35f;
        body.drag = airborne ? airDrag : groundFriction;
        Vector3 velocity = body.velocity;
        if (!airborne)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, groundFriction * Time.fixedDeltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0f, groundFriction * Time.fixedDeltaTime);
        }
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;
        body.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount == 0) return;
        ContactPoint contact = collision.GetContact(0);
        if (contact.normal.y > 0.55f && body.velocity.y > maxBounceSpeed)
        {
            Vector3 velocity = body.velocity;
            velocity.y = maxBounceSpeed * groundBounce;
            body.velocity = velocity;
        }
    }

    public bool TryControl(Transform newOwner, Transform socket)
    {
        if (newOwner == null || socket == null || Time.time - lastKickTime < controlReleaseTime)
            return false;
        if (owner != null && owner != newOwner)
            return TrySteal(newOwner, 1f);
        if (Vector3.Distance(transform.position, socket.position) > possessionDistance + 0.35f)
            return false;
        owner = newOwner;
        controlSocket = socket;
        body.velocity = Vector3.zero;
        return true;
    }

    public bool TrySteal(Transform challenger, float pressure = 1f)
    {
        if (challenger == null || owner == null || Time.time - lastKickTime < controlReleaseTime)
            return false;
        float radius = stealRadius + pressure * 0.5f;
        float distance = Vector3.Distance(transform.position, challenger.position);
        if (distance > radius) return false;
        Vector3 attackerDirection = challenger.position - owner.position;
        Vector3 ballDirection = transform.position - owner.position;
        float alignment = Vector3.Dot(attackerDirection.normalized, ballDirection.normalized);
        float pressureValue = Mathf.Clamp01(1f - distance / radius);
        if (alignment <= -0.3f || pressureValue <= 0.2f) return false;
        owner = challenger;
        controlSocket = null;
        body.velocity += (challenger.position - transform.position).normalized * pressurePush * pressure;
        return true;
    }

    public void ReleaseControl() { owner = null; controlSocket = null; }

    public void Shoot(Vector3 direction, float power, ShotType type, float charge = 1f)
    {
        Shoot(direction, power, type, 1f, 0f, ShotTechnique.Ground, 0f, false, charge);
    }

    public void Shoot(Vector3 direction, float power, ShotType type, float accuracy, float angle, ShotTechnique technique, float pressure, bool firstTouch, float charge = 1f)
    {
        ReleaseControl();
        lastKickTime = Time.time;
        Vector3 shotDirection = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
        Vector3 spreadDirection = Quaternion.Euler(0f, angle, 0f) * shotDirection;
        Vector3 finalDirection = Vector3.Lerp(shotDirection, spreadDirection, Mathf.Clamp01(1f - accuracy));
        finalDirection.y = 0f;
        finalDirection.Normalize();
        Vector3 shotVelocity = finalDirection * Mathf.Clamp(power, 0f, maxSpeed) * charge;
        if (technique == ShotTechnique.Lob || technique == ShotTechnique.Header || technique == ShotTechnique.Volley)
            shotVelocity += Vector3.up * (technique == ShotTechnique.Header ? 3.2f : 2.8f);
        if (technique == ShotTechnique.Curve)
            shotVelocity += new Vector3(finalDirection.z, 0f, -finalDirection.x) * (0.9f + pressure * 0.25f);
        if (technique == ShotTechnique.Place) shotVelocity *= 0.88f;
        if (technique == ShotTechnique.Power) shotVelocity *= 1.14f;
        if (technique == ShotTechnique.FirstTouch || firstTouch) shotVelocity += Vector3.up * 0.85f;
        if (pressure > 0.4f)
        {
            float deviation = Mathf.Clamp01(pressure) * 1.2f;
            shotVelocity += new Vector3(Random.Range(-deviation, deviation), 0f, Random.Range(-deviation, deviation));
        }
        if (type == ShotType.Lob) shotVelocity += Vector3.up * 2.5f;
        if (type == ShotType.Curve) shotVelocity += new Vector3(finalDirection.z, 0f, -finalDirection.x) * 0.6f;
        body.velocity = shotVelocity;
    }

    public void Pass(Vector3 direction, float power)
    {
        AudioManager.Instance?.PlayPass();
        Shoot(direction, power, ShotType.Ground, 1f);
    }

    public void Pass(Vector3 direction, PassProfile profile)
    {
        AudioManager.Instance?.PlayPass();
        ShotType type = profile.technique == PassTechnique.Lofted ? ShotType.Lob : ShotType.Ground;
        float lift = profile.technique == PassTechnique.Lofted ? 1f : 0f;
        Shoot(direction, profile.power, type, profile.accuracy, 0f, ShotTechnique.Ground, profile.pressure, profile.technique == PassTechnique.FirstTime, lift);
    }

    public void Kick(Vector3 direction, float power) => Shoot(direction, power, ShotType.Ground);

    public void ResetBall(Vector3? position = null)
    {
        ReleaseControl();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = position ?? initialPosition;
        lastKickTime = -10f;
    }

    public enum ShotType { Ground, Lob, Curve }
}
