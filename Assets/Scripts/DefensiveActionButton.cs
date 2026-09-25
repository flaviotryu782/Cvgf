using UnityEngine;
using UnityEngine.EventSystems;

public class DefensiveActionButton : MonoBehaviour, IPointerDownHandler
{
    public enum Action { Pressure, Tackle, Slide }
    [SerializeField] private Action action;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;
        if (action == Action.Tackle) MobileInput.Instance.QueueTackle();
        else if (action == Action.Slide) MobileInput.Instance.QueueSlide();
        else MobileInput.Instance.SetSprint(true);
    }
}
