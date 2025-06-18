using UnityEngine;
using TMPro; // Required for TextMeshPro elements
using System.Collections.Generic; // Required for using Lists
using System.Linq; // Required for OrderBy

public class StudyManager : MonoBehaviour
{
    [Header("System References")]
    [Tooltip("A reference to the master ResourceManager.")]
    public ResourceManager resourceManager;
    [Tooltip("The parent object holding all potential StudyZone locations.")]
    public Transform studyZoneParent;

    [Header("Quiz Settings")]
    [Tooltip("The list of all possible questions the quiz can ask.")]
    public List<Question> questionPool;
    [Tooltip("The amount of grades gained for a correct answer.")]
    public float gradesGained = 20f;
    [Tooltip("The amount of grades lost for an incorrect answer.")]
    public float gradesLost = 10f;
    [Tooltip("The number of study zones should be active at any given time.")]
    public int maxActiveZones = 3;
    [Tooltip("The prefab for the visual indicator (the glowing circle).")]
    public GameObject zoneVisualPrefab;
    [Tooltip("The scale to apply to the visual indicator to ensure it's visible.")]
    public Vector3 visualScale = new Vector3(3, 3, 3);

    [Header("UI References")]
    [Tooltip("The main UI panel for the quiz.")]
    public GameObject quizPanel;
    [Tooltip("The TextMeshPro element for the question.")]
    public TextMeshProUGUI questionText;
    [Tooltip("The InputField for the player's answer.")]
    public TMP_InputField answerInputField;

    private List<StudyZone> allStudyZones;
    private StudyZone currentActiveZone;
    private Question currentQuestion;
    private PlayerMovement playerMovement; // Cache a reference to the player

    void Start()
    {
        // --- PRE-FLIGHT CHECKS ---
        // Check if all essential references have been assigned in the Inspector.
        if (quizPanel == null || questionText == null || answerInputField == null)
        {
            Debug.LogError("STUDY MANAGER ERROR: One or more UI references are not assigned in the Inspector!", this);
            return; // Stop execution to prevent further errors.
        }
        if (zoneVisualPrefab == null)
        {
            Debug.LogError("STUDY MANAGER ERROR: The Zone Visual Prefab has not been assigned!", this);
            return;
        }
        if (studyZoneParent == null)
        {
            Debug.LogError("STUDY MANAGER ERROR: The Study Zone Parent transform has not been assigned!", this);
            return;
        }
        if (questionPool == null || questionPool.Count == 0)
        {
            Debug.LogError("STUDY MANAGER ERROR: The Question Pool is empty!", this);
            return;
        }

        // Find all possible study zones from the parent object.
        allStudyZones = studyZoneParent.GetComponentsInChildren<StudyZone>(true).ToList(); // Include inactive children
        
        // Add colliders to all study zones so the player can run into them.
        foreach (var zone in allStudyZones)
        {
            // If there's a lingering visual reference, just clear the reference.
            // This prevents the "Destroying assets is not permitted" error in Edit Mode.
            zone.activeVisual = null;
            
            // Add a warning if a zone has a zero scale, which would make it invisible.
            if (zone.transform.localScale == Vector3.zero)
            {
                Debug.LogWarning($"The StudyZone '{zone.name}' has a scale of zero. Its visuals will not be visible. Please set its scale to (1, 1, 1).", zone.gameObject);
            }
            
            if (zone.GetComponent<Collider>() == null)
            {
                SphereCollider col = zone.gameObject.AddComponent<SphereCollider>();
                col.radius = 1.5f; // A reasonable default size
                // IMPORTANT: Must NOT be a trigger for OnControllerColliderHit to work.
                col.isTrigger = false; 
            }
            zone.gameObject.SetActive(false);
        }

        // Activate the initial set of random study zones.
        ActivateRandomZones(maxActiveZones);
        
        // Hide the quiz panel at the start.
        quizPanel.SetActive(false);
        
        // Find the player in the scene to control their movement.
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void OnPlayerEnterZone(StudyZone zone)
    {
        // A player has entered a study zone.
        currentActiveZone = zone;
        
        // Pick a random question from the pool.
        currentQuestion = questionPool[Random.Range(0, questionPool.Count)];
        
        // Show the quiz panel and populate it with the question.
        questionText.text = currentQuestion.questionText;
        quizPanel.SetActive(true);
        
        // Freeze player movement and unlock cursor.
        if (playerMovement != null) playerMovement.SetMovement(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnSubmitAnswer()
    {
        // Get the player's answer from the input field.
        if (int.TryParse(answerInputField.text, out int playerAnswer))
        {
            if (playerAnswer == currentQuestion.correctAnswer)
            {
                // Correct answer
                resourceManager.AddResource(ResourceManager.ResourceType.Grades, gradesGained);
                Debug.Log("Correct! Grades increased.");
            }
            else
            {
                // Incorrect answer
                resourceManager.SubtractFromResource(resourceManager.grades, gradesLost);
                Debug.Log("Incorrect! Grades decreased.");
            }
        }
        
        // Clear the input field for next time.
        answerInputField.text = "";

        // Hide the panel and deactivate the zone they just used.
        quizPanel.SetActive(false);
        currentActiveZone.gameObject.SetActive(false);
        
        // Unfreeze player movement and lock cursor.
        if (playerMovement != null) playerMovement.SetMovement(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Activate a new random zone to replace the one that was used.
        ActivateRandomZones(1);
    }
    
    private void ActivateRandomZones(int count)
    {
        // Get a list of all currently inactive zones.
        var inactiveZones = allStudyZones.Where(z => !z.gameObject.activeSelf).ToList();
        
        // Shuffle the list and take the number we need.
        var zonesToActivate = inactiveZones.OrderBy(z => Random.value).Take(count);

        // First, ensure our prefab is valid.
        if (zoneVisualPrefab.GetComponent<ZoneVisuals>() == null)
        {
            Debug.LogError("The assigned Zone Visual Prefab does not have a ZoneVisuals script on it!", zoneVisualPrefab);
            return;
        }

        foreach (var zone in zonesToActivate)
        {
            zone.gameObject.SetActive(true);
            // If there's no visual, create one.
            if (zone.activeVisual == null)
            {
                // Instantiate the visual and parent it to the zone.
                GameObject visual = Instantiate(zoneVisualPrefab, zone.transform.position, zone.transform.rotation, zone.transform);
                
                // Force the visual's layer to Default (0) to ensure it's visible.
                // This overrides any incorrect layer settings on the parent StudyZone object.
                visual.layer = 0; 

                visual.transform.localScale = visualScale;
                zone.activeVisual = visual;

                // Add a detailed debug log to confirm creation.
                Debug.Log($"Created visual for StudyZone '{zone.name}' at position {visual.transform.position} on layer {LayerMask.LayerToName(visual.layer)}.", visual);
            }
        }
    }
} 