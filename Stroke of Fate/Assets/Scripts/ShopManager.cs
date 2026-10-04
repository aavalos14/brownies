using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public void BeginNextLevel()
    {
        SceneManager.LoadScene("Level");
    }
}
