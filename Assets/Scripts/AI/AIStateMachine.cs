using System.Collections.Generic;

public sealed class AIStateMachine
{
    private readonly Dictionary<AIStateId, IAIState> states = new Dictionary<AIStateId, IAIState>();
    private IAIState currentState;

    public AIStateId CurrentState => currentState != null ? currentState.Id : AIStateId.ReturnToPosition;

    public void Register(IAIState state)
    {
        if (state == null)
            return;

        states[state.Id] = state;
    }

    public void ChangeState(AIStateId id)
    {
        if (!states.TryGetValue(id, out IAIState nextState))
            return;

        if (currentState != null && currentState.Id == id)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void Tick()
    {
        currentState?.Tick();
    }
}
