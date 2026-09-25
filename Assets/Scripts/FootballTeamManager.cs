using UnityEngine;

public class FootballTeamManager : MonoBehaviour
{
    public enum TeamSide { Player, Opponent }

    [Header("Team")]
    [SerializeField] private TeamSide side = TeamSide.Player;
    [SerializeField] private PlayerController[] outfieldPlayers;
    [SerializeField] private TeamAIController[] aiPlayers;
    [SerializeField] private GoalkeeperController goalkeeper;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool enablePlayerSwitching = true;

    private int activeIndex;
    public PlayerController ActivePlayer => outfieldPlayers != null && outfieldPlayers.Length > 0 ? outfieldPlayers[activeIndex] : null;
    public PlayerController[] OutfieldPlayers => outfieldPlayers;
    public GoalkeeperController Goalkeeper => goalkeeper;

    private void Start()
    {
        ConfigureTeam();
        if (side == TeamSide.Player && MobileInput.Instance != null)
            MobileInput.Instance.SwitchRequested += SwitchPlayer;
    }

    private void OnDestroy()
    {
        if (side == TeamSide.Player && MobileInput.Instance != null)
            MobileInput.Instance.SwitchRequested -= SwitchPlayer;
    }

    private void Update()
    {
        if (side == TeamSide.Player && enablePlayerSwitching && Input.GetKeyDown(KeyCode.Tab))
            SwitchPlayer();
    }

    public void ConfigureTeam()
    {
        if (outfieldPlayers == null) return;
        for (int i = 0; i < outfieldPlayers.Length; i++)
        {
            if (outfieldPlayers[i] == null) continue;
            bool controlled = side == TeamSide.Player && i == activeIndex;
            outfieldPlayers[i].enabled = controlled;
            TeamAIController ai = outfieldPlayers[i].GetComponent<TeamAIController>();
            if (ai != null) ai.enabled = !controlled;
        }
    }

    public void SwitchPlayer()
    {
        if (side != TeamSide.Player || outfieldPlayers == null || outfieldPlayers.Length < 2) return;
        activeIndex = (activeIndex + 1) % outfieldPlayers.Length;
        ConfigureTeam();
    }

    public void ResetTeam()
    {
        if (outfieldPlayers == null) return;
        int spawnCount = spawnPoints == null ? 0 : spawnPoints.Length;
        for (int i = 0; i < outfieldPlayers.Length && i < spawnCount; i++)
        {
            if (outfieldPlayers[i] == null || spawnPoints[i] == null) continue;
            CharacterController controller = outfieldPlayers[i].GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            outfieldPlayers[i].transform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            if (controller != null) controller.enabled = true;
        }
        if (goalkeeper != null && spawnPoints != null && spawnPoints.Length > outfieldPlayers.Length && spawnPoints[outfieldPlayers.Length] != null)
            goalkeeper.transform.SetPositionAndRotation(spawnPoints[outfieldPlayers.Length].position, spawnPoints[outfieldPlayers.Length].rotation);
        ConfigureTeam();
    }
}
