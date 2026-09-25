using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    public enum Foot { Right, Left }
    public enum CelebrationStyle { Jump, Slide, ArmsUp, Point, KneeSlide }

    [SerializeField] private Animator animator;
    [SerializeField] private float locomotionDamp = .12f;
    [SerializeField] private float actionCooldown = .12f;
    [SerializeField] private float injuryDuration = 2.5f;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Sprint = Animator.StringToHash("Sprint");
    private static readonly int KickRight = Animator.StringToHash("KickRight");
    private static readonly int KickLeft = Animator.StringToHash("KickLeft");
    private static readonly int PassRight = Animator.StringToHash("PassRight");
    private static readonly int PassLeft = Animator.StringToHash("PassLeft");
    private static readonly int Header = Animator.StringToHash("Header");
    private static readonly int Dribble = Animator.StringToHash("Dribble");
    private static readonly int Tackle = Animator.StringToHash("Tackle");
    private static readonly int Slide = Animator.StringToHash("Slide");
    private static readonly int Foul = Animator.StringToHash("Foul");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Injured = Animator.StringToHash("Injured");
    private static readonly int Celebrate = Animator.StringToHash("Celebrate");
    private static readonly int CelebrationStyle = Animator.StringToHash("CelebrationStyle");
    private static readonly int Dive = Animator.StringToHash("Dive");
    private static readonly int Save = Animator.StringToHash("Save");
    private static readonly int Complain = Animator.StringToHash("Complain");

    private float nextActionTime;
    private float injuryEndTime;

    private void Reset() => animator = GetComponent<Animator>();
    private void Awake() { if (animator == null) animator = GetComponent<Animator>(); }

    public bool IsInjured => Time.time < injuryEndTime;

    public void SetLocomotion(float speed01, bool sprinting)
    {
        if (animator == null || IsInjured) return;
        animator.SetFloat(Speed, Mathf.Clamp01(speed01), locomotionDamp, Time.deltaTime);
        animator.SetBool(Sprint, sprinting);
    }

    public void PlayKick(Foot foot = Foot.Right) => Trigger(foot == Foot.Left ? KickLeft : KickRight);
    public void PlayPass(Foot foot = Foot.Right) => Trigger(foot == Foot.Left ? PassLeft : PassRight);
    public void PlayHeader() => Trigger(Header);
    public void PlayDribble() => Trigger(Dribble);
    public void PlayTackle() => Trigger(Tackle);
    public void PlaySlide() => Trigger(Slide);
    public void PlayFoul() => Trigger(Foul);
    public void PlayFall() => Trigger(Fall);
    public void PlayDive() => Trigger(Dive);
    public void PlaySave() => Trigger(Save);
    public void PlayComplain() => Trigger(Complain);

    public void PlayInjury()
    {
        injuryEndTime = Time.time + injuryDuration;
        Trigger(Injured, true);
    }

    public void PlayCelebrate(CelebrationStyle style = CelebrationStyle.ArmsUp)
    {
        if (animator == null || Time.time < nextActionTime) return;
        animator.SetInteger(CelebrationStyle, (int)style);
        animator.SetTrigger(Celebrate);
        nextActionTime = Time.time + actionCooldown;
    }

    // Compatibilidade com scripts antigos.
    public void PlayDefend() => PlayTackle();

    private void Trigger(int parameter, bool force = false)
    {
        if (animator == null || (!force && Time.time < nextActionTime)) return;
        animator.SetTrigger(parameter);
        nextActionTime = Time.time + actionCooldown;
    }
}
