using UnityEngine;

public sealed class GoalkeeperAdvancedAI : MonoBehaviour
{
    [SerializeField] private GoalkeeperController goalkeeper;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform goalCenter;
    [SerializeField] private float activeDistance = 22f;
    [SerializeField] private float minimumShotSpeed = 8f;
    [SerializeField] private float cueCooldown = 0.5f;

    private float nextCueTime;

    private void Reset()
    {
        goalkeeper = GetComponent<GoalkeeperController>();
    }

    private void Awake()
    {
        if (goalkeeper == null)
            goalkeeper = GetComponent<GoalkeeperController>();
    }

    private void Update()
    {
        if (goalkeeper == null || ball == null || goalCenter == null || Time.time < nextCueTime)
            return;

        Rigidbody body = ball.GetComponent<Rigidbody>();
        if (body == null || body.velocity.magnitude < minimumShotSpeed)
            return;

        float distance = Vector3.Distance(ball.position, goalCenter.position);
        Vector3 toGoal = goalCenter.position - ball.position;
        float angle = Vector3.Dot(body.velocity.normalized, toGoal.normalized);

        if (distance <= activeDistance && angle > 0.65f)
        {
            nextCueTime = Time.time + cueCooldown;
            goalkeeper.PlayPenaltySave();
        }
    }
}
