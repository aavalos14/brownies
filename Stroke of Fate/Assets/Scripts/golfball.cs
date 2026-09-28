using UnityEngine;
using UnityEngine.InputSystem;

public class golfball : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 clickPosition;
    private bool isReadyToPutt = true;

    [Header("Putt Settings")]
    public float powerMultiplier = 5f;
    public float maxPower = 15f;
    public float stopVelocityThreshold = 0.05f;

    [Header("Slow Down Settings")]
    public float slowDownTime = 3f;

    private float puttTimer = 0f;
    private Vector2 startingVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isReadyToPutt)
        {
            HandlePuttInput();
        }
    }

    void FixedUpdate()
    {
        if (isReadyToPutt)
        {
            return;
        }

        if (rb.linearVelocity.magnitude < stopVelocityThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            isReadyToPutt = true;
            puttTimer = 0f;
            return;
        }

        puttTimer += Time.fixedDeltaTime;
        float slowdownRate = startingVelocity.magnitude / Mathf.Max(slowDownTime, Time.fixedDeltaTime);
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            Vector2.zero,
            slowdownRate * Time.fixedDeltaTime
        );

        if (puttTimer >= slowDownTime)
        {
            rb.linearVelocity = Vector2.zero;
            puttTimer = 0f;
            isReadyToPutt = true;
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

                // Start the timer
                puttTimer = 0f;

                // Remember the velocity from the new putt
                startingVelocity = rb.linearVelocity;
                isReadyToPutt = false;
            }
        }
    }
}

