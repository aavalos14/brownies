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
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject instructionText;
    [SerializeField] private logic logicScript;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private ShopButtonController shopButtonController;
    [SerializeField] private GameObject[] shopButtons;
    [SerializeField] private BallData ballData;
    [SerializeField] private FMODUnity.EventReference wallCollisionEvent;
    [SerializeField] private FMODUnity.EventReference shotEvent;
    [SerializeField] private FMODUnity.EventReference holeCollisionEvent;
    private bool ballCaptured;
    private bool betweenLevels = false;
    private float ballPower = 1f;
    private float wallVelocityMultiplier = 1f;
    private int ballIncome = 0;

    [Header("Shot Settings")]
    [SerializeField] private float maxShotForce = 30f;
    [SerializeField] private float powerOscillationSpeed = 2f;
    [SerializeField] private float slowDownTime = 3f;
    [SerializeField] private float aimLineLength = 10f;

    private float puttTimer = 0f;
    private Vector2 startingVelocity;

    private bool isCharging = false;
    public bool IsCharging => isCharging;

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
        wallVelocityMultiplier = ballData.bounciness;
        ballIncome = ballData.income;
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
    public void ApplyBunkerEffect()
    {
        if (!allowBunkerEffects ||
            rb == null ||
            ballData == null ||
            ballCaptured ||
            betweenLevels)
        {
            return;
        }

        // Reduce the ball's current velocity.
        rb.linearVelocity *= ballData.traction;
        // Keep the existing slowdown system consistent
        // with the new velocity.
        startingVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            //Add wallVelocityMultiplier to the current velocity of the ball
            rb.linearVelocity += rb.linearVelocity.normalized * wallVelocityMultiplier;
            FMODUnity.RuntimeManager.PlayOneShot(wallCollisionEvent, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ballCaptured || !other.CompareTag("Hole"))
            return;

        if (rb == null)
            return;

        ballCaptured = true;
        FMODUnity.RuntimeManager.PlayOneShot(holeCollisionEvent, transform.position);

        if (logicScript != null)
        {
            HandleAddingScore();
            logicScript.AddGold(ballIncome);
            logicScript.UpdateBirdText();
            logicScript.UpdateIncomeText(ballIncome);
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
    private void HandleAddingScore()
    {
        int holeScore;

        if (levelManager.par - logicScript.stroke == 0)
        {
            holeScore = 1000;
        }
        else if (levelManager.par - logicScript.stroke == 1)
        {
            holeScore = 2000;
        }
        else if (levelManager.par - logicScript.stroke == 2)
        {
            holeScore = 3000;
        }
        else if (levelManager.par - logicScript.stroke >= 3)
        {
            holeScore = 4000;
        }
        else
        {
            holeScore = 0;
        }
        logicScript.AddScore(holeScore);
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

                    isCharging = false;
                }
            }
        }
    }
    private void ExitLevel()
    {
        if (shopUI != null)
            shopUI.SetActive(true);
            shopButtonController.ActivateShopButtons(shopButtons);


        if (logicScript != null)
        {
            logicScript.ResetStrokes();
            logicScript.AddHole();
            logicScript.RemoveBirdText();
        }

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
        FMODUnity.RuntimeManager.PlayOneShot(shotEvent, transform.position);

        puttTimer = 0f;
        startingVelocity = rb.linearVelocity;
        isSlowingDown = true;

        currentPowerPercent = 0.01f;

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        //if ball is in hole, return, otherwise add stroke
        Invoke(nameof(AddStrokeIfNotInHole), 3f);
    }
    private void AddStrokeIfNotInHole()
    {
        if (logicScript != null && !ballCaptured)
            logicScript.AddStroke();
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
