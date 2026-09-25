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
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform[] enemySpawns;
    [SerializeField] private Transform goalkeeperSpawn;
    [SerializeField] private float matchDuration = 120f;

    private int playerScore, enemyScore;
    private float timeRemaining;
    private bool running;

    private void Start() { RestartMatch(); }
    private void Update()
    {
        if (!running) return;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f) { timeRemaining = 0f; running = false; ShowResult(); }
        RefreshUI();
    }

    public void RegisterGoal(int scoringTeam)
    {
        if (!running) return;
        if (scoringTeam == 0) playerScore++; else enemyScore++;
        ResetPositions();
        ball.ResetBall(ballKickoff != null ? ballKickoff.position : (Vector3?)null);
        RefreshUI();
    }

    public void RestartMatch()
    {
        playerScore = enemyScore = 0; timeRemaining = matchDuration; running = true;
        if (endPanel != null) endPanel.SetActive(false);
        ResetPositions();
        if (ball != null) ball.ResetBall(ballKickoff != null ? ballKickoff.position : (Vector3?)null);
        RefreshUI();
    }

    private void ResetPositions()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && playerSpawn != null) { CharacterController c = player.GetComponent<CharacterController>(); if (c != null) c.enabled = false; player.transform.position = playerSpawn.position; if (c != null) c.enabled = true; }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length && i < enemySpawns.Length; i++) enemies[i].transform.position = enemySpawns[i].position;
        GameObject keeper = GameObject.FindGameObjectWithTag("Goalkeeper");
        if (keeper != null && goalkeeperSpawn != null) keeper.transform.position = goalkeeperSpawn.position;
    }

    private void RefreshUI()
    {
        if (scoreText != null) scoreText.text = $"{playerScore} - {enemyScore}";
        if (timerText != null) timerText.text = $"{Mathf.CeilToInt(timeRemaining / 60f):00}:{Mathf.CeilToInt(timeRemaining % 60f):00}";
    }

    private void ShowResult()
    {
        if (endPanel != null) endPanel.SetActive(true);
        if (resultText != null) resultText.text = playerScore == enemyScore ? "EMPATE" : (playerScore > enemyScore ? "VOCÊ VENCEU!" : "VOCÊ PERDEU!");
    }
}
