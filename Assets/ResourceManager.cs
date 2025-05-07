using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    [System.Serializable]
    public class Resource
    {
        public string name;
        public float currentValue;
        public float maxValue = 100f;
        public float decayRate = 1f; // How fast the resource decreases over time
        public float minValue = 0f;
        
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
        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - resource.decayRate);
        
        if (resource.currentValue <= resource.minValue)
        {
            resource.onResourceEmpty?.Invoke();
        }
    }

    // Public methods to modify resources
    public void AddToResource(Resource resource, float amount)
    {
        resource.currentValue = Mathf.Min(resource.maxValue, resource.currentValue + amount);
        
        if (resource.currentValue >= resource.maxValue)
        {
            resource.onResourceFull?.Invoke();
        }
    }

    public void SubtractFromResource(Resource resource, float amount)
    {
        resource.currentValue = Mathf.Max(resource.minValue, resource.currentValue - amount);
        
        if (resource.currentValue <= resource.minValue)
        {
            resource.onResourceEmpty?.Invoke();
        }
    }
} 