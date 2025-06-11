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
    [Tooltip("How many study zones should be active at any given time.")]
    public int maxActiveZones = 3;
    [Tooltip("The prefab for the visual indicator (the glowing circle).")]
    public GameObject zoneVisualPrefab;

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

    void Start()
    {
        // Find all possible study zones from the parent object.
        allStudyZones = studyZoneParent.GetComponentsInChildren<StudyZone>().ToList();
        
        // Deactivate all zones initially.
        foreach (var zone in allStudyZones)
        {
            zone.gameObject.SetActive(false);
        }

        // Activate the initial set of random study zones.
        ActivateRandomZones(maxActiveZones);
        
        // Hide the quiz panel at the start.
        quizPanel.SetActive(false);
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
        
        // Freeze player movement and unlock cursor (you'll need to implement this in PlayerMovement).
        // Time.timeScale = 0; 
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
                resourceManager.AddResource(ResourceManager.ResourceType.Grades, 20f);
                Debug.Log("Correct! Grades increased.");
            }
            else
            {
                // Incorrect answer
                resourceManager.SubtractFromResource(resourceManager.grades, 10f);
                Debug.Log("Incorrect! Grades decreased.");
            }
        }
        
        // Clear the input field for next time.
        answerInputField.text = "";

        // Hide the panel and deactivate the zone they just used.
        quizPanel.SetActive(false);
        currentActiveZone.gameObject.SetActive(false);
        
        // Unfreeze player movement and lock cursor.
        // Time.timeScale = 1; 
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

        foreach (var zone in zonesToActivate)
        {
            zone.gameObject.SetActive(true);
            // If there's no visual, create one.
            if (zone.activeVisual == null)
            {
                zone.activeVisual = Instantiate(zoneVisualPrefab, zone.transform.position, zone.transform.rotation, zone.transform);
            }
        }
    }
} 