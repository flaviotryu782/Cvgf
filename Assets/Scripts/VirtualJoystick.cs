using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 90f;
    [SerializeField] private bool dynamicOrigin;
    private RectTransform area;
    private int pointerId = -99;

    private void Awake() => area = transform as RectTransform;
    public void OnPointerDown(PointerEventData eventData) { if (pointerId != -99) return; pointerId = eventData.pointerId; OnDrag(eventData); }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != pointerId || MobileInput.Instance == null || area == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out Vector2 local);
        Vector2 value = Vector2.ClampMagnitude(local / radius, 1f);
        if (handle != null) handle.anchoredPosition = value * radius;
        MobileInput.Instance.SetMove(value);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != pointerId) return;
        pointerId = -99;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        MobileInput.Instance?.SetMove(Vector2.zero);
    }
}
