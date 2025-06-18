using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Required for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Over UI - Needs Setup")]
    [Tooltip("The parent object for the entire game over screen.")]
    public GameObject gameOverUI;
    [Tooltip("The semi-transparent image for the background overlay.")]
    public UnityEngine.UI.Image gameOverOverlay;
    [Tooltip("The text element for the game over message (e.g., 'You Passed Out').")]
    public TextMeshProUGUI gameOverText;
    [Tooltip("The button to restart the game.")]
    public UnityEngine.UI.Button restartButton;

    [Header("Main Game UI")]
    [Tooltip("The parent object for your regular in-game UI (e.g., the resource bars).")]
    public GameObject mainGameUI;

    [Header("Player Components")]
    [Tooltip("Reference to the PlayerMovement script to disable it and get its transform.")]
    public PlayerMovement playerMovement;

    public enum GameOverReason { PassedOut, Failed }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: if you have multiple scenes and want the manager to persist.
        }

        // Start with the game over UI hidden
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void TriggerGameOver(GameOverReason reason)
    {
        // Prevent triggering it multiple times
        if (playerMovement.enabled == false) return;

        playerMovement.enabled = false;

        // Unlock and show the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainGameUI != null)
        {
            // Instead of hiding the parent, hide all children except the GameOverScreen
            foreach (Transform child in mainGameUI.transform)
            {
                // We use the 'name' for comparison in case the direct reference is lost or null
                if (child.name != "GameOverScreen")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (reason == GameOverReason.PassedOut)
        {
            StartCoroutine(PassedOutSequence());
        }
        else // Failed
        {
            StartCoroutine(FailedSequence());
        }
    }

    private IEnumerator PassedOutSequence()
    {
        // 1. Simulate the fall animation via script
        if (playerMovement != null)
        {
            Transform playerTransform = playerMovement.transform;
            Quaternion startRotation = playerTransform.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(90, 0, 0); // Tilt forward 90 degrees
            float fallDuration = 1.5f;
            float elapsedTime = 0f;

            while(elapsedTime < fallDuration)
            {
                playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / fallDuration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            playerTransform.rotation = targetRotation; // Ensure it ends exactly at the target rotation
        }
        else
        {
            // If there's no player reference, just wait a moment before showing the UI
            yield return new WaitForSeconds(1.5f);
        }

        // 2. Activate the UI and then fade to red
        gameOverText.text = "You Passed Out";
        gameOverUI.SetActive(true);
        StartCoroutine(FadeOverlay(new Color(1f, 0f, 0f, 0.5f), 0.5f));
        yield return null;
    }

    private IEnumerator FailedSequence()
    {
        // No animation, just show the screen and fade to gray
        gameOverText.text = "You Failed";
        gameOverUI.SetActive(true);
        StartCoroutine(FadeOverlay(new Color(0.2f, 0.2f, 0.2f, 0.7f), 0.5f));
        yield return null; // Coroutine needs to yield at least once
    }

    private IEnumerator FadeOverlay(Color targetColor, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0); // Start fully transparent
        gameOverOverlay.color = startColor;

        while (elapsedTime < duration)
        {
            gameOverOverlay.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameOverOverlay.color = targetColor; // Ensure it ends at the exact target color
    }

    public void RestartGame()
    {
        // Reset time scale in case it was changed
        Time.timeScale = 1f;
        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 