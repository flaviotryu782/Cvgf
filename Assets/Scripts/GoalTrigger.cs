using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private int scoringTeam;
    [SerializeField] private MatchManager matchManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && matchManager != null) matchManager.RegisterGoal(scoringTeam);
    }
}
