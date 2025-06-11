using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    // Make the ResourceType enum from ResourceZone available here too.
    public enum ResourceType { Food, Toilet, Sleep, Grades }
    
    [System.Serializable]
    public class Resource
    {
        public string name;
        public float currentValue;
        public float maxValue = 100f;
        public float decayRate = 1f; // How fast the resource decreases over time
        public float minValue = 0f;
        public bool isDecaying = true; // New flag to control decay
        
        public UnityEvent onResourceEmpty;
        public UnityEvent onResourceFull;
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
        DecreaseResource(sleep);
        DecreaseResource(grades);
    }

    void DecreaseResource(Resource resource)
    {
        // Only decrease the resource if its decay is not paused.
        if (!resource.isDecaying)
        {
            return;
        }

        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - resource.decayRate);
        
        if (resource.currentValue <= resource.minValue)
        {
            resource.onResourceEmpty?.Invoke();
        }
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
        
        if (resource.currentValue >= resource.maxValue)
        {
            resource.onResourceFull?.Invoke();
        }
        
        if (resource.currentValue <= resource.minValue)
        {
            resource.onResourceEmpty?.Invoke();
        }
    }

    // Public methods to modify resources
    public void SubtractFromResource(Resource resource, float amount)
    {
        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - amount);
        
        if (resource.currentValue <= resource.minValue)
        {
            resource.onResourceEmpty?.Invoke();
        }
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