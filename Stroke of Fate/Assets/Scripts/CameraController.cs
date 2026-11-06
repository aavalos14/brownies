using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 8f;

    private Camera cameraComponent;
    private GolfBallController golfBallController;
    private Vector3 startingLocalPosition;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        golfBallController = GetComponentInParent<GolfBallController>();
        startingLocalPosition = transform.localPosition;

        if (golfBallController == null)
            Debug.LogError("CameraController must be a child of a GolfBallController.", this);
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        Mouse mouse = Mouse.current;
        if (golfBallController != null &&
            !golfBallController.IsCharging &&
            mouse != null &&
            mouse.middleButton.isPressed)
        {
            float worldUnitsPerPixel = cameraComponent.orthographicSize * 2f / Screen.height;
            Vector2 mouseDelta = mouse.delta.ReadValue();
            transform.position -=
                transform.right * (mouseDelta.x * worldUnitsPerPixel) +
                transform.up * (mouseDelta.y * worldUnitsPerPixel);
            return;
        }

        float returnAmount = 1f - Mathf.Exp(-returnSpeed * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            startingLocalPosition,
            returnAmount
        );
    }
}
