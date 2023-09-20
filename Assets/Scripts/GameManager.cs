using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This script controls the game
    // The agent spawner
    public AgentSpawner agentSpawner;
    // The UI manager
    public UIManager uiManager;
    // bool to check if the game is running
    public bool gameRunning;

    // Start is called before the first frame update
    void Start()
    {
        gameRunning = false; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to start the game
    public void StartGame()
    {
        // Set the game to running
        gameRunning = true;
        // Start the agent spawner
        agentSpawner.InitializeAgents();
        // Initialize the UI values
        uiManager.InitializeValues();
    }

    // Method to restart the game
    public void RestartGame()
    {
        if (!gameRunning)
        {
            return;
        }
        else
        {
            // Set the game to not running
            gameRunning = false;
            // Destroy all agents
            agentSpawner.DestroyAgents(); 
        }
    }

    // Method to check if the game is running
    public bool IsGameRunning()
    {
        return gameRunning;
    }
}
