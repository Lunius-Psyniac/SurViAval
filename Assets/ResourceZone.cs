using UnityEngine;

/// <summary>
/// Defines a trigger zone that replenishes a specific player resource over time.
/// </summary>
public class ResourceZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("The type of resource this zone affects.")]
    public ResourceManager.ResourceType resourceToAffect;
    [Tooltip("The amount of resource restored per second while the player is in the zone.")]
    public float amountPerSecond = 5f;

    [Header("Visuals")]
    [Tooltip("The visual indicator prefab to display for this zone.")]
    public GameObject visualIndicatorPrefab;

    private ResourceManager playerResourceManager;
    private bool isPlayerInside = false;

    void Start()
    {
        // Add a SphereCollider to this object to act as our trigger area.
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 2f; // Default radius, can be changed in Inspector.

        // Spawn the visual indicator if one is assigned.
        if (visualIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(visualIndicatorPrefab, transform.position, transform.rotation, this.transform);
            // Match the indicator's visual size to the trigger radius.
            indicator.transform.localScale = new Vector3(trigger.radius * 2, trigger.radius * 2, 1);
        }
    }

    void Update()
    {
        // If the player is inside the zone, continuously add the resource.
        if (isPlayerInside && playerResourceManager != null)
        {
            float amountToAdd = amountPerSecond * Time.deltaTime;
            // Now this call is valid because resourceToAffect is the correct type.
            playerResourceManager.AddResource(resourceToAffect, amountToAdd);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When an object enters, check if it's the Player.
        if (other.GetComponent<PlayerMovement>() != null)
        {
            // If it is the player, find the single ResourceManager in the scene.
            playerResourceManager = FindFirstObjectByType<ResourceManager>();
            if (playerResourceManager != null)
            {
                isPlayerInside = true;
                // Pause the decay of the resource this zone affects.
                playerResourceManager.PauseDecay(resourceToAffect);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When an object exits, if it was the player, stop replenishing.
        if (other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInside = false;
            // Resume the decay of the resource when the player leaves.
            if (playerResourceManager != null)
            {
                playerResourceManager.ResumeDecay(resourceToAffect);
            }
            playerResourceManager = null;
        }
    }
} 