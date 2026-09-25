using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 22f;
    private Rigidbody body;
    private Vector3 initialPosition;

    public Rigidbody Body => body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (body.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            body.velocity = body.velocity.normalized * maxSpeed;
    }

    public void Kick(Vector3 direction, float power)
    {
        direction.y = 0f;
        body.velocity = new Vector3(body.velocity.x * .25f, body.velocity.y, body.velocity.z * .25f);
        body.AddForce(direction.normalized * power, ForceMode.Impulse);
    }

    public void ResetBall(Vector3? position = null)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = position ?? initialPosition;
    }
}
