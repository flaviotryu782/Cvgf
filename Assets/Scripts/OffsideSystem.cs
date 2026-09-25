using UnityEngine;

public class OffsideSystem : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Transform attackingGoal;
    [SerializeField] private Transform[] attackers;
    [SerializeField] private Transform[] defenders;
    [SerializeField] private LineRenderer offsideLine;
    [SerializeField] private float lineWidth = .05f;
    [SerializeField] private float fieldHalfWidth = 20f;
    [SerializeField] private float tolerance = .15f;

    public bool IsOffside { get; private set; }
    public Transform OffsidePlayer { get; private set; }

    private void Update()
    {
        if (ball == null || attackingGoal == null || defenders == null || defenders.Length == 0) return;
        float goalDirection = Mathf.Sign(attackingGoal.position.z - ball.position.z);
        float secondLastDefender = GetSecondLastDefender(goalDirection);
        float lineZ = Mathf.Abs(goalDirection) < .01f ? ball.position.z : secondLastDefender;
        DrawLine(lineZ);
        IsOffside = false;
        OffsidePlayer = null;
        if (attackers == null) return;
        foreach (Transform attacker in attackers)
        {
            if (attacker == null) continue;
            float attackerZ = (attacker.position.z - lineZ) * goalDirection;
            float ballZ = (ball.position.z - lineZ) * goalDirection;
            if (attackerZ > tolerance && attackerZ > ballZ + tolerance)
            {
                IsOffside = true;
                OffsidePlayer = attacker;
                break;
            }
        }
    }

    public bool CheckAtPass(Transform receiver)
    {
        if (receiver == null || ball == null || attackingGoal == null) return false;
        float direction = Mathf.Sign(attackingGoal.position.z - ball.position.z);
        float line = GetSecondLastDefender(direction);
        bool beyondDefender = (receiver.position.z - line) * direction > tolerance;
        bool beyondBall = (receiver.position.z - ball.position.z) * direction > tolerance;
        return beyondDefender && beyondBall;
    }

    private float GetSecondLastDefender(float direction)
    {
        float first = direction > 0f ? float.PositiveInfinity : float.NegativeInfinity;
        float second = first;
        foreach (Transform defender in defenders)
        {
            if (defender == null) continue;
            float value = defender.position.z;
            if (direction > 0f)
            {
                if (value < first) { second = first; first = value; }
                else if (value < second) second = value;
            }
            else
            {
                if (value > first) { second = first; first = value; }
                else if (value > second) second = value;
            }
        }
        return defenders.Length == 1 ? first : second;
    }

    private void DrawLine(float z)
    {
        if (offsideLine == null) return;
        offsideLine.positionCount = 2;
        offsideLine.startWidth = lineWidth;
        offsideLine.endWidth = lineWidth;
        offsideLine.SetPosition(0, new Vector3(-fieldHalfWidth, .03f, z));
        offsideLine.SetPosition(1, new Vector3(fieldHalfWidth, .03f, z));
        offsideLine.startColor = IsOffside ? Color.red : Color.yellow;
        offsideLine.endColor = offsideLine.startColor;
    }
}
