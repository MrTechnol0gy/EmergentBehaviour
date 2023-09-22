using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterAI : MonoBehaviour
{
    // This script controls the behaviour of the hunter agents

    // The agent's Navmesh Agent
    private NavMeshAgent agent;
    // The agent's destination
    private Vector3 destination;
    // Bool to check if the agent has reached their destination
    private bool destinationReached = false;
    // reference to the agent spawner
    private AgentSpawner agentSpawner;
    // Time the state started
    public float TimeStartedState;
    public enum States
    {
        moving,
    }
    private States _currentState = States.moving;       //sets the starting enemy state    
    public States currentState 
    {
        get => _currentState;
        set {
            if (_currentState != value) 
            {
                // Calling ended state for the previous state registered.
                OnEndedState(_currentState);
                
                // Setting the new current state
                _currentState = value;
                
                // Registering here the time we're starting the state
                TimeStartedState = Time.time;
                
                // Call the started state method for the new state.
                OnStartedState(_currentState);
            }
        }
    }
    // OnStartedState is for things that should happen when a state first begins
    public void OnStartedState(States state) 
    {
        switch (state) 
        {
            case States.moving:
                //Debug.Log("I am moving.");
                // Sets the agent's color to green
                GetComponent<Renderer>().material.color = Color.green;
                // Get a new destination
                PursueVampire();
                break; 
        }
    }
    // OnUpdatedState is for things that occur during the state (main actions)
    public void OnUpdatedState(States state) 
    {
        switch (state) 
        {
            case States.moving:
                //Debug.Log("I am moving.");                
                
                // Check to see if the agent has reached their destination
                if (destinationReached)
                {
                    // Get a new destination
                    PursueVampire();
                    // Reset the destination reached bool
                    destinationReached = false;
                }
                else if (!destinationReached)
                
                    // Check to see if the agent has reached their destination
                    CheckDestinationReached();             
                break;
        }
    }

    // OnEndedState is for things that should end or change when a state ends; for cleanup
    public void OnEndedState(States state) 
    {
        switch (state) 
        {
            case States.moving:
                break;
        }
    }
    void Start()
    {
        agentSpawner = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>();
        agent = GetComponent<NavMeshAgent>();
        OnStartedState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdatedState(currentState);
    }

    // Gets the position of the closest vampire and moves there
    void PursueVampire()
    {
        // Get the closest vampire
        GameObject closestVampire = GetClosestVampire();
        // Set the destination to the vampire's position
        if (closestVampire != null)
        {
            destination = closestVampire.transform.position;
            agent.SetDestination(destination);
        }
    }       
    // Checks to see if the agent has reached their destination
    void CheckDestinationReached()
    {
        // If the agent has reached their destination
        if (agent.remainingDistance < 2f)
        {
            destinationReached = true;
        }
    }
    // Gets the closest vampire and returns it
    GameObject GetClosestVampire()
    {
        // Get the list of vampires
        List<GameObject> vampires = agentSpawner.GetVampires();
        // Set the closest vampire
        GameObject closestVampire = null;
        // Set the closest distance
        float closestDistance = Mathf.Infinity;
        // Loop through the list of vampires
        foreach (GameObject vampire in vampires)
        {
            // Get the distance between the agent and the vampire
            float distance = Vector3.Distance(transform.position, vampire.transform.position);
            // If the distance is less than the closest distance
            if (distance < closestDistance)
            {
                // Set the closest hunter
                closestVampire = vampire;
                // Set the closest distance
                closestDistance = distance;
            }
        }
        // Return the closest hunter
        return closestVampire;
    }
    // This method can be used to test if a certain time has elapsed since we registered an event time. 
    public bool TimeElapsedSince(float timeEventHappened, float testingTimeElapsed) => !(timeEventHappened + testingTimeElapsed > Time.time);

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //If we're not playing don't draw gizmos.
        if (!Application.isPlaying) return;
        
        //Setting the position for our debug label and the color.
        Vector3 debugPos = transform.position;
        debugPos.y += 2; 
        GUI.color = Color.black;
        UnityEditor.Handles.Label(debugPos,$"{currentState}"); //Draw the label.
    }
    #endif
}