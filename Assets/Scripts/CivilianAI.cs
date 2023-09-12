using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CivilianAI : MonoBehaviour
{
    private NavMeshAgent civilian;

    // Start is called before the first frame update
    void Start()
    {
        civilian = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    void SetRandomDestination()
    {
        // Define the random position within the NavMesh bounds
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * 10.0f;
        randomPoint += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas);

        // Set the agent's destination to the random point
        civilian.SetDestination(hit.position);
    }
}
