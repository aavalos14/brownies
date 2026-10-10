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
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource hitBallAudioSource;
    [SerializeField] private AudioSource hitHoleAudioSource;
    [SerializeField] private AudioClip hitBall;
    [SerializeField] private AudioClip hitHole;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject instructionText;
    [SerializeField] private logic logicScript;
    private LevelManager levelManager;
    private bool ballCaptured;
    private bool betweenLevels = false;

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

    
    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Hide line by default
        }
            
    }
    private void FixedUpdate()
    {
        if (betweenLevels)
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

    private void Update()
    {
        if (betweenLevels) return;

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
                if (shopUI.activeSelf)
                {
                    return;
                }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ballCaptured || !other.CompareTag("Hole"))
        {
            return;
        }

        if (rb == null)
        {
            return;
        }

        ballCaptured = true;

        if (logicScript != null)
        {
            logicScript.AddScore(1000); // Add 1000 points for sinking the ball
            if (scoreText != null)
            {
                scoreText.text = "Score: " + logicScript.GetScore().ToString();
            }
        }

        if (hitHoleAudioSource != null && hitHole != null)
        {
            hitHoleAudioSource.PlayOneShot(hitHole);
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        betweenLevels = true; // Set flag to indicate we're between levels
        rb.position = other.transform.position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        Invoke(nameof(ExitLevel), 3f);
    }
    private void ExitLevel()
    {
        shopUI.SetActive(true);
        if (instructionText != null)
        {
            instructionText.SetActive(false);
        }
        EnableNextLevel();
    }
    private void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        ballCaptured = false;
        betweenLevels = false; // Reset the between levels flag
        lineRenderer.enabled = true; // Show line renderer
        transform.position = Vector3.zero; // Reset position to origin (or any desired position)
    }
    private void EnableNextLevel()
    {
       levelManager = FindAnyObjectByType<LevelManager>();
       if (levelManager != null)
       {
           levelManager.CompleteCurrentLevel();
       }
       ResetBall(); // Reset ball for the next level
    }
    private void ShootBall()
    {
        float actualForce = currentPowerPercent * maxShotForce;
        rb.AddForce(shotDirection * actualForce, ForceMode2D.Impulse);
        puttTimer = 0f;
        startingVelocity = rb.linearVelocity;
        isSlowingDown = true;

        currentPowerPercent = 0.01f;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    private void UpdateSliderUI()
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