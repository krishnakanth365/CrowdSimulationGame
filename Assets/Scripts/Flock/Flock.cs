using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    public FlockParameters flockParameters;

    private NavMeshAgent navAgent;

    public bool isTheif = false;

    public GameObject hatObj;
    public GameObject watchObj;
    public GameObject bagObj;

    public bool hasBag;
    public bool hasWatch;
    public bool hasHat;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(GetRandomPositionOnNavMesh());
        navAgent.speed = flockParameters.speed;
        GetComponentInChildren<Animator>().SetFloat("Speed", 1);

        hatObj.SetActive(false);
        watchObj.SetActive(false);
        bagObj.SetActive(false);
        if (!isTheif)
        {
            EnableRandom();
        }
    }

    private void EnableRandom()
    {
        Randomize();
    }

    public void Randomize()
    {
        GameObject[] objects = { hatObj, bagObj, watchObj };
        bool[] hasObject = { hasHat, hasBag, hasWatch };
        
        int RandomNums = Random.Range(0, objects.Length);

        for (int i = 0; i < RandomNums; i++)
        {
            int tmp = Random.Range(0, 10);
            if (tmp > 2)
            {
                objects[i].SetActive(true);
                if (i == 0)
                {
                    hasHat = true;
                }
                if (i == 1)
                {
                    hasBag = true;
                }
                if (i == 2)
                {
                    hasWatch = true;
                }
            }
        }
    }

    private void Update()
    {
        if(navAgent.remainingDistance <= 0.1f)
        {
            navAgent.SetDestination(GetRandomPositionOnNavMesh());
        }
    }

    public void SetAsTheif()
    {
        gameObject.AddComponent<Theif>();
        GetComponent<Theif>().AssignReferences(watchObj, hatObj, bagObj);
        isTheif = true;
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
