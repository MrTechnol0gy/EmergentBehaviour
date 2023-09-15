using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VampireAI : MonoBehaviour
{
    // This script controls the behaviour of the vampire agents

    // The agent's Navmesh Agent
    private NavMeshAgent agent;
    // The agent's destination
    private Vector3 destination;
    // Bool to check if there are no more civilians
    private bool noMoreCivilians = false;
    // Bool to see if I'm turned
    private bool turned = false;
    // timer for how long it's been since I encountered a hunter
    private float timeSinceEncounter = 0;
    
    // Reference to the agent spawner script
    private AgentSpawner agentSpawner;
    // Time the state started
    public float TimeStartedState;
    public enum States
    {
        fleeing,
        pursuing,
        extinction,
        turned,
    }
    private States _currentState = States.pursuing;       //sets the starting enemy state    
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
            case States.fleeing:
                //Debug.Log("I am fleeing.");
                // Sets the agent's color to yellow
                GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case States.pursuing:
                //Debug.Log("I am pursuing.");
                // Sets the agent's color to red
                GetComponent<Renderer>().material.color = Color.red;
                CheckDanger();
                break;
            case States.extinction:
                //Debug.Log("I am extinct.");
                // Sets the agent's color to blue
                GetComponent<Renderer>().material.color = Color.blue;
                break;
            case States.turned:
                //Debug.Log("I am turned.");
                // Sets the agent's color to green
                GetComponent<Renderer>().material.color = Color.green;
                //Turn();
                break;
        }
    }
    // OnUpdatedState is for things that occur during the state (main actions)
    public void OnUpdatedState(States state) 
    {
        switch (state) 
        {
            case States.fleeing:
                //Debug.Log("I am fleeing."); 
                Flee();  
                CheckDanger();   
                CheckNoMoreCivilians();  
                // If there are no more civilians
                if (noMoreCivilians)
                {
                    // Set the state to extinction
                    currentState = States.extinction;
                }        
                break;
            case States.pursuing:
                //Debug.Log("I am pursuing."); 
                PursueCivilian();
                CheckDanger();   
                CheckNoMoreCivilians();   
                // If there are no more civilians
                if (noMoreCivilians)
                {
                    // Set the state to extinction
                    currentState = States.extinction;
                }         
                break;
            case States.extinction:
                //Debug.Log("I am extinct."); 
                // Stop the agent
                agent.isStopped = true;
                break;
            case States.turned:
                //Debug.Log("I am turned."); 
                break;
        }
    }

    // OnEndedState is for things that should end or change when a state ends; for cleanup
    public void OnEndedState(States state) 
    {
        switch (state) 
        {
            case States.fleeing:
                break;
            case States.pursuing:
                break;
            case States.extinction:
                break;
            case States.turned:
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

    // Gets the closest agent of a certain type, and if it is a civilian set the state to pursuing, if it is a hunter set the state to fleeing
    void CheckDanger()
    {
        // Get the closest agent
        GameObject closestAgent = GetClosestAgent();
        // If the closest agent is a civilian
        if (closestAgent.CompareTag("Civilian"))
        {
            // Set the state to pursuing
            currentState = States.pursuing;
        }
        // If the closest agent is a hunter
        else if (closestAgent.CompareTag("Hunter"))
        {
            // Set the state to fleeing
            currentState = States.fleeing;
        }
    }
    // pursues the closest civilian
    void PursueCivilian()
    {
        // Get the closest civilian
        GameObject closestCivilian = GetClosestAgent();
        // Set the agent's destination to the civilian's position
        agent.SetDestination(closestCivilian.transform.position);
    }

    // gets the closest hunter's direction and runs in the opposite direction
    void Flee()
    {
        // Get the closest hunter
        GameObject closestHunter = GetClosestAgent();
        // Get the direction to the hunter
        Vector3 direction = transform.position - closestHunter.transform.position;
        // Set the agent's destination to the opposite side of the hunter
        agent.SetDestination(transform.position + direction);
    }
    // Gets the closest agent
    GameObject GetClosestAgent()
    {
        // Get the closest civilian
        GameObject closestCivilian = GetClosestCivilian();
        // Get the closest hunter
        GameObject closestHunter = GetClosestHunter();
        // Get the distance to the closest civilian
        float civilianDistance = Vector3.Distance(transform.position, closestCivilian.transform.position);
        // Get the distance to the closest hunter
        float hunterDistance = Vector3.Distance(transform.position, closestHunter.transform.position);
        // If the agent is closer to the civilian than the hunter
        if (civilianDistance < hunterDistance)
        {
            // Return the closest civilian
            return closestCivilian;
        }
        // If the agent is closer to the hunter than the civilian
        else if (hunterDistance < civilianDistance)
        {
            // Return the closest hunter
            return closestHunter;
        }
        // If the agent is equidistant from the civilian and the hunter
        else
        {
            // Return the closest civilian
            return closestCivilian;
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
    // Gets the closest civilian and returns it
    GameObject GetClosestCivilian()
    {
        // Get the list of civilians
        List<GameObject> civilians = agentSpawner.GetCivilians();
        // Set the closest civilian
        GameObject closestCivilian = null;
        // Set the closest distance
        float closestDistance = Mathf.Infinity;
        // Loop through the list of civilians
        foreach (GameObject civilian in civilians)
        {
            // Get the distance between the agent and the civilian
            float distance = Vector3.Distance(transform.position, civilian.transform.position);
            // If the distance is less than the closest distance
            if (distance < closestDistance)
            {
                // Set the closest civilian
                closestCivilian = civilian;
                // Set the closest distance
                closestDistance = distance;
            }
        }
        // Return the closest civilian
        return closestCivilian;
    }
    // Checks if there are no more civilians
    void CheckNoMoreCivilians()
    {
        // Get the list of civilians
        List<GameObject> civilians = agentSpawner.GetCivilians();
        // If there are no more civilians
        if (civilians.Count == 0)
        {
            // Set noMoreCivilians to true
            noMoreCivilians = true;
        }
    }

    // if this agent is turned, replace them with a civilian
    

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