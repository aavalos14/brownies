using UnityEngine;
using UnityEngine.InputSystem;

public class golfball : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 clickPosition;
    private bool isReadyToPutt = true;

    public float powerMultiplier = 5f;
    public float maxPower = 15f;
    public float stopVelocityThreshold = 0.05f;

    // NEW: How long the ball can roll before stopping
    public float slowDownTime = 3f;

    // NEW: Keeps track of how long the ball has been moving
    private float puttTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if the ball has stopped moving
        if (rb.linearVelocity.magnitude < stopVelocityThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            isReadyToPutt = true;

            // Reset timer
            puttTimer = 0f;
        }
        else
        {
            isReadyToPutt = false;

            // NEW: Count how long the ball has been moving
            puttTimer += Time.deltaTime;

            // NEW: Stop the ball after the chosen amount of time
            if (puttTimer >= slowDownTime)
            {
                rb.linearVelocity = Vector2.zero;
                puttTimer = 0f;
                isReadyToPutt = true;
            }
        }

        if (isReadyToPutt)
        {
            HandlePuttInput();
        }
    }

    void HandlePuttInput()
    {
        // Mouse button pressed
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickPosition = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );
        }

        // Mouse button released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 releasePosition = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            // Calculate direction
            Vector2 direction = clickPosition - releasePosition;

            // Calculate power
            float distance = direction.magnitude;

            float finalPower = Mathf.Clamp(
                distance * powerMultiplier,
                0f,
                maxPower
            );

            // Apply force
            if (finalPower > 0.1f && direction.magnitude > 0.01f)
            {
                rb.AddForce(
                    direction.normalized * finalPower,
                    ForceMode2D.Impulse
                );

                // NEW: Start the timer when the ball is hit
                puttTimer = 0f;
            }
        }
    }
}

