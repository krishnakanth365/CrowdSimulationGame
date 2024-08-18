using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


[Serializable] public class FlockParameters
{
    public float speed;

    public float neighborRadius = 3f;
    public float separationDistance = 1.5f;

    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float randomnessFactor = 0.1f; // New property to control randomness

}

public class FlockGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] npcs;
    [SerializeField] int flockSize = 32;
    [SerializeField] FlockParameters flockParameters;
    [SerializeField] GameObject flockHolder;
    [SerializeField] List<Flock> flockList = new List<Flock>();

    private void Start()
    {
        GenerateMaxFlocks();
    }

    private void GenerateMaxFlocks()
    {
        int randomNumber = Random.Range(0, flockSize);
        for (int i = 0; i < flockSize; i++)
        {
            int randomNum = Random.Range(0, npcs.Length);
            GameObject newFlock = Instantiate(npcs[randomNum]);
            Vector3 targetPositon;
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            Mesh mesh = new Mesh();
            mesh.vertices = triangles.vertices;
            mesh.triangles = triangles.indices;
            targetPositon = GetRandomPointOnMesh(mesh);
            newFlock.transform.position = targetPositon;
            newFlock.transform.parent = flockHolder.transform;
            Flock newFlockComp = newFlock.GetComponent<Flock>();
            newFlockComp.flockParameters = flockParameters;
            flockList.Add(newFlockComp);
            if (i == randomNumber)
            {
                newFlockComp.SetAsTheif();
                newFlock.gameObject.name = "THEIFF NPC";
            }
        }
    }

    public static Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        //so everything above this point wants to be factored out

        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) 
            Debug.LogError("triIndex should never be -1");

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;
    }

    public static float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }
}
