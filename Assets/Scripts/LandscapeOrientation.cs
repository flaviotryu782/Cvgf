using UnityEngine;

public class LandscapeOrientation : MonoBehaviour
{
    [SerializeField] private bool allowUpsideDown = false;

    private void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = allowUpsideDown;
        Screen.orientation = allowUpsideDown ? ScreenOrientation.AutoRotation : ScreenOrientation.LandscapeLeft;
    }

    private void Start()
    {
        if (Screen.width < Screen.height) Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
}
