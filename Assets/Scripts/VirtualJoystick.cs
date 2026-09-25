using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 80f;
    private RectTransform area;

    private void Awake() => area = transform as RectTransform;

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (MobileInput.Instance == null || area == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out Vector2 local);
        Vector2 value = Vector2.ClampMagnitude(local / radius, 1f);
        if (handle != null) handle.anchoredPosition = value * radius;
        MobileInput.Instance.SetMove(value);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (MobileInput.Instance != null) MobileInput.Instance.SetMove(Vector2.zero);
    }
}
