using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DefensiveActionController : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private float tackleRange = 2f;
    [SerializeField] private float tackleForce = 5f;
    [SerializeField] private float slideRange = 2.8f;
    [SerializeField] private float slideCooldown = 1.2f;
    [SerializeField] private float pressureRange = 4f;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private FoulSystem foulSystem;
    private CharacterController controller;
    private float nextSlide;

    public bool IsPressuring { get; private set; }
    public bool IsSliding { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animationController == null) animationController = GetComponentInChildren<PlayerAnimationController>();
    }

    private void Update()
    {
        if (ball == null) return;
        IsPressuring = Vector3.Distance(transform.position, ball.position) <= pressureRange;
        IsSliding = false;
        bool tackle = MobileInput.Instance != null ? MobileInput.Instance.ConsumeTackle() : Input.GetKeyDown(KeyCode.E);
        bool slide = MobileInput.Instance != null ? MobileInput.Instance.ConsumeSlide() : Input.GetKeyDown(KeyCode.Q);
        if (tackle) AttemptTackle(false);
        if (slide) AttemptTackle(true);
    }

    public void AttemptTackle(bool slide)
    {
        if (ball == null || (slide && Time.time < nextSlide)) return;
        float range = slide ? slideRange : tackleRange;
        if (Vector3.Distance(transform.position, ball.position) > range) return;
        Vector3 toBall = ball.position - transform.position;
        if (Vector3.Dot(transform.forward, toBall.normalized) < (slide ? -.2f : .05f)) return;
        if (slide)
        {
            IsSliding = true;
            nextSlide = Time.time + slideCooldown;
            animationController?.PlaySlide();
        }
        else animationController?.PlayTackle();

        Rigidbody body = ball.GetComponent<Rigidbody>();
        if (body != null)
        {
            Vector3 impulse = (transform.forward + Vector3.up * (slide ? .08f : .02f)).normalized * tackleForce;
            body.AddForce(impulse, ForceMode.Impulse);
        }
        foulSystem?.RegisterChallenge(this, slide);
    }
}
