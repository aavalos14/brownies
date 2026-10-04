using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GolfBallController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private AudioSource hitBallAudioSource;
    [SerializeField] private AudioSource hitHoleAudioSource;
    [SerializeField] private AudioClip hitBall;
    [SerializeField] private AudioClip hitHole;

    [Header("Shot Settings")]
    [SerializeField] private float maxShotForce = 30f;
    [SerializeField] private float powerOscillationSpeed = 2f;
    [SerializeField] private float slowDownTime = 3f;
    [SerializeField] private float aimLineLength = 10f;
    private float puttTimer = 0f;
    private Vector2 startingVelocity;

    private bool isCharging = false;
    private bool isSlowingDown = false;
    private float currentPowerPercent = 0.01f;
    private Vector2 shotDirection;
    [Header("UI Visuals")]
    [SerializeField] private Image sliderFillImage;     // Drag the 'Fill' GameObject's Image here
    [SerializeField] private Gradient powerBarGradient; // Configure Green -> Yellow -> Red in Inspector

    
    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Hide line by default
        }
            
    }
    void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            isSlowingDown = false;
            return;
        }

        if (!isSlowingDown) return;

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
            isSlowingDown = false;
        }
    }

    void Update()
    {
        if (rb.bodyType == RigidbodyType2D.Static) return;

        // Stop aiming/shooting while moving
        if (rb.linearVelocity.magnitude > 0.05f) 
        {
            if (lineRenderer != null) lineRenderer.enabled = false;
            return;
        }

        // Ensure mouse device exists
        if (Mouse.current == null) return;

        // 1. Aim toward mouse position using Mouse.current.position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
        shotDirection = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

        // 1.5. Update LineRenderer to show aiming direction
        if (lineRenderer != null)
        {
            float currentAimLineLength = aimLineLength * currentPowerPercent;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position); // Start at ball
            lineRenderer.SetPosition(1, transform.position + (Vector3)(shotDirection * currentAimLineLength));

            // Tile the texture dynamically so dashes don't stretch when moving cursor
            lineRenderer.material.mainTextureScale = new Vector2(currentAimLineLength * 2f, 1f);
        }

        // 2. Handle Spacebar input via Keyboard.current
        if (Keyboard.current != null)
        {
            // Key pressed this frame
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                isCharging = true;
            }

            // Charging cycle
            if (isCharging)
            {
                currentPowerPercent = Mathf.PingPong(Time.time * powerOscillationSpeed, 0.99f) + 0.01f;

                UpdateSliderUI();
                
                if (powerSlider != null)
                    powerSlider.value = currentPowerPercent;

                // Key released this frame
                if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                {
                    ShootBall();
                    hitBallAudioSource.PlayOneShot(hitBall);
                    isCharging = false;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hole"))
        {
            hitHoleAudioSource.PlayOneShot(hitHole);
            // Wait 3 seconds and then load the shop
            Invoke("LoadShop", 3f);
        }
    }
    void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }
    void ShootBall()
    {
        float actualForce = currentPowerPercent * maxShotForce;
        rb.AddForce(shotDirection * actualForce, ForceMode2D.Impulse);
        puttTimer = 0f;
        startingVelocity = rb.linearVelocity;
        isSlowingDown = true;

        currentPowerPercent = 0.01f;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void UpdateSliderUI()
    {
        if (powerSlider != null)
        {
            powerSlider.value = currentPowerPercent;
        }
        // Display power converted to integer percentage (1% to 100%)
        if (powerText != null)
        {
            int displayPercent = Mathf.RoundToInt(currentPowerPercent * 100f);
            powerText.text = $"{displayPercent}%";
        }

        // Evaluates currentPowerPercent (0.01 to 1.0) against the gradient
        if (sliderFillImage != null && powerBarGradient != null)
        {
            sliderFillImage.color = powerBarGradient.Evaluate(currentPowerPercent);
        }
    }
}