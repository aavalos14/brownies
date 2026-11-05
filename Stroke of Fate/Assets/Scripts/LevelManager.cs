using UnityEngine;
using FMODUnity;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levels; // Assign Level_1, Level_2, etc., here in the Inspector
    private int currentLevelIndex = 0;
    public int par = 5; // Default par value, can be adjusted per level if needed
    [SerializeField] private StudioEventEmitter eventEmitter;
    [SerializeField] private string levelsCompleted = "Levels Completed";

    public void LoadLevel(int index)
    {
        // Ensure index doesn't exceed total levels
        if (index < 0 || index >= levels.Length) return;

        // Deactivate all levels first
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        // Activate the target level
        levels[index].SetActive(true);
        currentLevelIndex = index;
    }

    // Call this method when the current level is finished
    public void CompleteCurrentLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        eventEmitter.SetParameter(levelsCompleted, nextLevelIndex);

        if (nextLevelIndex < levels.Length)
        {
            // Instantly transition to the next level
            LoadLevel(nextLevelIndex);
        }
        else
        {
            Debug.Log("Game Cleared! No more levels.");
        }
    }
}