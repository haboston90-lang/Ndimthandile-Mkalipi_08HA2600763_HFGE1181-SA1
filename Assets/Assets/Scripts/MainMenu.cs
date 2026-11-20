using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public void PlayGame()
    {
        if (audioManager) audioManager.PlayClick();
        SceneManager.LoadScene("Game"); // Make sure your game scene is named Game
    }
    public void ExitGame()
    {
        if (audioManager) audioManager.PlayClick();
        Application.Quit();
    }
}