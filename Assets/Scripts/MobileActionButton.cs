using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ActionType { Kick, Pass, Sprint, Lob, Curve }
    [SerializeField] private ActionType action;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;
        if (action == ActionType.Kick || action == ActionType.Lob || action == ActionType.Curve) MobileInput.Instance.BeginKick();
        else if (action == ActionType.Pass) MobileInput.Instance.QueuePass();
        else MobileInput.Instance.SetSprint(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;
        if (action == ActionType.Kick) MobileInput.Instance.ReleaseKick(BallController.ShotType.Ground);
        else if (action == ActionType.Lob) MobileInput.Instance.ReleaseKick(BallController.ShotType.Lob);
        else if (action == ActionType.Curve) MobileInput.Instance.ReleaseKick(BallController.ShotType.Curve);
        else if (action == ActionType.Sprint) MobileInput.Instance.SetSprint(false);
    }
}
