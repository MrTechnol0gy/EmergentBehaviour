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
    // Bool to check if the agent is near another agent
    private bool nearAgent = false;
    // Time the state started
    public float TimeStartedState;
    public enum States
    {
        moving,
        hunting,
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
            case States.hunting:
                //Debug.Log("I am hunting.");
                // Sets the agent's color to red
                GetComponent<Renderer>().material.color = Color.red;
                // Find an agent whose colour is black to chase
                List<GameObject> agents = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>().GetAgents();
                foreach (GameObject otherAgent in agents)
                {
                    if (otherAgent.GetComponent<Renderer>().material.color == Color.black)
                    {
                        // Set the agent's destination to the agent's position
                        destination = otherAgent.transform.position;
                        agent.SetDestination(destination);
                    }
                }
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
                    GoHere();
                    // Reset the destination reached bool
                    destinationReached = false;
                }
                else if (!destinationReached && !nearAgent)
                {
                    // Check to see if the agent is near another agent
                    CheckForAgent();
                    // Check to see if the agent has reached their destination
                    CheckDestinationReached();
                }
                else if (nearAgent)
                {
                    // Set state to hunting
                    currentState = States.hunting;
                }
                break;
            case States.hunting:
                //Debug.Log("I am hunting.");  
                if (destinationReached)
                {
                    // Get a new destination
                    GetDestinationAgent();
                    // Reset the destination reached bool
                    destinationReached = false;
                }
                else if (!destinationReached)
                {
                    // Check to see if the agent has reached their destination
                    CheckDestinationReached();
                }
                else 
                {
                    // Chase the hunted target
                    PursueAgent();  
                }
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
        agent = GetComponent<NavMeshAgent>();
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
    void CheckForAgent()
    {
        // Check to see if this agent is near another agent
        List<GameObject> agents = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>().GetAgents();
        foreach (GameObject otherAgent in agents)
        {
            if (otherAgent != gameObject)
            {
                if (Vector3.Distance(transform.position, otherAgent.transform.position) < 3f)
                {
                    nearAgent = true;
                }
            }
        }
    }
    // Sets the agent's destination to the hunted agent's position
    void PursueAgent()
    {
        agent.SetDestination(destination);
    }
    // Gets a destination agent for the hunter agent
    void GetDestinationAgent()
    {
        // Find an agent whose colour is black to chase
        List<GameObject> agents = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>().GetAgents();
        foreach (GameObject otherAgent in agents)
        {
            if (otherAgent.GetComponent<Renderer>().material.color == Color.black)
            {
                // Set the agent's destination to the agent's position
                destination = otherAgent.transform.position;
                agent.SetDestination(destination);
            }
        }
    }

    // Checks to see if the agent has reached their destination
    void CheckDestinationReached()
    {
        // If the agent has reached their destination
        if (agent.remainingDistance < 1f)
        {
            destinationReached = true;
        }
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