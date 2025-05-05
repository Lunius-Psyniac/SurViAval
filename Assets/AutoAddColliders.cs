using System.Diagnostics;
using UnityEngine;

public class AutoAddColliders : MonoBehaviour
{
    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            if (!obj.GetComponent<Collider>())
            {
                obj.AddComponent<MeshCollider>();
            }
        }

        Debug.Log("Colliders added to all children!");
    }
}
