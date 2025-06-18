using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    // Make the ResourceType enum from ResourceZone available here too.
    public enum ResourceType { Food, Toilet, Sleep, Grades }
    
    [Header("System References")]
    [Tooltip("Reference to the TimeManager in the scene.")]
    public TimeManager timeManager;
    [Tooltip("Reference to the Player's movement script.")]
    public PlayerMovement playerMovement;

    [Header("Sleep Settings")]
    [Tooltip("The amount of sleep gained per second during the day while resting.")]
    public float daySleepGainRate = 1f;
    [Tooltip("The amount of sleep gained per second at night while resting.")]
    public float nightSleepGainRate = 5f;

    [System.Serializable]
    public class Resource
    {
        public string name;
        public float currentValue;
        public float maxValue = 100f;
        public float decayRate = 1f; // How fast the resource decreases over time
        public float minValue = 0f;
        public bool isDecaying = true; // New flag to control decay
    }

    [Header("Resources")]
    public Resource food;
    public Resource toilet;
    public Resource sleep;
    public Resource grades;

    [Header("Settings")]
    public float updateInterval = 1f; // How often to update resources (in seconds)
    private float timer;

    void Start()
    {
        InitializeResources();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            UpdateResources();
            timer = 0f;
        }
    }

    void InitializeResources()
    {
        // Initialize all resources to full
        food.currentValue = food.maxValue;
        toilet.currentValue = toilet.maxValue;
        sleep.currentValue = sleep.maxValue;
        grades.currentValue = grades.maxValue;
    }

    void UpdateResources()
    {
        // Decrease resources over time
        DecreaseResource(food);
        DecreaseResource(toilet);
        DecreaseResource(grades);

        // Handle Sleep logic separately because it can be gained or lost.
        HandleSleep();

        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (food.currentValue <= food.minValue || toilet.currentValue <= toilet.minValue || sleep.currentValue <= sleep.minValue)
        {
            GameManager.Instance.TriggerGameOver(GameManager.GameOverReason.PassedOut);
        }
        else if (grades.currentValue <= grades.minValue)
        {
            GameManager.Instance.TriggerGameOver(GameManager.GameOverReason.Failed);
        }
    }

    private void HandleSleep()
    {
        // If the player is standing still...
        if (playerMovement != null && !playerMovement.IsMoving)
        {
            float sleepRate = 0;
            // Check if it's night time (22:00 - 08:00)
            if (timeManager != null && timeManager.IsNight())
            {
                // At night, gain sleep at the specified rate.
                sleepRate = nightSleepGainRate;
            }
            else
            {
                // During the day, gain sleep at the specified rate.
                sleepRate = daySleepGainRate;
            }
            // Add the calculated amount to the sleep resource.
            AddToResource(sleep, sleepRate * updateInterval);
        }
        else
        {
            // If the player is moving, decrease the resource normally.
            DecreaseResource(sleep);
        }
    }

    void DecreaseResource(Resource resource)
    {
        // Only decrease the resource if its decay is not paused.
        if (!resource.isDecaying)
        {
            return;
        }

        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - resource.decayRate);
    }

    /// <summary>
    /// A public method to add to a resource using an enum type.
    /// This is cleaner to call from other scripts.
    /// </summary>
    public void AddResource(ResourceType type, float amount)
    {
        switch (type)
        {
            case ResourceType.Food:
                AddToResource(food, amount);
                break;
            case ResourceType.Toilet:
                AddToResource(toilet, amount);
                break;
            case ResourceType.Sleep:
                AddToResource(sleep, amount);
                break;
            case ResourceType.Grades:
                AddToResource(grades, amount);
                break;
        }
    }
    
    // This method is now kept private and used by the public-facing one.
    private void AddToResource(Resource resource, float amount)
    {
        resource.currentValue = Mathf.Min(resource.maxValue, resource.currentValue + amount);
    }

    // Public methods to modify resources
    public void SubtractFromResource(Resource resource, float amount)
    {
        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - amount);
    }

    /// <summary>
    /// Pauses the natural decay of a specific resource.
    /// </summary>
    public void PauseDecay(ResourceType type)
    {
        GetResource(type).isDecaying = false;
    }

    /// <summary>
    /// Resumes the natural decay of a specific resource.
    /// </summary>
    public void ResumeDecay(ResourceType type)
    {
        GetResource(type).isDecaying = true;
    }

    // A helper method to get the correct resource object from its type.
    private Resource GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food: return food;
            case ResourceType.Toilet: return toilet;
            case ResourceType.Sleep: return sleep;
            case ResourceType.Grades: return grades;
            default: return null;
        }
    }
} 