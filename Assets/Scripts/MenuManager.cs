using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainGame";
    [SerializeField] private AudioManager audioManager;

    private void Start() { if (audioManager == null) audioManager = AudioManager.Instance; audioManager?.PlayMenuMusic(); }
    public void PlayGame() => SceneManager.LoadScene(gameSceneName);
    public void QuitGame() => Application.Quit();
    public void ToggleVibration(bool enabled) { if (AudioManager.Instance != null) AudioManager.Instance.VibrationEnabled = enabled; }
}
