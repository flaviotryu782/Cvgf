using UnityEngine;

public class MobileGameplayActions : MonoBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private float protectionDistance = 1.8f;
    [SerializeField] private float dribbleDistance = 1.4f;
    [SerializeField] private float dribbleForce = 2.5f;
    private bool protecting;
    private bool dribbling;

    public void SetProtection(bool value) => protecting = value;
    public void SetDribble(bool value) => dribbling = value;
    private void Update()
    {
        if (ball == null) return;
        if (protecting && Vector3.Distance(transform.position, ball.position) <= protectionDistance)
        {
            Vector3 shield = Vector3.Cross(transform.forward, Vector3.up).normalized;
            if (ball.Body != null) ball.Body.AddForce(shield * .4f, ForceMode.Acceleration);
        }
        if (dribbling && Vector3.Distance(transform.position, ball.position) <= dribbleDistance)
        {
            animationController?.PlayDribble();
            if (ball.Body != null) ball.Body.AddForce(transform.forward * dribbleForce, ForceMode.Acceleration);
        }
    }
}
