using UnityEngine;

public class FoulSystem : MonoBehaviour
{
    [SerializeField] private PenaltyShootoutManager penaltyManager;
    [SerializeField] private Transform penaltyAreaCenter;
    [SerializeField] private float penaltyAreaRadius = 8f;
    [SerializeField] private float foulCooldown = 2f;
    [SerializeField, Range(0f, 1f)] private float slideFoulChance = .22f;
    [SerializeField, Range(0f, 1f)] private float hardContactFoulChance = .1f;
    [SerializeField] private bool cardsEnabled = true;
    private float nextFoulTime;
    private int yellowCards;

    public int YellowCards => yellowCards;
    public event System.Action<bool> FoulCalled;

    public void RegisterChallenge(DefensiveActionController defender, bool sliding)
    {
        if (defender == null || Time.time < nextFoulTime) return;
        float chance = sliding ? slideFoulChance : hardContactFoulChance;
        if (Random.value > chance) return;
        bool insidePenaltyArea = penaltyAreaCenter != null && Vector3.Distance(defender.transform.position, penaltyAreaCenter.position) <= penaltyAreaRadius;
        CallFoul(insidePenaltyArea, sliding);
    }

    public void CheckContact(Vector3 position, float relativeSpeed, bool sliding)
    {
        if (Time.time < nextFoulTime || relativeSpeed < 3.5f) return;
        bool inside = penaltyAreaCenter != null && Vector3.Distance(position, penaltyAreaCenter.position) <= penaltyAreaRadius;
        if (Random.value <= (sliding ? slideFoulChance : hardContactFoulChance)) CallFoul(inside, sliding);
    }

    private void CallFoul(bool penalty, bool severe)
    {
        nextFoulTime = Time.time + foulCooldown;
        if (cardsEnabled && severe) yellowCards++;
        FoulCalled?.Invoke(penalty);
        AudioManager.Instance?.PlayWhistle();
        if (penalty && penaltyManager != null) penaltyManager.StartShootout();
    }
}
