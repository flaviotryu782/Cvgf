using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public enum ShotType { Ground, Lob, Curve }

    [Header("Physics")]
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float possessionDistance = 2.15f;
    [SerializeField] private float followSharpness = 22f;
    [SerializeField] private float dribbleForward = 0.85f;
    [SerializeField] private float dribbleHeight = 0.16f;
    [SerializeField] private float curveStrength = 5f;
    [SerializeField] private float lobLift = 5.5f;

    private Rigidbody body;
    private Vector3 initialPosition;
    private Transform owner;
    private Transform controlSocket;
    private float dribbleTime;
    private float lastKickTime = -10f;

    public Rigidbody Body => body;
    public Transform Owner => owner;
    public bool IsControlled => owner != null;
    public event Action<Transform> PossessionChanged;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        if (owner != null && controlSocket != null)
        {
            dribbleTime += Time.fixedDeltaTime * 9f;
            Vector3 target = controlSocket.position + owner.forward * dribbleForward;
            target.y += Mathf.Abs(Mathf.Sin(dribbleTime)) * dribbleHeight;
            body.velocity = Vector3.Lerp(body.velocity, (target - body.position) * followSharpness, Time.fixedDeltaTime * 0.8f);
            body.angularVelocity = Vector3.zero;
            body.MovePosition(Vector3.Lerp(body.position, target, Time.fixedDeltaTime * followSharpness));
            return;
        }

        if (body.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            body.velocity = body.velocity.normalized * maxSpeed;
    }

    public bool TryControl(Transform newOwner, Transform socket)
    {
        if (newOwner == null || socket == null || Time.time - lastKickTime < .12f) return false;
        if (owner != null && owner != newOwner) return false;
        if (Vector3.Distance(transform.position, socket.position) > possessionDistance) return false;
        owner = newOwner;
        controlSocket = socket;
        body.isKinematic = false;
        body.velocity = Vector3.zero;
        PossessionChanged?.Invoke(owner);
        return true;
    }

    public void ReleaseControl()
    {
        owner = null;
        controlSocket = null;
        PossessionChanged?.Invoke(null);
    }

    public void Shoot(Vector3 direction, float power, ShotType type, float charge = 1f)
    {
        ReleaseControl();
        lastKickTime = Time.time;
        direction.y = 0f;
        if (direction.sqrMagnitude < .001f) direction = transform.forward;
        direction.Normalize();
        body.velocity = new Vector3(body.velocity.x * .12f, 0f, body.velocity.z * .12f);
        Vector3 impulse = direction * power * Mathf.Clamp01(charge);
        if (type == ShotType.Lob) impulse.y += lobLift * Mathf.Clamp01(charge);
        body.AddForce(impulse, ForceMode.Impulse);
        if (type == ShotType.Curve) body.AddTorque(Vector3.up * curveStrength * Mathf.Clamp01(charge), ForceMode.Impulse);
    }

    public void Kick(Vector3 direction, float power) => Shoot(direction, power, ShotType.Ground, 1f);

    public void Pass(Vector3 direction, float power) => Shoot(direction, power, ShotType.Ground, 1f);

    public void ResetBall(Vector3? position = null)
    {
        ReleaseControl();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = position ?? initialPosition;
        lastKickTime = Time.time;
    }
}
