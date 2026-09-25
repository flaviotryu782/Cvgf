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

    private AIAction DecideWithBall(AIPlayerContext context)
    {
        float shootScore = ScoreShoot(context);
        float passScore = ScorePass(context);
        float dribbleScore = ScoreDribble(context);

        if (shootScore >= passScore && shootScore >= dribbleScore)
            return AIAction.Shoot;

        if (passScore >= dribbleScore)
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

        if (supportScore > highest)
        {
            highest = supportScore;
            best = AIAction.Support;
        }

        if (defendScore > highest)
        {
            highest = defendScore;
            best = AIAction.Defend;
        }

        if (returnScore > highest)
            best = AIAction.Return;

        return best;
    }

    private float ScoreShoot(AIPlayerContext context)
    {
        if (context.TargetGoal == null)
            return 0f;

        float distance = Vector3.Distance(context.Self.position, context.TargetGoal.position);
        return Mathf.InverseLerp(25f, 5f, distance) * 10f;
    }

    private float ScorePass(AIPlayerContext context)
    {
        return 5f;
    }

    private float ScoreDribble(AIPlayerContext context)
    {
        return 4f;
    }

    private float ScoreChase(AIPlayerContext context)
    {
        if (context.DistanceToBall > controller.ChaseDistance)
            return 0f;

        return Mathf.InverseLerp(controller.ChaseDistance, 0f, context.DistanceToBall) * 10f;
    }

    private float ScoreSupport(AIPlayerContext context)
    {
        if (context.Ball == null || context.HomePosition == null)
            return 0f;

        float distanceToHome = Vector3.Distance(context.Self.position, context.HomePosition.position);
        return Mathf.InverseLerp(20f, 0f, distanceToHome) * 5f;
    }

    private float ScoreDefend(AIPlayerContext context)
    {
        return controller.Role == TeamAIControllerStateDriven.Role.Defender ? 7f : 2f;
    }

    private float ScoreReturn(AIPlayerContext context)
    {
        if (context.HomePosition == null)
            return 0f;

        float distance = Vector3.Distance(context.Self.position, context.HomePosition.position);
        return Mathf.InverseLerp(2f, 15f, distance) * 6f;
    }
}
