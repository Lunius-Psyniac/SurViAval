using UnityEngine;

/// <summary>
/// This script goes on the Apple prefab. It detects when a player enters its trigger
/// and then communicates with the necessary managers to handle being "eaten".
/// </summary>
public class AppleCollector : MonoBehaviour
{
    [Tooltip("How much the Food resource is replenished when this apple is eaten.")]
    public float foodValue = 20f;

    // This function is automatically called by Unity when another collider enters this one.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player by looking for the PlayerMovement script.
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            // Find the managers in the scene using the modern, recommended methods.
            ResourceManager resourceManager = FindFirstObjectByType<ResourceManager>();
            AppleManager appleManager = FindFirstObjectByType<AppleManager>();

            if (resourceManager != null && appleManager != null)
            {
                // Call the correct public method on the ResourceManager.
                resourceManager.AddResource(ResourceManager.ResourceType.Food, foodValue);
                
                // Tell the manager that an apple was eaten so it can respawn one.
                appleManager.OnAppleEaten();

                // Destroy this apple GameObject.
                Destroy(this.gameObject);
            }
            else
            {
                Debug.LogError("Could not find ResourceManager or AppleManager in the scene!", this);
            }
        }
    }
} 