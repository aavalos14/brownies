using UnityEngine;

public class PlayerLastPosition : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 lastSafePosition;

    [Header("Resting Detection")]
    public float stopSpeed = 0.05f;
    public float stopDelay = 0.5f;

    private float stoppedTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastSafePosition = rb != null
            ? rb.position
            : (Vector2)transform.position;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        // Check whether the golf ball has nearly stopped
        if (rb.linearVelocity.magnitude < stopSpeed)
        {
            stoppedTimer += Time.fixedDeltaTime;

            // Save its position after it has remained still
            if (stoppedTimer >= stopDelay)
            {
                lastSafePosition = rb.position;
            }
        }
        else
        {
            stoppedTimer = 0f;
        }
    }

    public void ResetPosition()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = lastSafePosition;
        }
        else
        {
            transform.position = lastSafePosition;
        }

        stoppedTimer = 0f;
    }
}
