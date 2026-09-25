using UnityEngine;

public sealed class AIDecisionSystem
{
    private readonly TeamAIControllerStateDriven controller;

    public AIDecisionSystem(TeamAIControllerStateDriven controller)
    {
        this.controller = controller;
    }

    public AIAction Decide()
    {
        AIPlayerContext context = controller.Context;

        if (context.HasBall)
            return DecideWithBall(context);

        return DecideWithoutBall(context);
    }

    public Transform FindBestPassTarget()
    {
        AIPlayerContext context = controller.Context;
        Transform best = null;
        float bestScore = float.MinValue;

        foreach (Transform target in controller.PassTargets)
        {
            if (target == null || target == context.Self)
                continue;

            Vector3 toTarget = target.position - context.Self.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance < controller.MinPassDistance || distance > controller.MaxPassDistance)
                continue;

            Vector3 direction = toTarget.normalized;
            float forwardScore = Vector3.Dot(context.Self.forward, direction);
            float spaceScore = Mathf.Clamp01(distance / controller.MaxPassDistance);
            float safetyScore = HasClearPass(context.Self.position, target.position) ? 1f : -3f;
            float score = forwardScore * 2f + spaceScore + safetyScore * 4f;

            if (score > bestScore)
            {
                bestScore = score;
                best = target;
            }
        }

        return best;
    }

    public Vector3 GetPassDirection(Transform target)
    {
        if (target == null)
            return Vector3.zero;

        Vector3 direction = target.position - controller.transform.position;
        direction.y = 0f;
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.zero;
    }

    private AIAction DecideWithBall(AIPlayerContext context)
    {
        Transform passTarget = FindBestPassTarget();
        float shootScore = ScoreShoot(context);
        float passScore = ScorePass(context, passTarget);
        float dribbleScore = ScoreDribble(context);

        if (shootScore >= controller.MinShootScore && shootScore >= passScore && shootScore >= dribbleScore)
            return AIAction.Shoot;

        if (passTarget != null && passScore >= dribbleScore)
            return AIAction.Pass;

        return AIAction.Dribble;
    }

    private AIAction DecideWithoutBall(AIPlayerContext context)
    {
        float chaseScore = ScoreChase(context);
        float supportScore = ScoreSupport(context);
        float defendScore = ScoreDefend(context);
        float returnScore = ScoreReturn(context);

        float highest = chaseScore;
        AIAction best = AIAction.Chase;

        if (supportScore > highest) { highest = supportScore; best = AIAction.Support; }
        if (defendScore > highest) { highest = defendScore; best = AIAction.Defend; }
        if (returnScore > highest) best = AIAction.Return;

        return best;
    }

    private float ScoreShoot(AIPlayerContext context)
    {
        if (context.TargetGoal == null)
            return 0f;

        Vector3 toGoal = context.TargetGoal.position - context.Self.position;
        toGoal.y = 0f;
        float distance = toGoal.magnitude;
        float distanceScore = Mathf.InverseLerp(controller.MaxShootDistance, controller.MinShootDistance, distance);
        float angleScore = Mathf.Clamp01(Vector3.Dot(context.Self.forward, toGoal.normalized));
        float clearScore = HasClearPass(context.Self.position, context.TargetGoal.position) ? 1f : 0f;

        return distanceScore * 7f + angleScore * 2f + clearScore * 3f;
    }

    private float ScorePass(AIPlayerContext context, Transform target)
    {
        if (target == null || !HasClearPass(context.Self.position, target.position))
            return 0f;

        float distance = Vector3.Distance(context.Self.position, target.position);
        return 5f + Mathf.Clamp01(distance / controller.MaxPassDistance) * 2f;
    }

    private float ScoreDribble(AIPlayerContext context)
    {
        return controller.Role == TeamAIControllerStateDriven.Role.Attacker ? 5f : 3.5f;
    }

    private float ScoreChase(AIPlayerContext context)
    {
        if (context.DistanceToBall > controller.ChaseDistance) return 0f;
        return Mathf.InverseLerp(controller.ChaseDistance, 0f, context.DistanceToBall) * 10f;
    }

    private float ScoreSupport(AIPlayerContext context)
    {
        if (context.Ball == null || context.HomePosition == null) return 0f;
        float distance = Vector3.Distance(context.Self.position, context.HomePosition.position);
        return Mathf.InverseLerp(20f, 0f, distance) * 5f;
    }

    private float ScoreDefend(AIPlayerContext context)
    {
        return controller.Role == TeamAIControllerStateDriven.Role.Defender ? 7f : 2f;
    }

    private float ScoreReturn(AIPlayerContext context)
    {
        if (context.HomePosition == null) return 0f;
        float distance = Vector3.Distance(context.Self.position, context.HomePosition.position);
        return Mathf.InverseLerp(2f, 15f, distance) * 6f;
    }

    private bool HasClearPass(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;
        if (distance <= 0.01f) return false;

        return !Physics.Raycast(origin + Vector3.up * 0.35f, direction.normalized, distance, controller.ObstacleMask, QueryTriggerInteraction.Ignore);
    }
}
