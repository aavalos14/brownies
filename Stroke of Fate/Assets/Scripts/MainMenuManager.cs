using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void BeginRun()
    {
        SceneManager.LoadScene("LevelScene");
    }
}