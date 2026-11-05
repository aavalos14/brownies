using UnityEngine;

public class ClickSpawnObjects : MonoBehaviour
{
    // Objects that will appear
    public GameObject[] objectsToSpawn;

    // How long they stay visible
    public float displayTime = 25f;

    private bool isActive = false;

    void OnMouseDown()
    {
        if (!isActive)
        {
            StartCoroutine(ShowObjects());
        }
    }

    System.Collections.IEnumerator ShowObjects()
    {
        isActive = true;

        // Make all objects appear
        foreach (GameObject obj in objectsToSpawn)
        {
            obj.SetActive(true);
        }

        // Wait 25 seconds
        yield return new WaitForSeconds(displayTime);

        // Make all objects disappear
        foreach (GameObject obj in objectsToSpawn)
        {
            obj.SetActive(false);
        }

        isActive = false;
    }
}
