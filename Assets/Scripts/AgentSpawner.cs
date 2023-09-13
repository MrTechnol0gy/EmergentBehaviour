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
