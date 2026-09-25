using System.Collections;
using UnityEngine;

public class PenaltyAimButton : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler
{
    public enum ButtonType { Kick, AimLeft, AimRight, AimUp, AimDown }
    [SerializeField] private ButtonType type;
    [SerializeField] private float aimStep = .8f;
    private bool held;

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        held = true;
        if (type == ButtonType.Kick) FindObjectOfType<PenaltyShootoutManager>()?.BeginKick();
        else StartCoroutine(AimLoop());
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        held = false;
        if (type == ButtonType.Kick) FindObjectOfType<PenaltyShootoutManager>()?.ReleaseKick();
    }

    private IEnumerator AimLoop()
    {
        PenaltyShootoutManager manager = FindObjectOfType<PenaltyShootoutManager>();
        while (held && manager != null)
        {
            float delta = aimStep * Time.deltaTime;
            if (type == ButtonType.AimLeft) manager.SetAimHorizontal(-delta);
            if (type == ButtonType.AimRight) manager.SetAimHorizontal(delta);
            if (type == ButtonType.AimUp) manager.SetAimVertical(delta);
            if (type == ButtonType.AimDown) manager.SetAimVertical(-delta);
            yield return null;
        }
    }
}
