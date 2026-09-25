using UnityEngine;

public class FoulSystem : MonoBehaviour
{
    [SerializeField] private PenaltyShootoutManager penaltyManager;
    [SerializeField, Range(0f, 1f)] private float foulChance = .08f;
    [SerializeField] private float cooldown = 4f;
    private float nextFoulTime;

    public void CheckContact(float relativeSpeed)
    {
        if (Time.time < nextFoulTime || penaltyManager == null) return;
        if (relativeSpeed >= 4f && Random.value < foulChance)
        {
            nextFoulTime = Time.time + cooldown;
            penaltyManager.StartShootout();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null) CheckContact(body.velocity.magnitude);
    }
}
