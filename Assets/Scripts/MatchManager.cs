using System.Collections;
using UnityEngine;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private Transform ballKickoff;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private FootballTeamManager playerTeam;
    [SerializeField] private FootballTeamManager opponentTeam;
    [SerializeField] private ReplayRecorder replayRecorder;
    [SerializeField] private GameObject replayPanel;
    [SerializeField] private float matchDuration = 120f;
    [SerializeField] private AudioManager audioManager;
    private int playerScore, enemyScore; private float timeRemaining; private bool running; private bool resetting;

    private void Start() { audioManager = audioManager != null ? audioManager : AudioManager.Instance; replayRecorder = replayRecorder != null ? replayRecorder : FindObjectOfType<ReplayRecorder>(); audioManager?.PlayMatchMusic(); audioManager?.StartCrowd(); RestartMatch(); }
    private void Update() { if (!running || resetting) return; timeRemaining -= Time.deltaTime; if (timeRemaining <= 0f) { timeRemaining = 0f; running = false; ShowResult(); } RefreshUI(); }

    public void RegisterGoal(int scoringTeam)
    {
        if (!running || resetting) return;
        if (scoringTeam == 0) playerScore++; else enemyScore++;
        running = false;
        audioManager?.PlayGoal(); audioManager?.PlayCelebration();
        if (replayRecorder != null && replayRecorder.HasReplay) StartCoroutine(GoalReplayThenReset());
        else ResetAfterGoal();
        RefreshUI();
    }

    private IEnumerator GoalReplayThenReset()
    {
        resetting = true;
        if (replayPanel != null) replayPanel.SetActive(true);
        replayRecorder.PlayLatestReplay();
        float timeout = 7f;
        while (replayRecorder.IsPlaying && timeout > 0f) { timeout -= Time.unscaledDeltaTime; yield return null; }
        if (replayPanel != null) replayPanel.SetActive(false);
        ResetAfterGoal();
        resetting = false;
        running = true;
    }

    private void ResetAfterGoal() { ResetTeams(); if (ball != null) ball.ResetBall(ballKickoff != null ? ballKickoff.position : (Vector3?)null); }
    public void RestartMatch() { playerScore = enemyScore = 0; timeRemaining = matchDuration; running = true; resetting = false; if (endPanel != null) endPanel.SetActive(false); if (replayPanel != null) replayPanel.SetActive(false); ResetAfterGoal(); RefreshUI(); }
    private void ResetTeams() { if (playerTeam != null) playerTeam.ResetTeam(); if (opponentTeam != null) opponentTeam.ResetTeam(); }
    private void RefreshUI() { if (scoreText != null) scoreText.text = $"{playerScore} - {enemyScore}"; if (timerText != null) timerText.text = $"{Mathf.CeilToInt(timeRemaining / 60f):00}:{Mathf.CeilToInt(timeRemaining % 60f):00}"; }
    private void ShowResult() { if (endPanel != null) endPanel.SetActive(true); if (resultText != null) resultText.text = playerScore == enemyScore ? "EMPATE" : (playerScore > enemyScore ? "VOCÊ VENCEU!" : "VOCÊ PERDEU!"); }
}
