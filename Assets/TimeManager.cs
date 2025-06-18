using UnityEngine;
using TMPro; // Required for UI text

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("The time of day the game should start at (in hours, 0-24).")]
    [Range(0f, 24f)]
    public float startTime = 8f;
    [Tooltip("How many real-world seconds it takes for one in-game minute to pass.")]
    public float secondsPerMinute = 1f;

    [Header("Lighting & Skybox")]
    [Tooltip("The main directional light in the scene that acts as the sun/moon.")]
    public Light sunLight;
    [Tooltip("The skybox material to be updated.")]
    public Material skyboxMaterial;

    [Header("UI")]
    [Tooltip("The TextMeshPro UI element for the clock display.")]
    public TextMeshProUGUI clockText;

    private float currentTimeHours;

    void Start()
    {
        // If we are coming from the main menu, reset the time.
        if (GameState.IsNewGame)
        {
            currentTimeHours = 8f; // Reset time to 8 AM
            GameState.IsNewGame = false; // Reset the flag so it doesn't run again
        }
        else
        {
            // Otherwise, use the time set in the inspector (for testing)
            currentTimeHours = startTime;
        }

        if (sunLight == null || skyboxMaterial == null)
        {
            Debug.LogError("TimeManager is missing a reference to the Sun Light or Skybox Material!", this);
        }
    }

    void Update()
    {
        // --- UPDATE TIME ---
        // Calculate the fraction of an in-game hour that has passed in this single frame.
        // This provides a smooth, continuous time progression.
        float hoursPassedThisFrame = (Time.deltaTime / secondsPerMinute) / 60f;
        currentTimeHours += hoursPassedThisFrame;

        // Loop the time back to 0 after 24 hours.
        if (currentTimeHours >= 24f)
        {
            currentTimeHours = 0f;
        }

        // --- UPDATE VISUALS ---
        // Only update the clock if it has been assigned
        if (clockText != null)
        {
            UpdateClockUI();
        }
        UpdateLighting();
    }

    private void UpdateClockUI()
    {
        if (clockText == null) return;

        // Convert float hours to hours and minutes.
        int hours = (int)currentTimeHours;
        int minutes = (int)((currentTimeHours - hours) * 60);

        // Format the string for the UI display (e.g., 08:05).
        clockText.text = $"{hours:D2}:{minutes:D2}";
    }

    private void UpdateLighting()
    {
        if (sunLight == null || skyboxMaterial == null) return;
        
        // Calculate the sun's angle based on the time of day.
        // 0 hours = -90 degrees (rising), 12 hours = 90 degrees (setting).
        float sunAngle = (currentTimeHours / 24f * 360f) - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0); // -30 on Y gives a nice angle.
        
        // Darken the ambient light and skybox at night.
        // We use an AnimationCurve for a smooth transition.
        AnimationCurve intensityCurve = new AnimationCurve(
            new Keyframe(0, 0), new Keyframe(6, 0), new Keyframe(8, 1), 
            new Keyframe(20, 1), new Keyframe(22, 0), new Keyframe(24, 0)
        );

        float lightIntensity = intensityCurve.Evaluate(currentTimeHours);
        sunLight.intensity = lightIntensity;
        RenderSettings.ambientIntensity = lightIntensity;
        skyboxMaterial.SetFloat("_Exposure", lightIntensity);
    }
    
    /// <summary>
    /// A public method to easily check if it's currently night time.
    /// </summary>
    public bool IsNight()
    {
        return currentTimeHours >= 22f || currentTimeHours < 8f;
    }
} 