
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GolfBallController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
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
    [SerializeField] private BallData ballData;
    [SerializeField] private Bunker bunkerScript;

    private LevelManager levelManager;
    private bool ballCaptured;
    private bool betweenLevels = false;
    private float ballPower = 1f;
    private float wallBounceVelocityAdder = 1f;
    private float nextWallBounceTime;

    [Header("Shot Settings")]
    [SerializeField] private float maxShotForce = 30f;
    [SerializeField] private float powerOscillationSpeed = 2f;
    [SerializeField] private float slowDownTime = 3f;
    [SerializeField] private float aimLineLength = 10f;

    [Header("Wall Bounce Settings")]
    [SerializeField] private float wallBounceCooldown = 3f;

    private float puttTimer = 0f;
    private Vector2 startingVelocity;

    private bool isCharging = false;
    private bool isSlowingDown = false;
    private float currentPowerPercent = 0.01f;
    private Vector2 shotDirection;

    [Header("UI Visuals")]
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Gradient powerBarGradient;

    [Header("Bunker Settings")]
    [SerializeField] private bool allowBunkerEffects = true;
    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        InitializeBall();
    }
    public void InitializeBall()
    {
        if (ballData == null) return;

        spriteRenderer.sprite = ballData.ballSprite;
        ballPower = ballData.power;
        bunkerScript.speedMultiplier = ballData.traction;
        wallBounceVelocityAdder = ballData.bounciness;
    }

    public void SetBallData(BallData newBallData)
    {
        if (newBallData == null)
        {
            Debug.LogError("Cannot set the active ball because the BallData is null.", this);
            return;
        }

        ballData = newBallData;
        InitializeBall();
    }

    private void FixedUpdate()
    {
        if (betweenLevels)
        {
            isSlowingDown = false;
            return;
        }

        if (!isSlowingDown)
            return;

        puttTimer += Time.fixedDeltaTime;

        float slowdownRate =
            startingVelocity.magnitude /
            Mathf.Max(slowDownTime, Time.fixedDeltaTime);

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
        if (Time.timeScale <= 0f || betweenLevels)
            return;

        // Stop aiming and shooting while the ball is moving.
        if (rb.linearVelocity.magnitude > 0.05f)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;

            return;
        }

        HandleAiming();
        HandleAimingLine();
        HandleShot();
    }

    // BUNKER EFFECT
    public void ApplyBunkerEffect(float speedMultiplier)
    {
        if (!allowBunkerEffects ||
            rb == null ||
            ballCaptured ||
            betweenLevels)
        {
            return;
        }

        // Reduce the ball's current velocity.
        rb.linearVelocity *= speedMultiplier;
        // Keep the existing slowdown system consistent
        // with the new velocity.
        startingVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) // Ball Bounciness Effect
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            //Add wallBounceVelocityAdder to the current velocity of the ball
            rb.linearVelocity += rb.linearVelocity.normalized * wallBounceVelocityAdder;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ballCaptured || !other.CompareTag("Hole"))
            return;

        if (rb == null)
            return;

        ballCaptured = true;

        if (logicScript != null)
        {
            logicScript.AddScore(1000);

            if (scoreText != null)
            {
                scoreText.text =
                    "Score: " + logicScript.GetScore().ToString();
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

        betweenLevels = true;

        rb.position = other.transform.position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Invoke(nameof(ExitLevel), 3f);
    }

    private void HandleAiming()
    {
        Vector2 mouseScreenPos =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPos.x,
                    mouseScreenPos.y,
                    Camera.main.nearClipPlane
                )
            );

        shotDirection =
            ((Vector2)mouseWorldPos -
            (Vector2)transform.position).normalized;
    }
    private void HandleAimingLine()
    {
        if (lineRenderer != null)
        {
            float currentAimLineLength =
                aimLineLength * currentPowerPercent;

            lineRenderer.enabled = true;

            lineRenderer.SetPosition(0, transform.position);

            lineRenderer.SetPosition(
                1,
                transform.position +
                (Vector3)(shotDirection * currentAimLineLength)
            );

            lineRenderer.material.mainTextureScale =
                new Vector2(currentAimLineLength * 2f, 1f);
        }
    }
    private void HandleShot()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (shopUI != null && shopUI.activeSelf)
                    return;

                isCharging = true;
            }

            if (isCharging)
            {
                currentPowerPercent =
                    Mathf.PingPong(
                        Time.time * powerOscillationSpeed,
                        0.99f
                    ) + 0.01f;

                UpdateSliderUI();

                if (powerSlider != null)
                    powerSlider.value = currentPowerPercent;

                if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                {
                    ShootBall();

                    if (hitBallAudioSource != null && hitBall != null)
                    {
                        hitBallAudioSource.PlayOneShot(hitBall);
                    }

                    isCharging = false;
                }
            }
        }
    }
    private void ExitLevel()
    {
        if (shopUI != null)
            shopUI.SetActive(true);

        if (logicScript != null)
            logicScript.ResetStrokes();

        if (instructionText != null)
            instructionText.SetActive(false);

        EnableNextLevel();
    }

    private void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        ballCaptured = false;
        betweenLevels = false;

        if (lineRenderer != null)
            lineRenderer.enabled = true;

        transform.position = Vector3.zero;

        puttTimer = 0f;
        startingVelocity = Vector2.zero;
        isSlowingDown = false;
        isCharging = false;
        currentPowerPercent = 0.01f;
    }

    private void EnableNextLevel()
    {
        levelManager = FindAnyObjectByType<LevelManager>();

        if (levelManager != null)
        {
            levelManager.CompleteCurrentLevel();
        }

        ResetBall();
    }

    private void ShootBall()
    {
        if (rb == null)
            return;

        float actualForce = currentPowerPercent * maxShotForce * ballPower;

        rb.AddForce(shotDirection * actualForce,ForceMode2D.Impulse);

        logicScript.AddStroke();

        puttTimer = 0f;
        startingVelocity = rb.linearVelocity;
        isSlowingDown = true;

        currentPowerPercent = 0.01f;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void UpdateSliderUI()
    {
        if (powerSlider != null)
        {
            powerSlider.value = currentPowerPercent;
        }

        if (powerText != null)
        {
            int displayPercent =
                Mathf.RoundToInt(currentPowerPercent * 100f);

            powerText.text = $"{displayPercent}%";
        }

        if (sliderFillImage != null && powerBarGradient != null)
        {
            sliderFillImage.color =
                powerBarGradient.Evaluate(currentPowerPercent);
        }
    }
}
