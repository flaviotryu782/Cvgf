using UnityEngine;

public class ManMarkingAI : MonoBehaviour
{
    [SerializeField] private Transform marker;
    [SerializeField] private Transform target;
    [SerializeField] private float markDistance = 2f;
    [SerializeField] private float moveSpeed = 4f;

    private CharacterController controller;
    private void Awake() => controller = GetComponent<CharacterController>();
    private void Update()
    {
        if (marker == null || target == null || controller == null) return;
        Vector3 desired = target.position - target.forward * markDistance;
        Vector3 direction = desired - marker.position; direction.y = 0f;
        if (direction.sqrMagnitude < .1f) return;
        direction.Normalize();
        controller.Move(direction * moveSpeed * Time.deltaTime);
        marker.rotation = Quaternion.Slerp(marker.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
    }
}
