using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstep;

    private void Awake()
    {
        if (animationController == null) animationController = GetComponentInParent<PlayerAnimationController>();
    }

    // Adicione estes eventos aos clipes no Animation window.
    public void Footstep()
    {
        if (audioSource != null && footstep != null) audioSource.PlayOneShot(footstep);
    }

    public void FinishInjury() { }
    public void FinishAction() { }
}
