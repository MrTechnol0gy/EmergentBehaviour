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
    // Bool to check if the agent is near a hunter
    private bool nearHunter = false;
    // Time the state started
    public float TimeStartedState;
    public enum States
    {
        moving,
        hunting,
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
                    // Get a new destination
                    GoHere();
                    // Reset the destination reached bool
                    destinationReached = false;
                }
                else if (!destinationReached && !nearAgent && !nearHunter)
                {
                    // Check to see if the agent is near another agent
                    CheckForAgent();
                    // Check to see if the agent has reached their destination
                    CheckDestinationReached();
                }
                else if (nearHunter)
                {
                    // Set state to fleeing
                    currentState = States.fleeing;
                }
                else if (nearAgent)
                {
                    // Set state to hunting
                    currentState = States.hunting;
                }
                else 
                {
                    // Continue towards current destination
                    agent.SetDestination(destination);
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
            case States.fleeing:
                //Debug.Log("I am fleeing.");
                if (destinationReached)
                {
                    // Get a new destination
                    FleeFromHunter();
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
                    // Flee from the hunter
                    agent.SetDestination(destination);  
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
            case States.hunting:
                break;
            case States.fleeing:
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
                if (Vector3.Distance(transform.position, otherAgent.transform.position) < 10f && otherAgent.GetComponent<Renderer>().material.color == Color.red)
                {
                    nearHunter = true;
                }
                else if (Vector3.Distance(transform.position, otherAgent.transform.position) < 2f && otherAgent.GetComponent<Renderer>().material.color == Color.black)
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

    // Sets the agent's destination to a point on the navmesh farthest from the closest hunter agent
    void FleeFromHunter()
    {
        // Get the list of agents
        List<GameObject> agents = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>().GetAgents();
        // For each hunter agent
        foreach (GameObject otherAgent in agents)
        {
            if (otherAgent.GetComponent<Renderer>().material.color == Color.red)
            {
                // add each hunter agent's position to a list
                List<Vector3> hunterPositions = new List<Vector3>
                {
                    otherAgent.transform.position
                };
                // get the position of the hunter agent closest to this agent
                Vector3 closestHunterPosition = hunterPositions[0];
                foreach (Vector3 hunterPosition in hunterPositions)
                {
                    if (Vector3.Distance(transform.position, hunterPosition) < Vector3.Distance(transform.position, closestHunterPosition))
                    {
                        closestHunterPosition = hunterPosition;
                    }
                }
                // Get the other agents position
                Vector3 otherAgentPosition = closestHunterPosition;
                // Get the direction to the other agent
                Vector3 direction = transform.position - otherAgentPosition;
                // Get the opposite direction
                Vector3 oppositeDirection = -direction;
                // Get the point on the navmesh farthest from the other agent
                NavMeshHit hit;
                NavMesh.SamplePosition(oppositeDirection, out hit, 100f, NavMesh.AllAreas);
                // Set the agent's destination to the point on the navmesh farthest from the other agent
                destination = hit.position;
                // Breaks out of the foreach loop
                break;
            }
        }
    }
    // Gets a destination agent for the hunter agent
    void GetDestinationAgent()
    {
        // Find an agent whose colour is black or yellow to chase
        List<GameObject> agents = GameObject.Find("AgentSpawner").GetComponent<AgentSpawner>().GetAgents();
        foreach (GameObject otherAgent in agents)
        {
            if (otherAgent.GetComponent<Renderer>().material.color == Color.black)
            {
                // Set the agent's destination to the agent's position
                destination = otherAgent.transform.position;
                agent.SetDestination(destination);
                break;
            }
            else if (otherAgent.GetComponent<Renderer>().material.color == Color.yellow)
            {
                // Set the agent's destination to the agent's position
                destination = otherAgent.transform.position;
                agent.SetDestination(destination);
                break;
            }
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