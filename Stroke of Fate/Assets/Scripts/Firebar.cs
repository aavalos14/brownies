
using System.Collections;
using UnityEngine;

public class FlameObstacle : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField] private float maxLength = 5f;
    [SerializeField] private float extendTime = 1f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float retractTime = 0.5f;
    [SerializeField] private float delayBetweenBursts = 1f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem flameParticles;

    [Header("Detection")]
    [SerializeField] private LayerMask ballLayer;

    private bool ballHit;
    private float currentLength;

    private void Start()
    {
        flameParticles.Stop();
        StartCoroutine(FlameRoutine());
    }

    private IEnumerator FlameRoutine()
    {
        while (true)
        {
            // Extend flame
            flameParticles.Play();

            float timer = 0f;

            while (timer < extendTime)
            {
                timer += Time.deltaTime;

                float progress = Mathf.Clamp01(timer / extendTime);
                currentLength = Mathf.Lerp(0f, maxLength, progress);

                UpdateFlame();
                CheckForBall();

                yield return null;
            }

            // Hold flame
            currentLength = maxLength;
            timer = 0f;

            while (timer < holdTime)
            {
                timer += Time.deltaTime;

                UpdateFlame();
                CheckForBall();

                yield return null;
            }

            // Retract flame
            timer = 0f;

            while (timer < retractTime)
            {
                timer += Time.deltaTime;

                float progress = Mathf.Clamp01(timer / retractTime);
                currentLength = Mathf.Lerp(maxLength, 0f, progress);

                UpdateFlame();
                CheckForBall();

                yield return null;
            }

            flameParticles.Stop();
            currentLength = 0f;
            ballHit = false;

            yield return new WaitForSeconds(delayBetweenBursts);
        }
    }

    private void UpdateFlame()
    {
        // Gradually change the particle emission distance.
        var shape = flameParticles.shape;
        shape.radius = 0.1f;

        var main = flameParticles.main;
        main.startSpeed = currentLength / Mathf.Max(extendTime, 0.01f);

        // Scale the particle system as the flame extends.
        flameParticles.transform.localScale =
            new Vector3(currentLength / maxLength, 1f, 1f);
    }

    private void CheckForBall()
    {
        if (ballHit || currentLength <= 0f)
            return;

        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            0.3f,
            direction,
            currentLength,
            ballLayer
        );

        if (hit.collider == null)
            return;

        PlayerLastPosition player =
            hit.collider.GetComponent<PlayerLastPosition>();

        if (player == null)
        {
            player = hit.collider.GetComponentInParent<PlayerLastPosition>();
        }

        if (player != null)
        {
            ballHit = true;
            player.ResetPosition();
        }
    }
}