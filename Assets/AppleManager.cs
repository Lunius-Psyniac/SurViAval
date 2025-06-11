using UnityEngine;
using System.Collections; // Required for using Coroutines

/// <summary>
/// Manages the spawning and respawning of apples within a defined area.
/// </summary>
public class AppleManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("The apple prefab to be spawned.")]
    public GameObject applePrefab;
    [Tooltip("The number of apples to have in the scene initially.")]
    public int initialAppleCount = 10;
    [Tooltip("The delay in seconds before a new apple spawns after one is eaten.")]
    public float respawnDelay = 1f;
    
    [Header("Spawn Area")]
    [Tooltip("The center of the rectangular area where apples can spawn.")]
    public Vector3 spawnAreaCenter;
    [Tooltip("The size (width, height, depth) of the spawn area.")]
    public Vector3 spawnAreaSize;
    [Tooltip("The layers on which apples are allowed to spawn.")]
    public LayerMask spawnableLayers;

    // This draws a helpful green box in the Scene view to show the spawn area.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
    }
    
    void Start()
    {
        if (applePrefab == null)
        {
            Debug.LogError("Apple Prefab is not assigned in the AppleManager!", this);
            return;
        }

        // Spawn the initial set of apples.
        for (int i = 0; i < initialAppleCount; i++)
        {
            SpawnNewApple();
        }
    }

    /// <summary>
    /// Called by the player when an apple is eaten.
    /// </summary>
    public void OnAppleEaten()
    {
        // Start the coroutine to handle the respawn.
        StartCoroutine(RespawnAppleCoroutine());
    }

    /// <summary>
    /// This is a Coroutine. It waits for the specified delay, then spawns a new apple.
    /// </summary>
    private IEnumerator RespawnAppleCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnNewApple();
    }

    private void SpawnNewApple()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // If we found a valid position, spawn the apple.
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(applePrefab, spawnPosition, Quaternion.identity, this.transform); // Spawn as a child of this manager
        }
        else
        {
            Debug.LogWarning("Could not find a valid spawn position for an apple. Check your spawn area and spawnable layers.", this);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Try a few times to find a valid spot, in case we randomly pick a bad location.
        for (int i = 0; i < 10; i++)
        {
            // Calculate a random point within the spawn area's horizontal bounds.
            float spawnX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2) + spawnAreaCenter.x;
            float spawnZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2) + spawnAreaCenter.z;

            // Start the raycast from the top of the spawn volume.
            Vector3 rayStart = new Vector3(spawnX, spawnAreaCenter.y + spawnAreaSize.y / 2, spawnZ);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, spawnAreaSize.y, spawnableLayers))
            {
                // We hit something! Return the point of contact, plus a small offset so the apple sits on top.
                return hit.point + new Vector3(0, 0.5f, 0);
            }
        }
        
        // If we failed to find a position after 10 tries, return zero.
        return Vector3.zero;
    }
} 