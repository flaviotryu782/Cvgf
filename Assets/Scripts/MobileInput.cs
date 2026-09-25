using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance { get; private set; }
    public Vector2 Move { get; private set; }
    public bool SprintHeld { get; private set; }
    private bool kickQueued, passQueued, tackleQueued, slideQueued;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    public void SetMove(Vector2 value) => Move = Vector2.ClampMagnitude(value, 1f);
    public void SetSprint(bool value) => SprintHeld = value;
    public void QueueKick() => kickQueued = true;
    public void QueuePass() => passQueued = true;
    public void QueueTackle() => tackleQueued = true;
    public void QueueSlide() => slideQueued = true;
    public bool ConsumeKick() { bool v = kickQueued; kickQueued = false; return v; }
    public bool ConsumePass() { bool v = passQueued; passQueued = false; return v; }
    public bool ConsumeTackle() { bool v = tackleQueued; tackleQueued = false; return v; }
    public bool ConsumeSlide() { bool v = slideQueued; slideQueued = false; return v; }
}
