using System;
using UnityEngine;

public class AdaptiveAIBrain : MonoBehaviour
{
    public enum ActionType { Advance, Pass, Shoot, Return, Mark }
    [SerializeField] private string profileId = "opponent";
    [SerializeField] private TeamAIController.Role role = TeamAIController.Role.Midfielder;
    [SerializeField] private float learningRate = 0.12f;
    [SerializeField] private float discount = 0.85f;
    [SerializeField] private float exploration = 0.08f;
    [SerializeField] private float saveInterval = 10f;

    private readonly float[,] q = new float[3, 5];
    private int state;
    private ActionType lastAction;
    private float nextTick;
    private float pendingReward;
    private float nextSave;

    public float Exploration => exploration;

    private void Awake()
    {
        Load();
        nextTick = Time.time + 1f;
        nextSave = Time.time + saveInterval;
    }

    private void Update()
    {
        if (Time.time < nextTick) return;
        nextTick = Time.time + 1f;
        LearnEverySecond();
        if (Time.time >= nextSave) { Save(); nextSave = Time.time + saveInterval; }
    }

    public ActionType Choose(int newState)
    {
        state = Mathf.Clamp(newState, 0, 2);
        if (UnityEngine.Random.value < exploration) lastAction = (ActionType)UnityEngine.Random.Range(0, 5);
        else
        {
            int best = 0;
            for (int i = 1; i < 5; i++) if (q[state, i] > q[state, best]) best = i;
            lastAction = (ActionType)best;
        }
        return lastAction;
    }

    public void AddReward(float reward) => pendingReward += reward;

    public void OnGoalAgainst() => AddReward(-2f);
    public void OnGoalFor() => AddReward(3f);

    public void SetProfile(string teamId, TeamAIController.Role newRole)
    {
        profileId = string.IsNullOrEmpty(teamId) ? "opponent" : teamId;
        role = newRole;
        Load();
    }

    private void LearnEverySecond()
    {
        int nextState = state;
        float bestNext = q[nextState, 0];
        for (int i = 1; i < 5; i++) bestNext = Mathf.Max(bestNext, q[nextState, i]);
        float target = pendingReward + discount * bestNext;
        q[state, (int)lastAction] = Mathf.Lerp(q[state, (int)lastAction], target, learningRate);
        pendingReward = 0f;
        exploration = Mathf.Clamp(exploration * .9995f, .025f, .2f);
    }

    private string Key(string suffix) => "football_ai_" + profileId + "_" + role + "_" + suffix;

    private void Load()
    {
        for (int s = 0; s < 3; s++) for (int a = 0; a < 5; a++) q[s, a] = PlayerPrefs.GetFloat(Key(s + "_" + a), 0f);
    }

    public void Save()
    {
        for (int s = 0; s < 3; s++) for (int a = 0; a < 5; a++) PlayerPrefs.SetFloat(Key(s + "_" + a), q[s, a]);
        PlayerPrefs.Save();
    }
}
