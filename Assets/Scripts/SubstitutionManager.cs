using UnityEngine;

public class SubstitutionManager : MonoBehaviour
{
    [SerializeField] private FootballTeamManager team;
    [SerializeField] private PlayerController[] bench;
    [SerializeField] private float staminaThreshold = .25f;
    [SerializeField] private float checkInterval = 5f;
    private float nextCheck;

    private void Update()
    {
        if (Time.time < nextCheck || team == null || bench == null || bench.Length == 0) return;
        nextCheck = Time.time + checkInterval;
        PlayerController[] players = team.OutfieldPlayers;
        if (players == null) return;
        for (int i = 0; i < players.Length && i < bench.Length; i++)
        {
            if (players[i] == null || bench[i] == null) continue;
            PlayerAnimationController anim = players[i].GetComponentInChildren<PlayerAnimationController>();
            if (anim != null && anim.IsInjured) Substitute(i);
        }
    }

    public void Substitute(int index)
    {
        if (team == null || bench == null || index < 0 || index >= bench.Length || bench[index] == null) return;
        // FootballTeamManager exposes a safe replacement operation in newer scenes.
        team.ReplacePlayer(index, bench[index]);
        bench[index] = null;
    }
}
