using UnityEngine;

public class RefereeAI : MonoBehaviour
{
    [SerializeField] private Transform referee;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform[] players;
    [SerializeField] private FoulSystem foulSystem;
    [SerializeField] private float followDistance = 9f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float visionRange = 20f;
    private float nextCheck;

    private void Update()
    {
        if (referee == null || ball == null) return;
        Vector3 target = ball.position - ball.forward * followDistance;
        target.y = referee.position.y;
        referee.position = Vector3.MoveTowards(referee.position, target, speed * Time.deltaTime);
        if (Time.time < nextCheck) return;
        nextCheck = Time.time + .25f;
        if (foulSystem == null || players == null) return;
        for (int i = 0; i < players.Length; i++)
        for (int j = i + 1; j < players.Length; j++)
        {
            if (players[i] == null || players[j] == null) continue;
            float distance = Vector3.Distance(players[i].position, players[j].position);
            if (distance < 1.1f && Vector3.Distance(referee.position, players[i].position) < visionRange)
                foulSystem.CheckContact(players[i].position, 4f, false);
        }
    }
}
