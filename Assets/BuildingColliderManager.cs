using UnityEngine;

public class BuildingColliderManager : MonoBehaviour
{
    [SerializeField] private bool makeConvex = false;
    [SerializeField] private bool updateOnEnable = true;

    void Start()
    {
        AddCollidersToChildren();
    }

    void OnEnable()
    {
        if (updateOnEnable)
        {
            AddCollidersToChildren();
        }
    }

    public void AddCollidersToChildren()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            // Remove existing colliders if they're not MeshColliders
            Collider existingCollider = obj.GetComponent<Collider>();
            if (existingCollider != null && !(existingCollider is MeshCollider))
            {
                Destroy(existingCollider);
            }

            // Add MeshCollider if none exists
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = obj.AddComponent<MeshCollider>();
                meshCollider.convex = makeConvex;
            }
        }

        Debug.Log($"Colliders added to {renderers.Length} children!");
    }
} 