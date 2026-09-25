using UnityEngine;

public sealed class AIDecisionSystem
{
    private readonly TeamAIControllerStateDriven controller;

    public AIDecisionSystem(TeamAIControllerStateDriven controller) { this.controller = controller; }

    public AIAction Decide()
    {
        AIPlayerContext context = controller.Context;
        return context.HasBall ? DecideWithBall(context) : DecideWithoutBall(context);
    }

    public Transform FindBestPassTarget()
    {
        AIPlayerContext context = controller.Context;
        Transform best = null;
        float bestScore = float.MinValue;
        foreach (Transform target in controller.PassTargets)
        {
            if (target == null || target == context.Self) continue;
            PassEvaluation evaluation = EvaluateTarget(target);
            if (!evaluation.valid || evaluation.score <= bestScore) continue;
            bestScore = evaluation.score;
            best = target;
        }
        return best;
    }

    public PassProfile BuildPassProfile(Transform target)
    {
        PassEvaluation evaluation = EvaluateTarget(target);
        float distance = Vector3.Distance(controller.transform.position, target.position);
        PassTechnique technique;

        if (evaluation.firstTime)
            technique = PassTechnique.FirstTime;
        else if (evaluation.forwardSpace > 0.55f && distance > controller.ThroughBallDistance)
            technique = PassTechnique.ThroughBall;
        else if (distance > controller.LongPassDistance)
            technique = PassTechnique.Lofted;
        else if (distance < controller.ShortPassDistance)
            technique = PassTechnique.ShortGround;
        else
            technique = PassTechnique.LongGround;

        return new PassProfile
        {
            power = Mathf.Clamp(distance * controller.PassPowerPerMeter, controller.MinPassPower, controller.MaxPassPower),
            accuracy = Mathf.Clamp01(evaluation.accuracy),
            pressure = evaluation.pressure,
            technique = technique,
            toSpace = evaluation.forwardSpace > 0.45f
        };
    }

    public Vector3 GetPassTargetPosition(Transform target, PassProfile profile)
    {
        if (target == null) return Vector3.zero;
        Vector3 position = target.position;
        if (profile.toSpace)
            position += target.forward * controller.SpaceLeadDistance;
        position.y = controller.BallPassHeight;
        return position;
    }

    public Vector3 GetPassDirection(Transform target) => GetPassTargetPosition(target, BuildPassProfile(target)) - controller.transform.position;

    private PassEvaluation EvaluateTarget(Transform target)
    {
        AIPlayerContext context = controller.Context;
        Vector3 toTarget = target.position - context.Self.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;
        if (distance < controller.MinPassDistance || distance > controller.MaxPassDistance) return PassEvaluation.Invalid;

        Vector3 direction = toTarget.normalized;
        float forwardSpace = Mathf.Clamp01(Vector3.Dot(context.Self.forward, direction) * 0.5f + 0.5f);
        bool clear = HasClearPass(context.Self.position, target.position);
        float pressure = EstimatePressure(target);
        float markedPenalty = pressure * 4f;
        float facingScore = Mathf.Clamp01(Vector3.Dot(target.forward, direction) * 0.5f + 0.5f);
        float accuracy = Mathf.Clamp01((clear ? 0.9f : 0.25f) + facingScore * 0.15f - pressure * 0.35f);
        float score = (clear ? 5f : -5f) + forwardSpace * 3f + facingScore * 2f - markedPenalty;

        if (controller.OffsideSystem != null && controller.OffsideSystem.CheckAtPass(target))
            return PassEvaluation.Invalid;

        return new PassEvaluation(true, score, accuracy, pressure, forwardSpace, false);
    }

    private float EstimatePressure(Transform target)
    {
        Collider[] nearby = Physics.OverlapSphere(target.position, controller.MarkingRadius, controller.ObstacleMask, QueryTriggerInteraction.Ignore);
        int opponents = 0;
        foreach (Collider item in nearby)
        {
            if (item != null && item.transform != target && !item.CompareTag("Teammate")) opponents++;
        }
        return Mathf.Clamp01(opponents / 2f);
    }

    private AIAction DecideWithBall(AIPlayerContext context)
    {
        Transform target = FindBestPassTarget();
        float shoot = ScoreShoot(context);
        float pass = ScorePass(context, target);
        float dribble = ScoreDribble(context);
        if (shoot >= controller.MinShootScore && shoot >= pass && shoot >= dribble) return AIAction.Shoot;
        if (target != null && pass >= dribble) return AIAction.Pass;
        return AIAction.Dribble;
    }

    private AIAction DecideWithoutBall(AIPlayerContext context)
    {
        float chase = ScoreChase(context), support = ScoreSupport(context), defend = ScoreDefend(context), ret = ScoreReturn(context);
        float highest = chase; AIAction best = AIAction.Chase;
        if (support > highest) { highest = support; best = AIAction.Support; }
        if (defend > highest) { highest = defend; best = AIAction.Defend; }
        if (ret > highest) best = AIAction.Return;
        return best;
    }

    private float ScoreShoot(AIPlayerContext c) { if (c.TargetGoal == null) return 0f; Vector3 d = c.TargetGoal.position - c.Self.position; d.y = 0f; float distance = d.magnitude; return Mathf.InverseLerp(controller.MaxShootDistance, controller.MinShootDistance, distance) * 7f + Mathf.Clamp01(Vector3.Dot(c.Self.forward, d.normalized)) * 2f + (HasClearPass(c.Self.position, c.TargetGoal.position) ? 3f : 0f); }
    private float ScorePass(AIPlayerContext c, Transform target) { if (target == null) return 0f; PassEvaluation e = EvaluateTarget(target); return e.valid ? e.score : 0f; }
    private float ScoreDribble(AIPlayerContext c) => controller.Role == TeamAIControllerStateDriven.Role.Attacker ? 5f : 3.5f;
    private float ScoreChase(AIPlayerContext c) => c.DistanceToBall > controller.ChaseDistance ? 0f : Mathf.InverseLerp(controller.ChaseDistance, 0f, c.DistanceToBall) * 10f;
    private float ScoreSupport(AIPlayerContext c) => c.HomePosition == null ? 0f : Mathf.InverseLerp(20f, 0f, Vector3.Distance(c.Self.position, c.HomePosition.position)) * 5f;
    private float ScoreDefend(AIPlayerContext c) => controller.Role == TeamAIControllerStateDriven.Role.Defender ? 7f : 2.5f;
    private float ScoreReturn(AIPlayerContext c) => c.HomePosition == null ? 0f : Mathf.InverseLerp(2f, 15f, Vector3.Distance(c.Self.position, c.HomePosition.position)) * 6f;

    private bool HasClearPass(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;
        return distance > 0.01f && !Physics.Raycast(origin + Vector3.up * 0.35f, direction.normalized, distance, controller.ObstacleMask, QueryTriggerInteraction.Ignore);
    }

    private readonly struct PassEvaluation
    {
        public readonly bool valid; public readonly float score; public readonly float accuracy; public readonly float pressure; public readonly float forwardSpace; public readonly bool firstTime;
        public PassEvaluation(bool valid, float score, float accuracy, float pressure, float forwardSpace, bool firstTime) { this.valid = valid; this.score = score; this.accuracy = accuracy; this.pressure = pressure; this.forwardSpace = forwardSpace; this.firstTime = firstTime; }
        public static PassEvaluation Invalid => new PassEvaluation(false, float.MinValue, 0f, 1f, 0f, false);
    }
}
