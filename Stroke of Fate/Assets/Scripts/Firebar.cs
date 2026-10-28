
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlameObstacle : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField] private float maxLength = 5f;
    [SerializeField] private float extendTime = 1f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float retractTime = 0.5f;
    [SerializeField] private float delayBetweenBursts = 1f;
    [SerializeField] private float flameWidth = 0.6f;

    [Header("Detection")]
    [SerializeField] private LayerMask ballLayer = ~0;

    private LineRenderer flameLine;
    private bool ballHit;
    private bool flameActive;

    private void Awake()
    {
        flameLine = GetComponent<LineRenderer>();

        flameLine.useWorldSpace = true;
        flameLine.positionCount = 2;
        flameLine.startWidth = 0f;
        flameLine.endWidth = 0f;
        flameLine.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(FlameRoutine());
    }

    private IEnumerator FlameRoutine()
    {
        while (true)
        {
            // Gradually extend the flame.
            flameActive = true;
            flameLine.enabled = true;

            float timer = 0f;

            while (timer < extendTime)
            {
                timer += Time.deltaTime;

                float progress = Mathf.Clamp01(timer / extendTime);
                float length = Mathf.Lerp(0f, maxLength, progress);

                UpdateFlame(length);
                CheckForBall(length);

                yield return null;
            }

            UpdateFlame(maxLength);
            CheckForBall(maxLength);

            // Keep the flame extended.
            timer = 0f;

            while (timer < holdTime)
            {
                timer += Time.deltaTime;

                UpdateFlame(maxLength);
                CheckForBall(maxLength);

                yield return null;
            }

            // Gradually retract the flame.
            timer = 0f;

            while (timer < retractTime)
            {
                timer += Time.deltaTime;

                float progress = Mathf.Clamp01(timer / retractTime);
                float length = Mathf.Lerp(maxLength, 0f, progress);

                UpdateFlame(length);
                CheckForBall(length);

                yield return null;
            }

            flameActive = false;
            flameLine.enabled = false;
            ballHit = false;

            yield return new WaitForSeconds(delayBetweenBursts);
        }
    }

    private void UpdateFlame(float length)
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        flameLine.SetPosition(0, origin);
        flameLine.SetPosition(1, origin + direction * length);

        // The flame becomes wider as it extends.
        float width = flameWidth * (length / maxLength);

        flameLine.startWidth = width;
        flameLine.endWidth = width * 0.7f;
    }

    private void CheckForBall(float length)
    {
        if (ballHit || length <= 0f)
            return;

        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            flameLine.endWidth * 0.5f,
            direction,
            length,
            ballLayer
        );

        if (hit.collider == null)
            return;

        PlayerLastPosition player =
            hit.collider.GetComponent<PlayerLastPosition>();

        if (player == null)
        {
            player = hit.collider
                .GetComponentInParent<PlayerLastPosition>();
        }

        if (player != null)
        {
            ballHit = true;
            player.ResetPosition();
        }
    }
}

