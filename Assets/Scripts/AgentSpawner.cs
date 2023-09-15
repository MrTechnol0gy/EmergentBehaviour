using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour
{
    // This script spawns a number of agents at random locations on a Navmesh

    // The civilian prefab to spawn
    public GameObject civviePrefab;
    // The hunter prefab to spawn
    public GameObject hunterPrefab;
    // The vampire prefab to spawn
    public GameObject vampirePrefab;
    // The number of civilians to spawn
    public int numCivvies;
    // The number of hunters to spawn
    public int numHunters;
    // The number of vampires to spawn
    public int numVampires;
    // The spawn location
    private Vector3 spawnLocation;
    // The list of all agents
    private List<GameObject> agents;
    // The list of civilians
    private List<GameObject> civilians;
    // The list of hunters
    private List<GameObject> hunters;
    // The list of vampires
    private List<GameObject> vampires;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the list of agents
        agents = new List<GameObject>();
        // Initialize the list of civilians
        civilians = new List<GameObject>();
        // Initialize the list of hunters
        hunters = new List<GameObject>();
        // Initialize the list of vampires
        vampires = new List<GameObject>();
        
        // Spawn the civilians
        for (int i = 0; i < numCivvies; i++)
        {
            // Set the spawn location
            SetSpawnLocation();
            // Spawn the agent
            GameObject agent = Instantiate(civviePrefab, spawnLocation, Quaternion.identity);
            // Add the agent to the list of all agents
            agents.Add(agent);
            // Add the agent to the list of civilians
            civilians.Add(agent);
        }
        // Spawn the hunters
        for (int i = 0; i < numHunters; i++)
        {
            // Set the spawn location
            SetSpawnLocation();
            // Spawn the agent
            GameObject agent = Instantiate(hunterPrefab, spawnLocation, Quaternion.identity);
            // Add the agent to the list of all agents
            agents.Add(agent);
            // Add the agent to the list of hunters
            hunters.Add(agent);
        }
        // Spawn the vampires
        for (int i = 0; i < numVampires; i++)
        {
            // Set the spawn location
            SetSpawnLocation();
            // Spawn the agent
            GameObject agent = Instantiate(vampirePrefab, spawnLocation, Quaternion.identity);
            // Add the agent to the list of all agents
            agents.Add(agent);
            // Add the agent to the list of vampires
            vampires.Add(agent);
        }
    }
    // Set the spawn location as a random point on a Navmesh
    public void SetSpawnLocation()
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
        Vector3 point = Vector3.Lerp(v0, Vector3.Lerp(v1, v2, UnityEngine.Random.value), UnityEngine.Random.value);
        // Set the spawn location
        spawnLocation = point;
    }

    // Remove an agent prefab from the game and replace it with a vampire prefab or civilian prefab
    public void ReplaceAgent(GameObject agent)
    {
        // Get the agent's position
        Vector3 position = agent.transform.position;
        // Get the agent's rotation
        Quaternion rotation = agent.transform.rotation;
        // Remove the agent from the list of agents
        agents.Remove(agent);
        // if the agent is a civilian, remove it from the list of civilians
        if (civilians.Contains(agent))
        {
            civilians.Remove(agent);
        }
        // if the agent is a hunter, remove it from the list of hunters
        else if (hunters.Contains(agent))
        {
            hunters.Remove(agent);
        }
        // if the agent is a vampire, remove it from the list of vampires
        else if (vampires.Contains(agent))
        {
            vampires.Remove(agent);
        }
        // Destroy the agent
        Destroy(agent);
        // Spawn a vampire if the agent was a civilian or hunter
        if (agent.CompareTag("Civilian") || agent.CompareTag("Hunter"))
        {
            GameObject vampire = Instantiate(vampirePrefab, position, rotation);
            // Add the vampire to the list of agents
            agents.Add(vampire);
            // Add the vampire to the list of vampires
            vampires.Add(vampire);
        }
        else if (agent.CompareTag("Vampire"))
        {
            GameObject civilian = Instantiate(civviePrefab, position, rotation);
            // Add the civilian to the list of agents
            agents.Add(civilian);
            // Add the civilian to the list of civilians
            civilians.Add(civilian);
        }
    }
    

    // Get the list of all agents
    public List<GameObject> GetAgents()
    {
        return agents;
    }
    // Get the list of civilians
    public List<GameObject> GetCivilians()
    {
        return civilians;
    }
    // Get the list of hunters
    public List<GameObject> GetHunters()
    {
        return hunters;
    }
    // Get the list of vampires
    public List<GameObject> GetVampires()
    {
        return vampires;
    }
}
