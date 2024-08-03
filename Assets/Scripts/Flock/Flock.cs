using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Flock : MonoBehaviour
{
    public FlockParameters flockParameters;

    private NavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(GetRandomPositionOnNavMesh());
        navAgent.speed = flockParameters.speed;
        GetComponentInChildren<Animator>().SetFloat("Speed", 1);
    }

    private void Update()
    {
        if(navAgent.remainingDistance <= 0.1f)
        {
            navAgent.SetDestination(GetRandomPositionOnNavMesh());
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;
        Vector3 targetPositon = FlockGenerator.GetRandomPointOnMesh(mesh);
        return targetPositon;
    }
}
