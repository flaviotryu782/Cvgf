using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ActionType { Kick, Pass, Sprint }
    [SerializeField] private ActionType action;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;
        if (action == ActionType.Kick) MobileInput.Instance.QueueKick();
        else if (action == ActionType.Pass) MobileInput.Instance.QueuePass();
        else MobileInput.Instance.SetSprint(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (action == ActionType.Sprint && MobileInput.Instance != null) MobileInput.Instance.SetSprint(false);
    }
}
