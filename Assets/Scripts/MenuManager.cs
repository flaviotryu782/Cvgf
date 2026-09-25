using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainGame";

    public void PlayGame() => SceneManager.LoadScene(gameSceneName);
    public void QuitGame() => Application.Quit();
}
