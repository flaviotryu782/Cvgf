using UnityEngine;
using UnityEngine.EventSystems;

public class MobileGameplayButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Action { Protect, Dribble, AutoSwitch }
    [SerializeField] private Action action;
    [SerializeField] private MobileGameplayActions gameplay;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (action == Action.AutoSwitch) { MobileInput.Instance?.RequestSwitch(); return; }
        if (gameplay == null) gameplay = FindObjectOfType<MobileGameplayActions>();
        if (gameplay == null) return;
        if (action == Action.Protect) gameplay.SetProtection(true); else gameplay.SetDribble(true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (gameplay == null) return;
        if (action == Action.Protect) gameplay.SetProtection(false); else if (action == Action.Dribble) gameplay.SetDribble(false);
    }
}
