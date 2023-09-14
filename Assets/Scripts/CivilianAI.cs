using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianAI : MonoBehaviour
{
    // This script controls the behaviour of the civilian agents

    // The agent's Navmesh Agent
    private NavMeshAgent agent;
    // The agent's destination
    private Vector3 destination;
    // Bool to check if the agent has reached their destination
    private bool destinationReached = false;
    // Reference to the agent spawner script
    private AgentSpawner agentSpawner;
    // Time the state started
    public float TimeStartedState;
    public enum States
    {
        moving,
        fleeing,
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
                // Sets the agent's color to black
                GetComponent<Renderer>().material.color = Color.black;
                // Get a new destination
                GoHere();
                break;            
            case States.fleeing:
                //Debug.Log("I am fleeing.");
                // Sets the agent's color to yellow
                GetComponent<Renderer>().material.color = Color.yellow;
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
                    // Check to see if the agent is in danger
                    CheckDanger();
                    // Get a new destination
                    GoHere();
                    // Reset the destination reached bool
                    destinationReached = false;
                }
                else if (!destinationReached)
                {
                    // Check to see if the agent is in danger
                    CheckDanger();
                    // Check to see if the agent has reached their destination
                    CheckDestinationReached();             
                }
                break;
            case States.fleeing:
                //Debug.Log("I am fleeing.");                
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
            case States.fleeing:
                break;
        }
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Get the agent spawner script
        agentSpawner = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>();
        OnStartedState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdatedState(currentState);
    }

    // Gets a random point on the Navmesh
    void GoHere()
    {
        // Get the Navmesh
        NavMeshTriangulation navMesh = NavMesh.CalculateTriangulation();
        // Get a random triangle
        int t = UnityEngine.Random.Range(0, navMesh.indices.Length - 3);
        // Get the vertices of the triangle
        Vector3 v0 = navMesh.vertices[navMesh.indices[t]];
        Vector3 v1 = navMesh.vertices[navMesh.indices[t + 1]];
        Vector3 v2 = navMesh.vertices[navMesh.indices[t + 2]];
        // Get a random point on the triangle
        destination = Vector3.Lerp(v0, Vector3.Lerp(v1, v2, UnityEngine.Random.value), UnityEngine.Random.value);
        // Set the agent's destination
        agent.SetDestination(destination);
    }     
    // Checks to see if the agent is in danger
    void CheckDanger()
    {
        // Get the closest hunter
        GameObject closestHunter = GetClosestHunter();
        // Get the closest vampire
        GameObject closestVampire = GetClosestVampire();
        // Get the distance between the agent and the closest hunter
        float hunterDistance = Vector3.Distance(transform.position, closestHunter.transform.position);
        // Get the distance between the agent and the closest vampire
        float vampireDistance = Vector3.Distance(transform.position, closestVampire.transform.position);
        // If the agent is closer to the hunter than the vampire
        if (hunterDistance < vampireDistance)
        {
            // Set the agent's destination to the opposite side of the hunter
            agent.SetDestination(transform.position + (transform.position - closestHunter.transform.position));
        }
        // If the agent is closer to the vampire than the hunter
        else if (vampireDistance < hunterDistance)
        {
            // Set the agent's destination to the opposite side of the vampire
            agent.SetDestination(transform.position + (transform.position - closestVampire.transform.position));
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
    
    // Gets the closest hunter and returns it
    GameObject GetClosestHunter()
    {
        // Get the list of hunters
        List<GameObject> hunters = agentSpawner.GetHunters();
        // Set the closest hunter
        GameObject closestHunter = null;
        // Set the closest distance
        float closestDistance = Mathf.Infinity;
        // Loop through the list of hunters
        foreach (GameObject hunter in hunters)
        {
            // Get the distance between the agent and the hunter
            float distance = Vector3.Distance(transform.position, hunter.transform.position);
            // If the distance is less than the closest distance
            if (distance < closestDistance)
            {
                // Set the closest hunter
                closestHunter = hunter;
                // Set the closest distance
                closestDistance = distance;
            }
        }
        // Return the closest hunter
        return closestHunter;
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
                // Set the closest vampire
                closestVampire = vampire;
                // Set the closest distance
                closestDistance = distance;
            }
        }
        // Return the closest vampire
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