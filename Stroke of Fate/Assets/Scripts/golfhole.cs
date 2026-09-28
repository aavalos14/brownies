using UnityEngine;

public class golfhole : MonoBehaviour
{
    private bool ballCaptured;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ballCaptured || !other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody2D ballBody = other.attachedRigidbody;
        if (ballBody == null)
        {
            return;
        }

        ballCaptured = true;
        ballBody.position = new Vector2(transform.position.x, transform.position.y);
        ballBody.linearVelocity = Vector2.zero;
        ballBody.angularVelocity = 0f;

        golfball ballController = ballBody.GetComponent<golfball>();
        if (ballController != null)
        {
            ballController.enabled = false;
        }

        ballBody.bodyType = RigidbodyType2D.Static;
    }
}


