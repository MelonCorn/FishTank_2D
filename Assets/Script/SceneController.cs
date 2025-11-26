using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
