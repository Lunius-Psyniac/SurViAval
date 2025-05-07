using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUIManager : MonoBehaviour
{
    [Header("Resource References")]
    public ResourceManager resourceManager;

    [Header("UI Elements")]
    public Slider foodSlider;
    public Slider toiletSlider;
    public Slider sleepSlider;
    public Slider gradesSlider;

    [Header("Text Elements")]
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI toiletText;
    public TextMeshProUGUI sleepText;
    public TextMeshProUGUI gradesText;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Update sliders
        foodSlider.value = resourceManager.food.currentValue / resourceManager.food.maxValue;
        toiletSlider.value = resourceManager.toilet.currentValue / resourceManager.toilet.maxValue;
        sleepSlider.value = resourceManager.sleep.currentValue / resourceManager.sleep.maxValue;
        gradesSlider.value = resourceManager.grades.currentValue / resourceManager.grades.maxValue;

        // Update text
        foodText.text = $"Food: {resourceManager.food.currentValue:F0}%";
        toiletText.text = $"Toilet: {resourceManager.toilet.currentValue:F0}%";
        sleepText.text = $"Sleep: {resourceManager.sleep.currentValue:F0}%";
        gradesText.text = $"Grades: {resourceManager.grades.currentValue:F0}%";
    }
} 