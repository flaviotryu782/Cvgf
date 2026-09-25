using UnityEngine;

public sealed class ChaseBallState : IAIState
{
    private readonly AIPlayerContext context;
    private readonly TeamAIControllerStateDriven controller;

    public AIStateId Id => AIStateId.ChaseBall;

    public ChaseBallState(AIPlayerContext context, TeamAIControllerStateDriven controller)
    {
        this.context = context;
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Tick()
    {
        if (context.Ball == null)
            return;

        if (context.BallController != null && context.BallController.Owner != null && context.BallController.Owner != context.Self)
        {
            // Outro jogador já está com a bola; o agente só deve disputar se estiver em faixa de pressão.
            if (context.DistanceToBall <= controller.ChaseDistance)
            {
                context.MoveTo(context.Ball.position, controller.MoveSpeed * 0.8f);
            }
            else
            {
                controller.ChangeState(AIStateId.ReturnToPosition);
            }
            return;
        }

        context.MoveTo(context.Ball.position, controller.MoveSpeed);

        if (context.TryTakePossession(controller.ControlRange))
        {
            controller.ChangeState(AIStateId.Dribble);
        }
    }

    public void Exit()
    {
    }
}
