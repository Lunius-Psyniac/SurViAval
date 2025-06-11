using UnityEngine;

/// <summary>
/// Handles the visual behavior of an apple, making it hover and rotate.
/// </summary>
public class Apple : MonoBehaviour
{
    [Header("Hover Animation")]
    [Tooltip("How fast the apple bobs up and down.")]
    public float hoverSpeed = 1f;
    [Tooltip("How high the apple bobs.")]
    public float hoverHeight = 0.25f;
    [Tooltip("A vertical offset from the spawn point.")]
    public float verticalOffset = 0.5f;

    [Header("Rotation")]
    [Tooltip("How fast the apple spins.")]
    public float rotationSpeed = 50f;

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position to calculate the hover from.
        startPosition = transform.position;
    }

    void Update()
    {
        // Hovering animation using a sine wave for smooth motion.
        float newY = startPosition.y + verticalOffset + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Constant rotation around the Y axis.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
} 