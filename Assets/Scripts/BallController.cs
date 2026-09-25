using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float possessionDistance = 2.15f;
    [SerializeField] private float followSharpness = 22f;
    [SerializeField] private float dribbleForward = 1.2f;
    [SerializeField] private float drag = 0.8f;

    private Rigidbody body;
    private Vector3 initialPosition;
    private Transform owner;
    private Transform controlSocket;
    private float dribbleTime;
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
            dribbleTime += Time.fixedDeltaTime * 9f;
            Vector3 target = controlSocket.position + owner.forward * dribbleForward;
            Vector3 delta = target - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude > 0.01f)
            {
                Vector3 followVelocity = delta * followSharpness;
                body.velocity = Vector3.Lerp(body.velocity, followVelocity, 0.25f);
            }
            else
            {
                body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.15f);
            }
        }
    }

    public bool TryControl(Transform newOwner, Transform socket)
    {
        if (newOwner == null || socket == null)
            return false;

        if (Time.time - lastKickTime < 0.12f)
            return false;

        if (owner != null && owner != newOwner)
            return false;

        if (Vector3.Distance(transform.position, socket.position) > possessionDistance + 0.35f)
            return false;

        owner = newOwner;
        controlSocket = socket;
        return true;
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
        body.velocity = direction * shotPower * charge;

        switch (type)
        {
            case ShotType.Lob:
                body.velocity += Vector3.up * 2.5f;
                break;
            case ShotType.Curve:
                body.velocity += new Vector3(direction.z, 0f, -direction.x) * 0.6f;
                break;
        }
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
