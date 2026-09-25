using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PenaltyShootoutManager : MonoBehaviour
{
    public enum Phase { Inactive, Aim, Shooting, Result, Finished }

    [Header("References")]
    [SerializeField] private BallController ball;
    [SerializeField] private Transform penaltyBallSpot;
    [SerializeField] private Transform targetGoal;
    [SerializeField] private GoalkeeperController goalkeeper;
    [SerializeField] private CameraDirector cameraDirector;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private GameObject penaltyPanel;
    [SerializeField] private Text resultText;
    [Header("Rules")]
    [SerializeField] private int kicksPerTeam = 5;
    [SerializeField] private float aimSensitivity = 1.4f;
    [SerializeField] private float shotPower = 15f;
    [SerializeField] private float minPower = .35f;
    [SerializeField] private float maxPower = 1f;
    [SerializeField] private float resultDelay = 1.8f;

    private Phase phase = Phase.Inactive;
    private Vector2 aim;
    private bool playerTurn;
    private bool kickHeld;
    private float kickStarted;
    private int playerGoals;
    private int opponentGoals;
    private int playerKicks;
    private int opponentKicks;

    public Phase CurrentPhase => phase;
    public int PlayerGoals => playerGoals;
    public int OpponentGoals => opponentGoals;

    private void Update()
    {
        if (phase != Phase.Aim || !playerTurn) return;
        aim.x += Input.GetAxisRaw("Horizontal") * aimSensitivity * Time.deltaTime;
        aim.y += Input.GetAxisRaw("Vertical") * aimSensitivity * Time.deltaTime;
        aim = Vector2.ClampMagnitude(aim, 1f);
        if (powerSlider != null) powerSlider.value = CurrentPower();
        if (!Application.isMobilePlatform && Input.GetKeyDown(KeyCode.Space)) BeginKick();
        if (!Application.isMobilePlatform && Input.GetKeyUp(KeyCode.Space)) ReleaseKick();
    }

    public void StartShootout()
    {
        playerGoals = opponentGoals = playerKicks = opponentKicks = 0;
        playerTurn = true;
        phase = Phase.Aim;
        if (penaltyPanel != null) penaltyPanel.SetActive(true);
        if (cameraDirector != null) cameraDirector.SetPenaltyTargets(ball != null ? ball.transform : null, targetGoal);
        cameraDirector?.SetPenaltyCamera();
        PositionBall();
    }

    public void SetAim(Vector2 value) => aim = Vector2.ClampMagnitude(value, 1f);
    public void SetAimHorizontal(float value) => aim.x = Mathf.Clamp(value, -1f, 1f);
    public void SetAimVertical(float value) => aim.y = Mathf.Clamp(value, -1f, 1f);
    public void BeginKick()
    {
        if (phase != Phase.Aim || !playerTurn) return;
        kickHeld = true;
        kickStarted = Time.time;
    }

    public void ReleaseKick()
    {
        if (phase != Phase.Aim || !playerTurn || !kickHeld) return;
        kickHeld = false;
        StartCoroutine(PlayerKick());
    }

    public void KickImmediately() { BeginKick(); ReleaseKick(); }

    private IEnumerator PlayerKick()
    {
        phase = Phase.Shooting;
        float power = Mathf.Clamp(minPower + (Time.time - kickStarted) * .65f, minPower, maxPower);
        Vector3 direction = AimDirection();
        ball?.Shoot(direction, shotPower, BallController.ShotType.Ground, power);
        AudioManager.Instance?.PlayKick();
        cameraDirector?.ZoomForShot(.7f);
        yield return new WaitForSeconds(resultDelay);
        bool scored = EvaluateShot(direction, power);
        if (scored) playerGoals++;
        playerKicks++;
        yield return ShowResult(scored, true);
        if (phase != Phase.Finished) StartOpponentKick();
    }

    private void StartOpponentKick()
    {
        playerTurn = false;
        phase = Phase.Shooting;
        PositionBall();
        StartCoroutine(OpponentKick());
    }

    private IEnumerator OpponentKick()
    {
        yield return new WaitForSeconds(.35f);
        Vector2 opponentAim = new Vector2(Random.Range(-1f, 1f), Random.Range(-.2f, .65f));
        Vector3 direction = DirectionFromAim(opponentAim);
        float power = Random.Range(.65f, 1f);
        ball?.Shoot(direction, shotPower, BallController.ShotType.Ground, power);
        AudioManager.Instance?.PlayKick();
        yield return new WaitForSeconds(resultDelay);
        bool scored = EvaluateShot(direction, power);
        if (scored) opponentGoals++;
        opponentKicks++;
        yield return ShowResult(scored, false);
        if (phase != Phase.Finished)
        {
            playerTurn = true;
            phase = Phase.Aim;
            PositionBall();
        }
    }

    private IEnumerator ShowResult(bool scored, bool player)
    {
        phase = Phase.Result;
        AudioManager.Instance?.PlayWhistle();
        if (resultText != null) resultText.text = scored ? (player ? "GOL!" : "GOL DO ADVERSÁRIO") : (player ? "DEFESA!" : "ERROU!");
        yield return new WaitForSeconds(.7f);
        if (IsFinished()) FinishShootout();
    }

    private void FinishShootout()
    {
        phase = Phase.Finished;
        if (resultText != null) resultText.text = playerGoals > opponentGoals ? "VOCÊ VENCEU NOS PÊNALTIS!" : "VOCÊ PERDEU NOS PÊNALTIS!";
        AudioManager.Instance?.PlayCelebration();
        cameraDirector?.SetFollowPlayer();
    }

    private bool IsFinished()
    {
        int remainingPlayer = kicksPerTeam - playerKicks;
        int remainingOpponent = kicksPerTeam - opponentKicks;
        if (playerKicks >= kicksPerTeam && opponentKicks >= kicksPerTeam) return playerGoals != opponentGoals;
        if (playerGoals > opponentGoals + remainingOpponent) return true;
        if (opponentGoals > playerGoals + remainingPlayer) return true;
        return false;
    }

    private void PositionBall()
    {
        if (ball == null) return;
        ball.ResetBall(penaltyBallSpot != null ? penaltyBallSpot.position : (Vector3?)null);
        aim = Vector2.zero;
    }

    private float CurrentPower() => Mathf.Clamp(minPower + (Time.time - kickStarted) * .65f, minPower, maxPower);
    private Vector3 AimDirection() => DirectionFromAim(aim);
    private Vector3 DirectionFromAim(Vector2 value)
    {
        Vector3 baseDirection = targetGoal != null ? targetGoal.forward : Vector3.forward;
        Vector3 right = targetGoal != null ? targetGoal.right : Vector3.right;
        Vector3 up = Vector3.up;
        return (baseDirection + right * value.x + up * value.y).normalized;
    }

    private bool EvaluateShot(Vector3 direction, float power)
    {
        float goalkeeperReach = goalkeeper == null ? .15f : .32f;
        float centrality = Mathf.Abs(Vector3.Dot(direction, targetGoal != null ? targetGoal.forward : Vector3.forward));
        bool keeperSave = Random.value < goalkeeperReach * (1f - Mathf.Clamp01(Mathf.Abs(aim.x) * .75f));
        return !keeperSave && power > .42f && centrality > .45f;
    }
}
