using UnityEngine;

public class glorioushole  : MonoBehaviour
{
    // CHANGE THIS:
    // Drag the location you want the player to teleport to
    // into this field in the Inspector.
    public Transform teleportLocation;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // CHANGE THIS:
            // Moves the player to the new location
            other.transform.position = teleportLocation.position;
        }
    }
}

