using UnityEngine;

public class AdvancedTacticalManager : MonoBehaviour
{
    public enum MarkingMode { ManMarking, Zonal }
    public enum TacticalStyle { Balanced, HighPress, CounterAttack, Defensive }

    [SerializeField] private FootballTeamManager team;
    [SerializeField] private MarkingMode marking = MarkingMode.Zonal;
    [SerializeField] private TacticalStyle style = TacticalStyle.Balanced;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform ownGoal;
    [SerializeField] private Transform opponentGoal;
    [SerializeField] private Transform[] defensiveLine;
    [SerializeField] private Transform[] opponents;
    [SerializeField] private float defensiveLineDepth = 12f;
    [SerializeField] private float compactness = 8f;
    [SerializeField] private float highPressDistance = 16f;
    [SerializeField] private float transitionSpeed = 4f;
    [SerializeField] private int ownScore;
    [SerializeField] private int opponentScore;

    public MarkingMode Marking => marking;
    public TacticalStyle Style => style;
    public float Possession { get; private set; }

    private float possessionTimer;
    private float lastDecision;

    private void Update()
    {
        if (ball == null) return;
        UpdatePossession();
        if (Time.time >= lastDecision + .5f)
        {
            lastDecision = Time.time;
            SelectStyleFromScore();
            MoveDefensiveLine();
        }
    }

    private void UpdatePossession()
    {
        bool controlledByTeam = team != null && team.ActivePlayer != null && Vector3.Distance(team.ActivePlayer.transform.position, ball.position) < 3f;
        possessionTimer += (controlledByTeam ? 1f : -1f) * Time.deltaTime;
        Possession = Mathf.Clamp(Possession + (controlledByTeam ? 18f : -18f) * Time.deltaTime, 0f, 100f);
    }

    private void SelectStyleFromScore()
    {
        if (ownScore < opponentScore) style = TacticalStyle.HighPress;
        else if (ownScore > opponentScore) style = TacticalStyle.Defensive;
        else style = TacticalStyle.Balanced;
    }

    private void MoveDefensiveLine()
    {
        if (defensiveLine == null || defensiveLine.Length == 0 || ownGoal == null || ball == null) return;
        float direction = Mathf.Sign(opponentGoal != null ? opponentGoal.position.z - ownGoal.position.z : 1f);
        float targetZ = ownGoal.position.z + direction * defensiveLineDepth;
        if (style == TacticalStyle.HighPress) targetZ += direction * 5f;
        if (style == TacticalStyle.Defensive) targetZ -= direction * 4f;
        if (marking == MarkingMode.Zonal) targetZ = Mathf.Lerp(targetZ, ball.position.z - direction * 2f, .18f);
        foreach (Transform defender in defensiveLine)
        {
            if (defender == null) continue;
            Vector3 p = defender.position;
            p.z = Mathf.Lerp(p.z, targetZ, transitionSpeed * Time.deltaTime);
            defender.position = p;
        }
    }

    public void SetScore(int own, int opponent) { ownScore = own; opponentScore = opponent; }
    public void SetMarkingMode(MarkingMode value) => marking = value;
    public void SetStyle(TacticalStyle value) => style = value;
}
