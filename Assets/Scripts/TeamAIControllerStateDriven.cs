using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeamAIControllerStateDriven : MonoBehaviour
{
    public enum Role
    {
        Attacker,
        Midfielder,
        Defender
    }

    [Header("Team")]
    [SerializeField] private Role role = Role.Midfielder;
    [SerializeField] private string teamId = "opponent";

    [Header("References")]
    [SerializeField] private Transform ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private Transform targetGoal;
    [SerializeField] private Transform homePosition;
    [SerializeField] private Transform ballSocket;
    [SerializeField] private AdaptiveAIBrain adaptiveBrain;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4.2f;
    [SerializeField] private float chaseDistance = 18f;
    [SerializeField] private float controlRange = 2f;

    [Header("Actions")]
    [SerializeField] private float shotPower = 14f;
    [SerializeField] private float decisionInterval = 0.25f;

    private CharacterController characterController;
    private PlayerAnimationController animationController;
    private AIStateMachine stateMachine;
    private AIDecisionSystem decisionSystem;
    private AIPlayerContext context;
    private float nextDecisionTime;

    public Role RoleValue => role;
    public Role Role => role;
    public Transform Ball => ball;
    public BallController BallController => ballController;
    public Transform TargetGoal => targetGoal;
    public Transform HomePosition => homePosition;
    public Transform BallSocket => ballSocket;
    public CharacterController CharacterController => characterController;
    public PlayerAnimationController AnimationController => animationController;
    public AIPlayerContext Context => context;
    public AIDecisionSystem DecisionSystem => decisionSystem;
    public float MoveSpeed => moveSpeed;
    public float ChaseDistance => chaseDistance;
    public float ControlRange => controlRange;
    public float ShotPower => shotPower;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animationController = GetComponentInChildren<PlayerAnimationController>();

        context = new AIPlayerContext(this);
        decisionSystem = new AIDecisionSystem(this);

        stateMachine = new AIStateMachine();
        stateMachine.Register(new ChaseBallState(context, this));
        stateMachine.Register(new ReturnToPositionState(context, this));
        stateMachine.Register(new DribbleState(context, this));
        stateMachine.Register(new ShootState(context, this));

        stateMachine.ChangeState(AIStateId.ReturnToPosition);

        if (adaptiveBrain != null)
            adaptiveBrain.SetProfile(teamId, role);
    }

    private void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + decisionInterval;
            EvaluateTransition();
        }

        stateMachine.Tick();
    }

    private void EvaluateTransition()
    {
        AIAction action = decisionSystem.Decide();

        if (context.HasBall)
        {
            if (action == AIAction.Shoot)
            {
                ChangeState(AIStateId.Shoot);
                return;
            }

            if (action == AIAction.Pass)
            {
                ChangeState(AIStateId.Pass);
                return;
            }

            ChangeState(AIStateId.Dribble);
            return;
        }

        if (action == AIAction.Chase)
        {
            ChangeState(AIStateId.ChaseBall);
            return;
        }

        ChangeState(AIStateId.ReturnToPosition);
    }

    public void ChangeState(AIStateId state)
    {
        stateMachine.ChangeState(state);
    }
}
