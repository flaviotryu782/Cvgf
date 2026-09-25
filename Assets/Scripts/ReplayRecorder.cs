using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    [System.Serializable]
    private struct Frame
    {
        public float time;
        public Vector3[] positions;
        public Quaternion[] rotations;
        public Vector3 ballVelocity;
    }

    [Header("Recording")]
    [SerializeField] private BallController ball;
    [SerializeField] private Transform[] trackedPlayers;
    [SerializeField] private float secondsToKeep = 10f;
    [SerializeField] private float sampleRate = 20f;
    [Header("Replay camera")]
    [SerializeField] private Camera replayCamera;
    [SerializeField] private Transform cameraFocus;
    [SerializeField] private Vector3[] cameraOffsets = {
        new Vector3(0f, 6f, -10f),
        new Vector3(9f, 4f, -6f),
        new Vector3(-9f, 4f, -6f)
    };
    [SerializeField] private float replaySpeed = .45f;

    private readonly List<Frame> frames = new List<Frame>();
    private readonly List<Behaviour> disabledBehaviours = new List<Behaviour>();
    private float nextSample;
    private bool playing;
    private int cameraIndex;
    private Vector3 savedBallPosition;
    private Quaternion savedBallRotation;
    private Vector3 savedCameraPosition;
    private Quaternion savedCameraRotation;
    private bool savedCamera;

    public bool IsPlaying => playing;
    public bool HasReplay => frames.Count > 1;

    private void Update()
    {
        if (!playing && Time.unscaledTime >= nextSample) Capture();
    }

    private void Capture()
    {
        nextSample = Time.unscaledTime + 1f / Mathf.Max(1f, sampleRate);
        Frame frame = new Frame
        {
            time = Time.unscaledTime,
            positions = new Vector3[trackedPlayers == null ? 0 : trackedPlayers.Length],
            rotations = new Quaternion[trackedPlayers == null ? 0 : trackedPlayers.Length],
            ballVelocity = ball != null && ball.Body != null ? ball.Body.velocity : Vector3.zero
        };
        for (int i = 0; i < frame.positions.Length; i++)
        {
            if (trackedPlayers[i] == null) continue;
            frame.positions[i] = trackedPlayers[i].position;
            frame.rotations[i] = trackedPlayers[i].rotation;
        }
        frames.Add(frame);
        float cutoff = Time.unscaledTime - secondsToKeep;
        while (frames.Count > 2 && frames[0].time < cutoff) frames.RemoveAt(0);
    }

    public void PlayLatestReplay() { if (HasReplay) StartCoroutine(PlayReplay(false)); }
    public void SaveBestReplay()
    {
        if (!HasReplay) return;
        PlayerPrefs.SetInt("football_best_replay_frames", frames.Count);
        PlayerPrefs.SetFloat("football_best_replay_duration", frames[frames.Count - 1].time - frames[0].time);
        PlayerPrefs.Save();
    }

    public void SkipReplay()
    {
        if (playing) StopReplay();
    }

    public void NextCameraAngle()
    {
        cameraIndex = cameraOffsets == null || cameraOffsets.Length == 0 ? 0 : (cameraIndex + 1) % cameraOffsets.Length;
    }

    private IEnumerator PlayReplay(bool saved)
    {
        playing = true;
        disabledBehaviours.Clear();
        MonoBehaviour[] all = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in all)
        {
            if (behaviour == this || behaviour == replayCamera?.GetComponent<Camera>()) continue;
            if (behaviour is ReplayControlButton) continue;
            if (behaviour.enabled && (behaviour is PlayerController || behaviour is TeamAIController || behaviour is GoalkeeperController || behaviour is DefensiveActionController || behaviour is MatchManager))
            {
                behaviour.enabled = false;
                disabledBehaviours.Add(behaviour);
            }
        }

        if (ball != null)
        {
            savedBallPosition = ball.transform.position;
            savedBallRotation = ball.transform.rotation;
            ball.ReleaseControl();
            ball.Body.isKinematic = true;
        }
        if (replayCamera != null)
        {
            savedCameraPosition = replayCamera.transform.position;
            savedCameraRotation = replayCamera.transform.rotation;
            savedCamera = true;
            replayCamera.enabled = true;
        }

        float start = frames[0].time;
        float end = frames[frames.Count - 1].time;
        float cursor = start;
        cameraIndex = 0;
        while (playing && cursor <= end)
        {
            ApplyFrame(cursor);
            cursor += Time.unscaledDeltaTime * replaySpeed;
            yield return null;
        }
        StopReplay();
    }

    private void ApplyFrame(float time)
    {
        if (frames.Count == 0) return;
        Frame a = frames[0]; Frame b = frames[frames.Count - 1];
        for (int i = 1; i < frames.Count; i++)
        {
            if (frames[i].time >= time) { a = frames[i - 1]; b = frames[i]; break; }
        }
        float t = Mathf.InverseLerp(a.time, b.time, time);
        for (int i = 0; i < a.positions.Length; i++)
        {
            if (trackedPlayers == null || i >= trackedPlayers.Length || trackedPlayers[i] == null) continue;
            trackedPlayers[i].SetPositionAndRotation(Vector3.Lerp(a.positions[i], b.positions[i], t), Quaternion.Slerp(a.rotations[i], b.rotations[i], t));
        }
        if (ball != null)
        {
            Vector3 p = Vector3.Lerp(GetBallPosition(a), GetBallPosition(b), t);
            ball.transform.position = p;
            ball.Body.velocity = Vector3.Lerp(a.ballVelocity, b.ballVelocity, t);
        }
        if (replayCamera != null)
        {
            Vector3 focus = cameraFocus != null ? cameraFocus.position : ball != null ? ball.transform.position : transform.position;
            Vector3 offset = cameraOffsets != null && cameraOffsets.Length > 0 ? cameraOffsets[cameraIndex] : new Vector3(0f, 6f, -10f);
            replayCamera.transform.position = focus + offset;
            replayCamera.transform.LookAt(focus + Vector3.up);
        }
    }

    private Vector3 GetBallPosition(Frame frame)
    {
        return ball == null ? Vector3.zero : ball.transform.position + frame.ballVelocity * .01f;
    }

    private void StopReplay()
    {
        playing = false;
        foreach (Behaviour behaviour in disabledBehaviours) if (behaviour != null) behaviour.enabled = true;
        disabledBehaviours.Clear();
        if (ball != null)
        {
            ball.Body.isKinematic = false;
            ball.transform.position = savedBallPosition;
            ball.transform.rotation = savedBallRotation;
        }
        if (replayCamera != null && savedCamera)
        {
            replayCamera.transform.position = savedCameraPosition;
            replayCamera.transform.rotation = savedCameraRotation;
        }
    }
}
