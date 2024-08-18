using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private List<Flock> neighbors = new List<Flock>();
    private Vector3 flockingDirection;

    private float timeSinceLastUpdate = 0f;
    public float updateInterval = 7f;

    private bool isPaused = false;

    [SerializeField] Animator myAnimator;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = flockParameters.speed;
        SetRandomDestination();

        hatObj.SetActive(false);
        watchObj.SetActive(false);
        bagObj.SetActive(false);
        if (!isTheif)
        {
            EnableRandom();
        }

        if (!myAnimator)
        {
            myAnimator = GetComponentInChildren<Animator>();
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
        if (isPaused) return; // Skip the update if paused

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval || navAgent.remainingDistance <= 0.1f)
        {
            timeSinceLastUpdate = 0f;

            FindNeighbors();
            flockingDirection = CalculateFlocking();

            // Check for NaN in flockingDirection
            if (float.IsNaN(flockingDirection.x) || float.IsNaN(flockingDirection.y) || float.IsNaN(flockingDirection.z))
            {
                flockingDirection = Vector3.zero;  // Reset to avoid errors
            }

            // Blend the flocking direction with NavMeshAgent
            Vector3 destination = transform.position + flockingDirection + Random.insideUnitSphere * flockParameters.randomnessFactor;
            SetDestinationOnNavMesh(destination);
        }
        myAnimator.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    private void FindNeighbors()
    {
        neighbors.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, flockParameters.neighborRadius);
        foreach (Collider collider in colliders)
        {
            Flock flock = collider.GetComponent<Flock>();
            if (flock != null && flock != this)
            {
                neighbors.Add(flock);
            }
        }
    }

    private Vector3 CalculateFlocking()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        foreach (Flock neighbor in neighbors)
        {
            Vector3 toNeighbor = transform.position - neighbor.transform.position;

            // Separation
            if (toNeighbor.magnitude < flockParameters.separationDistance)
            {
                separation += toNeighbor.normalized / toNeighbor.magnitude;
            }

            // Alignment
            alignment += neighbor.navAgent.velocity;

            // Cohesion
            cohesion += neighbor.transform.position;
        }

        if (neighbors.Count > 0)
        {
            alignment /= neighbors.Count;
            cohesion = (cohesion / neighbors.Count) - transform.position;

            separation *= flockParameters.separationWeight;
            alignment *= flockParameters.alignmentWeight;
            cohesion *= flockParameters.cohesionWeight * 0.5f; // Reduce cohesion's influence
        }

        Vector3 flockingDirection = separation + alignment + cohesion;

        // Check for NaN in resulting vectors
        if (float.IsNaN(flockingDirection.x) || float.IsNaN(flockingDirection.y) || float.IsNaN(flockingDirection.z))
        {
            flockingDirection = Vector3.zero;  // Reset to avoid errors
        }

        return flockingDirection;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDestination = GetRandomPositionOnNavMesh();
        navAgent.SetDestination(randomDestination);
    }

    private void SetDestinationOnNavMesh(Vector3 destination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, flockParameters.neighborRadius, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;
        Vector3 targetPosition = FlockGenerator.GetRandomPointOnMesh(mesh);
        return targetPosition;
    }

    public void SetAsTheif()
    {
        gameObject.AddComponent<Theif>();
        GetComponent<Theif>().AssignReferences(watchObj, hatObj, bagObj);
        isTheif = true;
    }

    public void PauseFlock(float duration)
    {
        StartCoroutine(PauseForSeconds(duration));
    }

    private IEnumerator PauseForSeconds(float duration)
    {
        myAnimator.SetFloat("Speed", 0f);
        isPaused = true;
        navAgent.isStopped = true;

        yield return new WaitForSeconds(duration);

        navAgent.isStopped = false;
        isPaused = false;
        myAnimator.SetFloat("Speed", navAgent.velocity.magnitude);
    }
}
