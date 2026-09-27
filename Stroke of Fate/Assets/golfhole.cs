using UnityEngine;

public class golfhole : MonoBehaviour
{
    public Transform teleportLocation;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportLocation.position;
        }
    }
}


