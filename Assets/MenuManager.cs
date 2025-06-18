using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Tooltip("The name of the main game scene to load.")]
    public string gameSceneName = "SampleScene";

    public void StartGame()
    {
        // Set the flag to tell the game manager this is a fresh start
        GameState.IsNewGame = true;
        
        // Load the main game scene
        SceneManager.LoadScene(gameSceneName);
    }

    void Start()
    {
        // Make sure the cursor is visible and unlocked in the menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
} 