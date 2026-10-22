
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserTurret : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float laserLength = 15f;
    [SerializeField] private float laserWidth = 0.15f;
    [SerializeField] private LayerMask collisionLayers = ~0;

    [Header("Laser Appearance")]
    [SerializeField] private Color laserColor = Color.red;

    [Header("Optional Timing")]
    [SerializeField] private bool useTimer = false;
    [SerializeField] private float activeDuration = 3f;
    [SerializeField] private float inactiveDuration = 2f;

    private LineRenderer lineRenderer;
    private bool laserActive = true;
    private bool ballWasHit;
    private float timer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
    }

    private void Update()
    {
        if (useTimer)
        {
            timer += Time.deltaTime;

            float duration = laserActive
                ? activeDuration
                : inactiveDuration;

            if (timer >= duration)
            {
                timer = 0f;
                laserActive = !laserActive;
                ballWasHit = false;
            }
        }

        lineRenderer.enabled = laserActive;

        if (!laserActive)
            return;

        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,
            laserWidth * 0.5f,
            direction,
            laserLength,
            collisionLayers
        );

        float distance = laserLength;
        PlayerLastPosition hitBall = null;

        // Find the nearest object hit by the laser.
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform))
                continue;

            distance = hit.distance;

            hitBall = hit.collider.GetComponent<PlayerLastPosition>();

            if (hitBall == null)
            {
                hitBall = hit.collider
                    .GetComponentInParent<PlayerLastPosition>();
            }

            break;
        }

        Vector3 endPosition =
            (Vector3)(origin + direction * distance);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);

        if (hitBall != null && !ballWasHit)
        {
            ballWasHit = true;
            hitBall.ResetPosition();
        }

        // Allow another hit once the ball leaves the beam.
        if (ballWasHit && !BallStillInBeam())
        {
            ballWasHit = false;
        }
    }

    private bool BallStillInBeam()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,
            laserWidth * 0.5f,
            direction,
            laserLength,
            collisionLayers
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetComponent<PlayerLastPosition>() != null ||
                hit.collider.GetComponentInParent<PlayerLastPosition>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
