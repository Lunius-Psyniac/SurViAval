using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("How fast the camera sways. Lower is slower.")]
    public float swaySpeed = 0.5f;

    [Header("Positional Sway")]
    [Tooltip("Enable or disable movement sway.")]
    public bool enablePositionSway = true;
    [Tooltip("How far the camera moves from its original position.")]
    public float positionSwayAmount = 0.02f;

    [Header("Rotational Sway")]
    [Tooltip("Enable or disable rotational sway.")]
    public bool enableRotationSway = true;
    [Tooltip("How much the camera rotates from its original orientation in degrees.")]
    public float rotationSwayAmount = 0.5f;

    // We assume this script is on an object that starts at a local position/rotation of zero.
    private Vector3 basePosition;
    private Quaternion baseRotation;

    // Use unique seeds for Perlin noise to get different patterns for each axis
    private float positionSeedX, positionSeedY;
    private float rotationSeedX, rotationSeedY, rotationSeedZ;

    void Start()
    {
        // Store the starting local position and rotation
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;

        // Generate random seeds so the sway isn't the same every time you start the game
        positionSeedX = Random.Range(0f, 100f);
        positionSeedY = Random.Range(0f, 100f);
        rotationSeedX = Random.Range(100f, 200f);
        rotationSeedY = Random.Range(100f, 200f);
        rotationSeedZ = Random.Range(200f, 300f);
    }

    void LateUpdate() // Use LateUpdate to apply effects after player movement
    {
        // We use a single time variable to keep all movements in sync
        float time = Time.time * swaySpeed;

        if (enablePositionSway)
        {
            // Calculate smooth, organic-looking offsets using Perlin noise.
            // We subtract 0.5 and multiply by 2 to make the noise range from -1 to 1.
            float xPosOffset = (Mathf.PerlinNoise(time, positionSeedX) - 0.5f) * 2f * positionSwayAmount;
            float yPosOffset = (Mathf.PerlinNoise(time, positionSeedY) - 0.5f) * 2f * positionSwayAmount;

            // Apply the offset to the base local position
            transform.localPosition = basePosition + new Vector3(xPosOffset, yPosOffset, 0);
        }

        if (enableRotationSway)
        {
            // Calculate smooth rotational offsets and scale them by the sway amount
            float xRot = (Mathf.PerlinNoise(time, rotationSeedX) - 0.5f) * 2f * rotationSwayAmount;
            float yRot = (Mathf.PerlinNoise(time, rotationSeedY) - 0.5f) * 2f * rotationSwayAmount;
            float zRot = (Mathf.PerlinNoise(time, rotationSeedZ) - 0.5f) * 2f * rotationSwayAmount;

            // Combine the base rotation with the new offset rotation. Halve the z-axis roll for subtlety.
            Quaternion rotationOffset = Quaternion.Euler(xRot, yRot, zRot * 0.5f);
            transform.localRotation = baseRotation * rotationOffset;
        }
    }
} 