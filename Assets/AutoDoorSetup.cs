using UnityEngine;

/// <summary>
/// Finds all GameObjects with HingeJoints in its children and automatically configures
/// their Rigidbody, HingeJoint, and BoxCollider components for consistent door behavior.
/// </summary>
public class AutoDoorSetup : MonoBehaviour
{
    [Header("Rigidbody Settings")]
    [Tooltip("The mass of the door's Rigidbody.")]
    public float doorMass = 0.02f;
    [Tooltip("The angular drag of the door's Rigidbody. This slows down rotation.")]
    public float angularDrag = 0f;

    [Header("Hinge Joint Settings")]
    [Tooltip("The local anchor point of the Hinge Joint.")]
    public Vector3 hingeAnchor = Vector3.zero;
    [Tooltip("The local axis of rotation for the Hinge Joint.")]
    public Vector3 hingeAxis = new Vector3(0, 1, 0);

    [Header("Spring Settings")]
    [Tooltip("How strongly the spring pulls the door closed.")]
    public float springForce = 2f;
    [Tooltip("How much resistance the door has to prevent endless swinging.")]
    public float springDamper = 2f;

    [Header("Collider Settings")]
    [Tooltip("An offset to apply to the BoxCollider's center, relative to the mesh's geometric center.")]
    public Vector3 colliderCenterOffset = new Vector3(0.1f, 0, 0);
    [Tooltip("How much smaller the BoxCollider should be than the visual mesh. 0.9 = 90% of the size.")]
    [Range(0.1f, 1f)]
    public float colliderSizeModifier = 0.9f;

    void Start()
    {
        // Find all HingeJoint components in this object and any of its children.
        HingeJoint[] doors = GetComponentsInChildren<HingeJoint>(true); // Use 'true' to include inactive GameObjects

        if (doors.Length == 0)
        {
            Debug.LogWarning("AutoDoorSetup: No HingeJoints were found in any children.", this.gameObject);
            return;
        }

        int configuredCount = 0;
        foreach (HingeJoint doorJoint in doors)
        {
            GameObject doorObject = doorJoint.gameObject;

            // --- Configure Rigidbody ---
            if (doorObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.mass = doorMass;
                rb.angularDamping = angularDrag;
            }

            // --- Configure HingeJoint ---
            doorJoint.anchor = hingeAnchor;
            doorJoint.axis = hingeAxis;
            doorJoint.useSpring = true;
            JointSpring newSpring = new JointSpring
            {
                spring = springForce,
                damper = springDamper,
                targetPosition = 0
            };
            doorJoint.spring = newSpring;

            // --- Configure BoxCollider ---
            if (doorObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter) && meshFilter.sharedMesh != null)
            {
                BoxCollider boxCollider = doorObject.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    boxCollider = doorObject.AddComponent<BoxCollider>();
                }

                Bounds meshBounds = meshFilter.sharedMesh.bounds;
                boxCollider.center = meshBounds.center + colliderCenterOffset;
                boxCollider.size = meshBounds.size * colliderSizeModifier;

                configuredCount++;
            }
            else
            {
                Debug.LogWarning($"AutoDoorSetup: Door '{doorObject.name}' has no MeshFilter. Cannot automatically size its BoxCollider.", doorObject);
            }
        }

        Debug.Log($"AutoDoorSetup successfully configured {configuredCount}/{doors.Length} doors.");
    }
} 