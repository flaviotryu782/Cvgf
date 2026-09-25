using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance { get; private set; }
    public Vector2 Move { get; private set; }
    public bool SprintHeld { get; private set; }
    public event System.Action SwitchRequested;

    private bool passQueued, tackleQueued, slideQueued, kickReleased;
    private float kickStarted;
    private BallController.ShotType shotType = BallController.ShotType.Ground;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetMove(Vector2 value) => Move = Vector2.ClampMagnitude(value, 1f);
    public void SetSprint(bool value) => SprintHeld = value;
    public void QueuePass() => passQueued = true;
    public void QueueTackle() => tackleQueued = true;
    public void QueueSlide() => slideQueued = true;
    public void RequestSwitch() => SwitchRequested?.Invoke();

    public void BeginKick() { kickReleased = false; kickStarted = Time.time; }
    public void ReleaseKick(BallController.ShotType type = BallController.ShotType.Ground) { shotType = type; kickReleased = true; }
    public void QueueKick() { BeginKick(); ReleaseKick(); }

    public bool ConsumeKick()
    {
        bool value = kickReleased;
        kickReleased = false;
        return value;
    }

    public bool ConsumeKick(out float charge, out BallController.ShotType type)
    {
        if (!kickReleased) { charge = 0f; type = shotType; return false; }
        kickReleased = false;
        charge = Mathf.Clamp(.35f + (Time.time - kickStarted) * .65f, .35f, 1f);
        type = shotType;
        return true;
    }

    public bool ConsumePass() { bool value = passQueued; passQueued = false; return value; }
    public bool ConsumeTackle() { bool value = tackleQueued; tackleQueued = false; return value; }
    public bool ConsumeSlide() { bool value = slideQueued; slideQueued = false; return value; }
}
