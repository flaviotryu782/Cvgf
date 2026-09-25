using UnityEngine;

/// <summary>
/// Adds stable contact behavior to CharacterController players without enabling
/// rigidbody-style pushing between teammates.
/// </summary>
[DisallowMultipleComponent]
public sealed class FootballContactPhysics : MonoBehaviour
{
    [SerializeField] private float maxImpactSpeed = 3.5f;
    [SerializeField] private float teammatePush = 0.08f;
    [SerializeField] private float opponentPush = 0.22f;
    [SerializeField] private float impactCooldown = 0.08f;

    private CharacterController controller;
    private float nextImpactTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null || hit.collider.attachedRigidbody == null)
            return;

        if (Time.time < nextImpactTime)
            return;

        Rigidbody otherBody = hit.collider.attachedRigidbody;
        Vector3 relativeVelocity = controller != null ? controller.velocity : Vector3.zero;
        float impactSpeed = Mathf.Min(relativeVelocity.magnitude, maxImpactSpeed);

        if (impactSpeed <= 0.05f)
            return;

        bool teammate = hit.collider.CompareTag("Teammate");
        float push = teammate ? teammatePush : opponentPush;
        Vector3 direction = hit.moveDirection;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            otherBody.AddForce(direction.normalized * impactSpeed * push, ForceMode.VelocityChange);
            nextImpactTime = Time.time + impactCooldown;
        }
    }
}
