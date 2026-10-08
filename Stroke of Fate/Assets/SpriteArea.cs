using UnityEngine;

public class SpriteArea : MonoBehaviour
{
    // The area/sprite you are currently on
    public GameObject currentArea;

    // The new area/sprite you want to activate
    public GameObject newArea;

    // Where the player will appear in the new area
    public Transform spawnPoint;

    // The player
    public GameObject player;

    private bool hasTransitioned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure only the player activates the transition
        if (other.CompareTag("Player") && !hasTransitioned)
        {
            hasTransitioned = true;

            // Turn off the area you were on
            if (currentArea != null)
            {
                currentArea.SetActive(false);
            }

            // Turn on the new area
            if (newArea != null)
            {
                newArea.SetActive(true);
            }

            // Move the player to the new area's spawn point
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
            }
        }
    }
}
