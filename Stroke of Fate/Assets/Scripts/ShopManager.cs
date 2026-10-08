// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class ShopManager : MonoBehaviour
// {
//     public void BeginNextLevel()
//     {
//         SceneManager.LoadScene("Level");
//     }
// }

// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class ShopManager : MonoBehaviour
// {
//     // The area number we want to load
//     public int areaToLoad = 1;

//     public void BeginNextLevel()
//     {
//         // Remember which area the player should appear in
//         PlayerPrefs.SetInt("AreaToLoad", areaToLoad);
//         PlayerPrefs.Save();

//         // Load the Level scene
//         SceneManager.LoadScene("Level");
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    // Name of the area the player should appear in
    public string newSpriteName = "Grassland Level 1";

    public void BeginNextLevel()
    {
        // Remember the selected sprite/area
        PlayerPrefs.SetString("newSprite", newSpriteName);

        // Load the Level scene
        SceneManager.LoadScene("Level");
    }
}