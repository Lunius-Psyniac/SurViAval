using UnityEngine;

/// <summary>
/// Animates a material and scale of an object to create a gentle pulsing effect.
/// Uses Lerp for smooth interpolation.
/// </summary>
public class ZoneVisuals : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("The speed of the pulsing animation.")]
    public float pulseSpeed = 1f;
    [Tooltip("The minimum scale during the pulse.")]
    public float minScale = 0.95f;
    [Tooltip("The maximum scale during the pulse.")]
    public float maxScale = 1.05f;
    [Tooltip("The minimum alpha (transparency) during the pulse.")]
    public float minAlpha = 0.5f;
    [Tooltip("The maximum alpha (transparency) during the pulse.")]
    public float maxAlpha = 1f;

    private Material materialInstance;
    private Vector3 initialScale;

    void Start()
    {
        // We create an instance of the material so that changing its color
        // doesn't affect every other object using the same material.
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materialInstance = renderer.material;
        }

        initialScale = transform.localScale;
    }

    void Update()
    {
        // Mathf.PingPong creates a value that goes from 0 to 1 and back to 0.
        // This is perfect for a looping animation.
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);

        // Interpolate scale based on the ping-pong value.
        transform.localScale = initialScale * Mathf.Lerp(minScale, maxScale, t);

        // Interpolate alpha (transparency) based on the ping-pong value.
        if (materialInstance != null)
        {
            // We must use SetColor with the specific property name for particle shaders.
            Color newColor = materialInstance.GetColor("_TintColor");
            newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            materialInstance.SetColor("_TintColor", newColor);
        }
    }
} 