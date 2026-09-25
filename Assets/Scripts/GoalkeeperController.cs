using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GoalkeeperController : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Transform goalCenter;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float xLimit = 4.5f;
    [SerializeField] private float zLimit = 1.5f;
    [SerializeField] private float saveDistance = 1.3f;
    [SerializeField] private float predictionTime = .35f;
    private CharacterController controller;

    private void Awake() => controller = GetComponent<CharacterController>();
    private void Update()
    {
        if (ball == null || goalCenter == null) return;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 predicted = ball.position + (rb != null ? rb.velocity * predictionTime : Vector3.zero);
        Vector3 target = new Vector3(Mathf.Clamp(predicted.x, -xLimit, xLimit), transform.position.y, goalCenter.position.z);
        Vector3 direction = target - transform.position; direction.y = 0f;
        if (direction.sqrMagnitude > .04f) controller.Move(direction.normalized * moveSpeed * Time.deltaTime);
        Vector3 p = transform.position; p.x = Mathf.Clamp(p.x, -xLimit, xLimit); p.z = Mathf.Clamp(p.z, goalCenter.position.z - zLimit, goalCenter.position.z + zLimit); transform.position = p;
        if (Vector3.Distance(transform.position, ball.position) < saveDistance && rb != null) { rb.AddForce((ball.position - transform.position).normalized * 7f, ForceMode.Impulse); }
    }
}
