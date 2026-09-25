using UnityEngine;

public class GoalkeeperAdvancedAI : MonoBehaviour
{
    [SerializeField] private GoalkeeperController goalkeeper;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform goalCenter;
    [SerializeField] private float reactionDelay = .12f;
    [SerializeField] private float predictionTime = .45f;
    [SerializeField] private float diveSpeed = 7f;
    [SerializeField] private float horizontalLimit = 4.5f;
    private float nextReaction;
    private Vector3 predicted;

    private void Update()
    {
        if (ball == null || goalCenter == null) return;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (Time.time >= nextReaction)
        {
            nextReaction = Time.time + reactionDelay;
            predicted = ball.position + (rb != null ? rb.velocity * predictionTime : Vector3.zero);
        }
        Vector3 target = new Vector3(Mathf.Clamp(predicted.x, -horizontalLimit, horizontalLimit), transform.position.y, goalCenter.position.z);
        Vector3 direction = target - transform.position; direction.y = 0f;
        if (direction.sqrMagnitude > .05f) transform.position = Vector3.MoveTowards(transform.position, target, diveSpeed * Time.deltaTime);
        if (rb != null && rb.velocity.magnitude > 8f && Vector3.Distance(transform.position, predicted) < 1.4f) goalkeeper?.PlayPenaltySave();
    }
}
