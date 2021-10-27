using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Quit() {
        Application.Quit();
    }

    public void LoadGame() {
        SceneManager.LoadScene(1);
    }
}
