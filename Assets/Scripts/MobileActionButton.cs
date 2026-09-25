using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ActionType { Kick, Pass, Sprint, Lob, Curve, Tackle, Slide, Pressure, SwitchPlayer }
    [SerializeField] private ActionType action;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;
        switch (action)
        {
            case ActionType.Kick: case ActionType.Lob: case ActionType.Curve: MobileInput.Instance.BeginKick(); break;
            case ActionType.Pass: MobileInput.Instance.QueuePass(); break;
            case ActionType.Tackle: MobileInput.Instance.QueueTackle(); break;
            case ActionType.Slide: MobileInput.Instance.QueueSlide(); break;
            case ActionType.Pressure: MobileInput.Instance.SetSprint(true); break;
            case ActionType.Sprint: MobileInput.Instance.SetSprint(true); break;
            case ActionType.SwitchPlayer: MobileInput.Instance.RequestSwitch(); break;
        }
    }

    public void OnPointerUp(PointerEventData eventData) => ReleaseHeldAction();
    public void OnPointerExit(PointerEventData eventData) => ReleaseHeldAction();

    private void ReleaseHeldAction()
    {
        if (MobileInput.Instance == null) return;
        if (action == ActionType.Kick) MobileInput.Instance.ReleaseKick(BallController.ShotType.Ground);
        else if (action == ActionType.Lob) MobileInput.Instance.ReleaseKick(BallController.ShotType.Lob);
        else if (action == ActionType.Curve) MobileInput.Instance.ReleaseKick(BallController.ShotType.Curve);
        else if (action == ActionType.Sprint || action == ActionType.Pressure) MobileInput.Instance.SetSprint(false);
    }
}
