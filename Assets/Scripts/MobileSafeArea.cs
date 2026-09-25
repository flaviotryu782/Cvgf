using UnityEngine;

public class MobileSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Vector2 extraPadding = new Vector2(24f, 18f);
    private Rect lastSafeArea;
    private ScreenOrientation lastOrientation;

    private void Awake() { if (panel == null) panel = transform as RectTransform; Apply(); }
    private void Update()
    {
        if (lastSafeArea != Screen.safeArea || lastOrientation != Screen.orientation) Apply();
    }
    private void Apply()
    {
        if (panel == null) return;
        Rect safe = Screen.safeArea;
        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x += extraPadding.x; min.y += extraPadding.y;
        max.x -= extraPadding.x; max.y -= extraPadding.y;
        panel.anchorMin = new Vector2(min.x / Screen.width, min.y / Screen.height);
        panel.anchorMax = new Vector2(max.x / Screen.width, max.y / Screen.height);
        panel.offsetMin = Vector2.zero; panel.offsetMax = Vector2.zero;
        lastSafeArea = safe; lastOrientation = Screen.orientation;
    }
}
