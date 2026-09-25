using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    public enum ActionState { Locomotion, Kick, Pass, Celebrate, Fall, Defend, Dive }

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float locomotionDamp = .12f;
    [SerializeField] private float actionCooldown = .15f;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Sprint = Animator.StringToHash("Sprint");
    private static readonly int Kick = Animator.StringToHash("Kick");
    private static readonly int Pass = Animator.StringToHash("Pass");
    private static readonly int Celebrate = Animator.StringToHash("Celebrate");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Defend = Animator.StringToHash("Defend");
    private static readonly int Dive = Animator.StringToHash("Dive");

    private float nextActionTime;

    private void Reset() => animator = GetComponent<Animator>();
    private void Awake() { if (animator == null) animator = GetComponent<Animator>(); }

    public void SetLocomotion(float speed01, bool sprinting)
    {
        if (animator == null) return;
        animator.SetFloat(Speed, Mathf.Clamp01(speed01), locomotionDamp, Time.deltaTime);
        animator.SetBool(Sprint, sprinting);
    }

    public void PlayKick() { if (CanPlayAction()) animator.SetTrigger(Kick); }
    public void PlayPass() { if (CanPlayAction()) animator.SetTrigger(Pass); }
    public void PlayCelebrate() { if (CanPlayAction()) animator.SetTrigger(Celebrate); }
    public void PlayFall() { if (CanPlayAction()) animator.SetTrigger(Fall); }
    public void PlayDefend() { if (CanPlayAction()) animator.SetTrigger(Defend); }
    public void PlayDive() { if (CanPlayAction()) animator.SetTrigger(Dive); }

    private bool CanPlayAction()
    {
        if (animator == null || Time.time < nextActionTime) return false;
        nextActionTime = Time.time + actionCooldown;
        return true;
    }
}
