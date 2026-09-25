public interface IAIState
{
    AIStateId Id { get; }
    void Enter();
    void Tick();
    void Exit();
}
