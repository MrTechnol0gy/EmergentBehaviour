using UnityEngine;
using UnityEngine.AI;

public class AgentPlacement : MonoBehaviour
{
    public GameObject agentPrefab; // Reference to your agent's prefab

    void Start()
    {
        SpawnAgentOnNavMesh();
    }

    void SpawnAgentOnNavMesh()
    {
        // Find a valid position on the NavMesh
        Vector3 spawnPosition = FindValidNavMeshPosition();

        // Instantiate the agent prefab at the valid position
        GameObject agentObject = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);

        // Access the NavMeshAgent component on the agent and set its destination if needed
        NavMeshAgent agent = agentObject.GetComponent<NavMeshAgent>();        
    }

    Vector3 FindValidNavMeshPosition()
    {
        NavMeshHit hit;
        Vector3 randomPoint;

        // Repeat until a valid position on the NavMesh is found
        do
        {
            randomPoint = Random.insideUnitSphere * 10.0f;
            randomPoint += transform.position;
        }
        while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

        return hit.position;
    }
}
