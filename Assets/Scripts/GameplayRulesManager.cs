using UnityEngine;

public class GameplayRulesManager : MonoBehaviour
{
    [SerializeField] private OffsideSystem offsideSystem;
    [SerializeField] private FoulSystem foulSystem;
    [SerializeField] private GameObject offsidePanel;
    [SerializeField] private GameObject foulPanel;

    private void OnEnable()
    {
        if (foulSystem != null) foulSystem.FoulCalled += OnFoul;
    }
    private void OnDisable()
    {
        if (foulSystem != null) foulSystem.FoulCalled -= OnFoul;
    }
    private void Update()
    {
        if (offsidePanel != null) offsidePanel.SetActive(offsideSystem != null && offsideSystem.IsOffside);
    }
    public bool ValidatePass(Transform receiver)
    {
        bool offside = offsideSystem != null && offsideSystem.CheckAtPass(receiver);
        if (offside && offsidePanel != null) offsidePanel.SetActive(true);
        return !offside;
    }
    private void OnFoul(bool penalty)
    {
        if (foulPanel != null) foulPanel.SetActive(true);
    }
}
