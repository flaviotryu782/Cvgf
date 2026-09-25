using System.Collections;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    public enum Mode { FollowPlayer, FocusGoal, Replay, Celebration, Penalty }

    public static CameraDirector Instance { get; private set; }
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform ballTarget;
    [SerializeField] private Transform goalTarget;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 9f, -12f);
    [SerializeField] private Vector3 penaltyOffset = new Vector3(0f, 5f, -9f);
    [SerializeField] private float followSmooth = 7f;
    [SerializeField] private float normalFov = 55f;
    [SerializeField] private float shotFov = 44f;
    [SerializeField] private float penaltyFov = 38f;
    [SerializeField] private float modeDuration = 3f;

    private Mode mode = Mode.FollowPlayer;
    private float shakeTime;
    private float shakeAmount;
    private float fovTarget;
    private Coroutine modeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (targetCamera == null) targetCamera = Camera.main;
        fovTarget = normalFov;
        if (targetCamera != null) targetCamera.fieldOfView = normalFov;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;
        Vector3 targetPosition;
        Transform lookTarget;
        switch (mode)
        {
            case Mode.FocusGoal:
                targetPosition = (ballTarget != null ? ballTarget.position : transform.position) + Vector3.up * 5f - Vector3.forward * 8f;
                lookTarget = goalTarget != null ? goalTarget : ballTarget;
                break;
            case Mode.Replay:
                targetPosition = (ballTarget != null ? ballTarget.position : transform.position) + Vector3.up * 6f - Vector3.forward * 10f;
                lookTarget = ballTarget;
                break;
            case Mode.Celebration:
                targetPosition = (playerTarget != null ? playerTarget.position : transform.position) + Vector3.up * 4f - Vector3.forward * 7f;
                lookTarget = playerTarget;
                break;
            case Mode.Penalty:
                targetPosition = (ballTarget != null ? ballTarget.position : transform.position) + penaltyOffset;
                lookTarget = goalTarget;
                break;
            default:
                targetPosition = (playerTarget != null ? playerTarget.position : transform.position) + followOffset;
                lookTarget = playerTarget != null ? playerTarget : ballTarget;
                break;
        }

        targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetPosition, followSmooth * Time.deltaTime);
        if (lookTarget != null)
        {
            Vector3 look = lookTarget.position + Vector3.up;
            targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, Quaternion.LookRotation(look - targetCamera.transform.position), followSmooth * Time.deltaTime);
        }
        targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, fovTarget, 8f * Time.deltaTime);
        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            targetCamera.transform.position += Random.insideUnitSphere * shakeAmount * Mathf.Clamp01(shakeTime * 10f);
        }
    }

    public void SetTargets(Transform player, Transform ball, Transform goal)
    {
        playerTarget = player; ballTarget = ball; goalTarget = goal;
    }

    public void SetFollowPlayer() { StopModeRoutine(); mode = Mode.FollowPlayer; fovTarget = normalFov; }
    public void ZoomForShot(float duration = .65f) { fovTarget = shotFov; Shake(.06f, .12f); StartTemporaryMode(Mode.FollowPlayer, duration); }
    public void FocusOnGoal(float duration = 2.2f) { StartTemporaryMode(Mode.FocusGoal, duration); }
    public void PlayGoalReplay(float duration = 3f) { StartTemporaryMode(Mode.Replay, duration); }
    public void PlayCelebration(float duration = 2.5f) { StartTemporaryMode(Mode.Celebration, duration); }
    public void SetPenaltyCamera() { StopModeRoutine(); mode = Mode.Penalty; fovTarget = penaltyFov; }
    public void Shake(float duration, float amount) { shakeTime = Mathf.Max(shakeTime, duration); shakeAmount = Mathf.Max(shakeAmount, amount); }

    private void StartTemporaryMode(Mode temporaryMode, float duration)
    {
        StopModeRoutine();
        modeRoutine = StartCoroutine(TemporaryMode(temporaryMode, duration));
    }

    private IEnumerator TemporaryMode(Mode temporaryMode, float duration)
    {
        mode = temporaryMode;
        if (temporaryMode != Mode.FollowPlayer) fovTarget = temporaryMode == Mode.Penalty ? penaltyFov : shotFov;
        yield return new WaitForSeconds(duration);
        SetFollowPlayer();
    }

    private void StopModeRoutine()
    {
        if (modeRoutine != null) StopCoroutine(modeRoutine);
        modeRoutine = null;
    }
}
