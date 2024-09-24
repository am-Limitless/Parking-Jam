using UnityEngine;
using UnityEngine.AI;

public class aiNpcControl : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject path;
    [SerializeField] private Rigidbody[] rigidBodies;

    [Header("Path Settings")]
    [SerializeField] private float minDistance = 14f;
    [SerializeField] private float pedestrianSpeed = 2f; // Randomized speed
    [SerializeField] private float recoveryTime = 5f; // Time to recover from ragdoll

    private Transform[] pathPoints;
    private int currentIndex = 0;
    private bool isRagdoll = false;
    private float ragdollTimer = 0f;
    private const float RagdollHitForce = 125f;

    private void Start()
    {
        InitializeComponents();
        InitializePathPoints();
        SetInitialDestination();

        // Randomize pedestrian speed for more variety
        agent.speed = pedestrianSpeed;
    }

    private void FixedUpdate()
    {
        if (!isRagdoll)
        {
            Roam();
        }
        else
        {
            ragdollTimer += Time.deltaTime;
            if (ragdollTimer >= recoveryTime)
            {
                DeactivateRagdoll();
                ragdollTimer = 0f;
            }
        }
    }

    private void InitializeComponents()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    private void InitializePathPoints()
    {
        int childCount = path.transform.childCount;
        pathPoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            pathPoints[i] = path.transform.GetChild(i);
        }
    }

    private void SetInitialDestination()
    {
        agent.SetDestination(pathPoints[currentIndex].position);
    }

    public void Roam()
    {
        if (Vector3.Distance(transform.position, pathPoints[currentIndex].position) < minDistance)
        {
            currentIndex = (currentIndex + 1) % pathPoints.Length;
        }

        agent.SetDestination(pathPoints[currentIndex].position);
        animator.SetFloat("vertical", agent.isStopped ? 0 : 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerCar") && !isRagdoll)
        {
            ActivateRagdoll();
        }
    }

    private void ActivateRagdoll()
    {
        isRagdoll = true;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
            Vector3 forceDirection = (rigidBody.transform.position - transform.position).normalized;
            rigidBody.AddForce(forceDirection * RagdollHitForce, ForceMode.Impulse);
        }

        animator.enabled = false;
        agent.enabled = false;
    }

    private void DeactivateRagdoll()
    {
        isRagdoll = false;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }

        animator.enabled = true;
        agent.enabled = true;
        SetInitialDestination();
    }
}
