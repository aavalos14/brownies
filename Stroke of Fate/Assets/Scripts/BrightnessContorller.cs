
using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    public Slider brightnessSlider;
    public Image darknessOverlay;

    void Start()
    {
        brightnessSlider.value = 0.5f;
        UpdateBrightness(0.5f);
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
    }

    public void UpdateBrightness(float value)
    {
        float darkness = Mathf.Clamp01((0.5f - value) * 1.6f);

        Color color = darknessOverlay.color;
        color.a = darkness;
        darknessOverlay.color = color;
    }
}

