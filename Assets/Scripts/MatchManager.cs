using UnityEngine;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private BallController ball; [SerializeField] private Transform ballKickoff; [SerializeField] private TextMeshProUGUI scoreText; [SerializeField] private TextMeshProUGUI timerText; [SerializeField] private GameObject endPanel; [SerializeField] private TextMeshProUGUI resultText; [SerializeField] private FootballTeamManager playerTeam; [SerializeField] private FootballTeamManager opponentTeam; [SerializeField] private Transform enemyGoal; [SerializeField] private float matchDuration = 120f; [SerializeField] private AudioManager audioManager;
    private int playerScore, enemyScore; private float timeRemaining; private bool running;
    private void Start() { audioManager = audioManager != null ? audioManager : AudioManager.Instance; audioManager?.PlayMatchMusic(); audioManager?.StartCrowd(); if (CameraDirector.Instance != null) CameraDirector.Instance.SetTargets(playerTeam != null && playerTeam.ActivePlayer != null ? playerTeam.ActivePlayer.transform : null, ball != null ? ball.transform : null, enemyGoal); RestartMatch(); }
    private void Update() { if (!running) return; timeRemaining -= Time.deltaTime; if (timeRemaining <= 0f) { timeRemaining = 0f; running = false; audioManager?.PlayWhistle(); ShowResult(); } RefreshUI(); }
    public void RegisterGoal(int scoringTeam) { if (!running) return; if (scoringTeam == 0) playerScore++; else enemyScore++; audioManager?.PlayGoal(); audioManager?.PlayCelebration(); audioManager?.PlayCommentary(); CameraDirector.Instance?.FocusOnGoal(); CameraDirector.Instance?.PlayGoalReplay(); ResetTeams(); if (ball != null) ball.ResetBall(ballKickoff != null ? ballKickoff.position : (Vector3?)null); RefreshUI(); }
    public void RestartMatch() { playerScore = enemyScore = 0; timeRemaining = matchDuration; running = true; if (endPanel != null) endPanel.SetActive(false); audioManager?.PlayWhistle(); ResetTeams(); if (ball != null) ball.ResetBall(ballKickoff != null ? ballKickoff.position : (Vector3?)null); RefreshUI(); }
    private void ResetTeams() { if (playerTeam != null) playerTeam.ResetTeam(); if (opponentTeam != null) opponentTeam.ResetTeam(); if (CameraDirector.Instance != null && playerTeam != null && playerTeam.ActivePlayer != null) CameraDirector.Instance.SetTargets(playerTeam.ActivePlayer.transform, ball != null ? ball.transform : null, enemyGoal); }
    private void RefreshUI() { if (scoreText != null) scoreText.text = $"{playerScore} - {enemyScore}"; if (timerText != null) timerText.text = $"{Mathf.CeilToInt(timeRemaining / 60f):00}:{Mathf.CeilToInt(timeRemaining % 60f):00}"; }
    private void ShowResult() { if (endPanel != null) endPanel.SetActive(true); if (resultText != null) resultText.text = playerScore == enemyScore ? "EMPATE" : (playerScore > enemyScore ? "VOCÊ VENCEU!" : "VOCÊ PERDEU!"); }
    public void StartPenaltyCamera() => CameraDirector.Instance?.SetPenaltyCamera();
    public void StopSpecialCamera() => CameraDirector.Instance?.SetFollowPlayer();
}
