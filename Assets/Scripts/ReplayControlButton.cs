using UnityEngine;
using UnityEngine.EventSystems;

public class ReplayControlButton : MonoBehaviour, IPointerClickHandler
{
    public enum Action { Skip, NextAngle, SaveBest, Play }
    [SerializeField] private Action action;
    [SerializeField] private ReplayRecorder recorder;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (recorder == null) recorder = FindObjectOfType<ReplayRecorder>();
        if (recorder == null) return;
        if (action == Action.Skip) recorder.SkipReplay();
        else if (action == Action.NextAngle) recorder.NextCameraAngle();
        else if (action == Action.SaveBest) recorder.SaveBestReplay();
        else recorder.PlayLatestReplay();
    }
}
