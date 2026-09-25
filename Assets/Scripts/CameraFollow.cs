using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -12f);
    [SerializeField] private float smooth = 8f;
    private void Start()
    {
        if (CameraDirector.Instance != null) CameraDirector.Instance.SetTargets(target, null, null);
    }
    private void LateUpdate()
    {
        if (CameraDirector.Instance != null) return;
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}
