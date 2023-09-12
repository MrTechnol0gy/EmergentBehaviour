using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour
{
    // This script spawns a number of agents at random locations on a Navmesh

    // The prefab to spawn
    public GameObject agentPrefab;
    // The number of agents to spawn
    public int numAgents;
    // The spawn location
    private Vector3 spawnLocation;
    // The list of agents
    private List<GameObject> agents;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the list of agents
        agents = new List<GameObject>();
        
        // Spawn the agents with a delay between each spawn
        for (int i = 0; i < numAgents; i++)
        {
            // Set the spawn location
            SetSpawnLocation();
            // Spawn the agent
            GameObject agent = Instantiate(agentPrefab, spawnLocation, Quaternion.identity);
            // Add the agent to the list
            agents.Add(agent);
            // Wait for 0.1 seconds
            StartCoroutine(Wait(0.1f));
        }
    }

    // Wait for a certain amount of time
    IEnumerator Wait(float seconds)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(seconds);
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

    // Get the list of agents
    public List<GameObject> GetAgents()
    {
        return agents;
    }
}
