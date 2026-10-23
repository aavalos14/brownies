
using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    public Slider brightnessSlider;
    public Image darknessOverlay;

    void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);

        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        }

        UpdateBrightness(savedBrightness);
    }

    public void UpdateBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);

        if (darknessOverlay == null)
            return;

        Color color;

        if (value < 0.5f)
        {
            color = new Color(0f, 0f, 0f, (0.5f - value) * 1.6f);
        }
        else
        {
            color = new Color(1f, 1f, 1f, (value - 0.5f) * 1.2f);
        }

        darknessOverlay.color = color;
    }
}
