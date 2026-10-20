using System.Collections;
using System.Collections.Generic;
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
        GameOver
    }

    public GameState currentState;  // Stores current state of the game
    public GameState previousState; // // Stores previous state of the game

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;

    // Current stat displays
    public Text currentHealthDisplay;
    public Text currentMaxHealthDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    // Flags related to game state
    public bool isGameOver = false;
    public bool isPaused = false;
    

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
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }
}
