using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;  // Stores current state of the game
    public GameState previousState; // // Stores previous state of the game

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas; // canvas to draw floating damage text
    public float textFontSize = 20; // font size of spawned text
    public TMP_FontAsset textFont;  // font to the floating text
    public Camera referenceCamera;  // camera to convert world coordinates to screen coordinates

    [Header("UI Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject LevelUpScreen;

    [Header("Current stat displays")]
    public Text currentHealthDisplay;
    public Text currentMaxHealthDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    [Header("Stopwatch")]
    public float timeLimit; // time limit in seconds
    float stopwatchTime; // current time elapsed since the stopwatch started
    public Text stopwatchDisplay;

    // Flags related to game state
    public bool isGameOver = false;
    public bool isPaused = false;
    public bool choosingUpgrade;

    // Reference to the player's game object
    public GameObject playerObject;
    

    void Awake()
    {
        // Warning check to see if there is another singleton
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }

        // Any additional screens like a pause screen disabled by default
        DisableScreens();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                // code for gameplay state
                //Debug.Log("Game is game");
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                // code for paused state
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                // code for the game over state
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f; // Stop game entirely
                    Cursor.visible = true;
                    //Debug.Log("GAME IS OVER");
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Debug.Log("Upgrades shown");
                    LevelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    // Defines the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if(currentState != GameState.Paused)
        {
            previousState = currentState;
            currentState = GameState.Paused;
            Time.timeScale = 0f; // stops game
            isPaused = true;
            pauseScreen.SetActive(true);
            Cursor.visible = true; // puts mouse back on screen
            Debug.Log("Game is paused");
        }
    }

    public void ResumeGame()
    {
        if(currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume game
            isPaused = false;
            pauseScreen.SetActive(false);
            //Debug.Log("Removes mouse");
            Cursor.visible = false; // Removes mouse cursor from screen
            //Debug.Log("Game is resumed");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        LevelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        // Probably gonna remove this later, but it sets a time limit for gameplay
        // Could maybe use this for transitioning to next level
        if(stopwatchTime >= timeLimit)
        {
            GameOver();
        }
    }

    void UpdateStopwatchDisplay()
    {
        // Calculate number of minutes and seconds that have elapsed
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        // Update stopwatch text to display the elapsed time
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f; // resume game
        LevelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        // Start generating the floating text
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont)
        {
            tmPro.font = textFont;
        }
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        // Make sure this is destroyed after the duration finishes
        Destroy(textObj, duration);

        // Parent the generated text object to the canvas
        textObj.transform.SetParent(instance.damageTextCanvas.transform);

        // Pan the text upwards and fade it away over time
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while(t < duration)
        {
            // Wait for a frame and update the time
            yield return w;
            t += Time.deltaTime;

            // Fade the text to the right alpha value
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            // Pan the text upwards
            yOffset += speed * Time.deltaTime;
            if(target != null)
            {
                rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));
            }
        }
    }
    
    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If canvas is not set, end the function
        // so no floating text is generated
        if (!instance.damageTextCanvas && target == null)
        {
            return;
        }

        // Find a relevant camera to convert the world position to screen position
        if (!instance.referenceCamera)
        {
            instance.referenceCamera = Camera.main;
        }

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }
}
