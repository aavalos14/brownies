using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Builds and controls the gameplay pause menu and its settings screen.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const float DefaultAudioSetting = 0.5f;

    private GameObject existingPauseLabel;
    private GameObject pauseOverlay;
    private GameObject pauseView;
    private GameObject settingsView;
    private Button pauseButton;
    private bool isPaused;
    private float previousTimeScale = 1f;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisible;
    private Image darknessOverlay;

    private void Start()
    {
        Transform existingLabel = transform.Find("PauseButton");
        if (existingLabel != null)
        {
            existingPauseLabel = existingLabel.gameObject;
            existingPauseLabel.SetActive(false);
        }

        Transform darknessOverlayTransform = transform.Find("DarknessOverlay");
        if (darknessOverlayTransform != null)
        {
            darknessOverlay = darknessOverlayTransform.GetComponent<Image>();
        }

        previousCursorLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        UpdateBrightnessOverlay(PlayerPrefs.GetFloat("Brightness", 0.5f));
        ApplySfxVolume(PlayerPrefs.GetFloat("SFXVolume", DefaultAudioSetting));

        BuildPauseButton();
        BuildPauseOverlay();
    }

    private void Update()
    {
        if (KeyboardEscapePressed())
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else if (settingsView.activeSelf)
            {
                ShowPauseView();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private bool KeyboardEscapePressed()
    {
        return UnityEngine.InputSystem.Keyboard.current != null &&
               UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
    }

    private void BuildPauseButton()
    {
        pauseButton = CreateButton(
            transform,
            "PauseButton_Runtime",
            "Pause",
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(28f, -26f),
            new Vector2(150f, 60f),
            new Color(0.08f, 0.18f, 0.16f, 0.94f),
            26,
            TogglePause
        );
    }

    private void BuildPauseOverlay()
    {
        pauseOverlay = CreateUIObject("PauseOverlay", transform);
        RectTransform overlayRect = pauseOverlay.GetComponent<RectTransform>();
        SetStretch(overlayRect);

        Image overlayImage = pauseOverlay.AddComponent<Image>();
        overlayImage.color = new Color(0.015f, 0.035f, 0.03f, 0.42f);
        overlayImage.raycastTarget = true;

        GameObject panel = CreateUIObject("PausePanel", pauseOverlay.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        SetCentered(panelRect, Vector2.zero, new Vector2(540f, 640f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.07f, 0.13f, 0.11f, 1f);

        pauseView = CreateUIObject("PauseView", panel.transform);
        SetStretch(pauseView.GetComponent<RectTransform>());
        settingsView = CreateUIObject("SettingsView", panel.transform);
        SetStretch(settingsView.GetComponent<RectTransform>());
        settingsView.SetActive(false);

        CreateText(pauseView.transform, "PausedTitle", "PAUSED", new Vector2(0f, 248f), new Vector2(440f, 64f), 42);
        CreateMenuButton(pauseView.transform, "Resume", new Vector2(0f, 150f), ResumeGame);
        CreateMenuButton(pauseView.transform, "Settings", new Vector2(0f, 62f), OpenSettings);
        CreateMenuButton(pauseView.transform, "Restart", new Vector2(0f, -26f), RestartToLevelOne);
        CreateMenuButton(pauseView.transform, "Quit", new Vector2(0f, -114f), QuitToMainMenu);

        BuildSettingsView();
        pauseOverlay.SetActive(false);
    }

    private void BuildSettingsView()
    {
        CreateText(settingsView.transform, "SettingsTitle", "SETTINGS", new Vector2(0f, 248f), new Vector2(440f, 64f), 38);

        CreateSettingsSlider("Brightness", 140f, PlayerPrefs.GetFloat("Brightness", 0.5f), UpdateBrightnessOverlay);

        CreateSettingsSlider("Music Volume", 40f, PlayerPrefs.GetFloat("MusicVolume", DefaultAudioSetting), value =>
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
        });

        CreateSettingsSlider("SFX Volume", -60f, PlayerPrefs.GetFloat("SFXVolume", DefaultAudioSetting), value =>
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();
            ApplySfxVolume(value);
        });

        CreateMenuButton(settingsView.transform, "Back", new Vector2(0f, -210f), ShowPauseView);
    }

    /// <summary>
    /// Applies brightness to the gameplay overlay and persists the setting.
    /// </summary>
    private void UpdateBrightnessOverlay(float value)
    {
        float brightness = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Brightness", brightness);
        PlayerPrefs.Save();

        if (darknessOverlay == null)
        {
            return;
        }

        if (brightness < 0.5f)
        {
            darknessOverlay.color = new Color(0f, 0f, 0f, (0.5f - brightness) * 1.6f);
        }
        else
        {
            darknessOverlay.color = new Color(1f, 1f, 1f, (brightness - 0.5f) * 1.2f);
        }
    }

    /// <summary>
    /// Applies the selected SFX level to all active audio sources in gameplay.
    /// </summary>
    private void ApplySfxVolume(float volume)
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>();
        float clampedVolume = Mathf.Clamp01(volume);

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = clampedVolume;
        }
    }

    private void CreateSettingsSlider(string label, float yPosition, float initialValue, UnityEngine.Events.UnityAction<float> onValueChanged)
    {
        CreateText(settingsView.transform, label + "Label", label, new Vector2(-164f, yPosition + 28f), new Vector2(340f, 34f), 22, TextAlignmentOptions.Left);

        GameObject sliderObject = CreateUIObject(label.Replace(" ", string.Empty) + "Slider", settingsView.transform);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        SetCentered(sliderRect, new Vector2(0f, yPosition - 8f), new Vector2(350f, 28f));

        Image trackImage = sliderObject.AddComponent<Image>();
        trackImage.color = new Color(0.12f, 0.2f, 0.18f, 1f);

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.direction = Slider.Direction.LeftToRight;

        GameObject fillObject = CreateUIObject("Fill", sliderObject.transform);
        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillRect.pivot = new Vector2(0f, 0.5f);
        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = new Color(0.55f, 0.76f, 0.35f, 1f);
        fillImage.raycastTarget = false;
        slider.fillRect = fillRect;

        GameObject handleObject = CreateUIObject("Handle", sliderObject.transform);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(26f, 38f);
        Image handleImage = handleObject.AddComponent<Image>();
        handleImage.color = new Color(0.88f, 0.93f, 0.78f, 1f);
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.value = slider.maxValue;
        slider.value = Mathf.Clamp01(initialValue);
        slider.onValueChanged.AddListener(onValueChanged);
    }

    private void CreateMenuButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction callback)
    {
        CreateButton(
            parent,
            label + "Button",
            label,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            position,
            new Vector2(350f, 64f),
            new Color(0.19f, 0.34f, 0.23f, 1f),
            26,
            callback
        );
    }

    private Button CreateButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 position,
        Vector2 size,
        Color backgroundColor,
        int fontSize,
        UnityEngine.Events.UnityAction callback)
    {
        GameObject buttonObject = CreateUIObject(objectName, parent);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.pivot = pivot;
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = backgroundColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.highlightedColor = Color.Lerp(backgroundColor, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(backgroundColor, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.onClick.AddListener(callback);

        GameObject labelObject = CreateUIObject("Label", buttonObject.transform);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        SetStretch(labelRect);
        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        return button;
    }

    private TMP_Text CreateText(Transform parent, string objectName, string value, Vector2 position, Vector2 size, int fontSize, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        GameObject textObject = CreateUIObject(objectName, parent);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        SetCentered(textRect, position, size);

        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = new Color(0.93f, 0.95f, 0.86f, 1f);
        text.alignment = alignment;
        text.raycastTarget = false;
        return text;
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private void SetCentered(RectTransform rectTransform, Vector2 position, Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
    }

    private void SetStretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// Opens the pause menu and stops gameplay time.
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (pauseOverlay == null)
        {
            return;
        }

        previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowPauseView();
        pauseOverlay.SetActive(true);
        pauseOverlay.transform.SetAsLastSibling();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Closes the pause menu and resumes gameplay.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
        Cursor.lockState = previousCursorLockMode;
        Cursor.visible = previousCursorVisible;
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the pause menu's settings controls.
    /// </summary>
    public void OpenSettings()
    {
        pauseView.SetActive(false);
        settingsView.SetActive(true);
    }

    /// <summary>
    /// Returns from settings to the pause menu.
    /// </summary>
    public void ShowPauseView()
    {
        if (pauseView != null)
        {
            pauseView.SetActive(true);
        }

        if (settingsView != null)
        {
            settingsView.SetActive(false);
        }
    }

    /// <summary>
    /// Restarts the gameplay scene, returning to level one.
    /// </summary>
    public void RestartToLevelOne()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Leaves gameplay and loads the main menu.
    /// </summary>
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void OnDestroy()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }

        if (existingPauseLabel != null)
        {
            existingPauseLabel.SetActive(true);
        }
    }
}
