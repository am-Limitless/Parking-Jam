using UnityEngine;
using UnityEngine.AI;


public class NpcController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject path;
    [SerializeField] private Rigidbody[] rigidBodies;

    [Header("Path Settings")]
    [SerializeField] private float minDistance = 1f;

    private Transform[] pathPoints;
    public int currentIndex = 0;

    private const float RagdollHitForce = 125f;

    private void Start()
    {
        // Initialize components and path points
        InitializeComponents();
        InitializePathPoints();
        SetInitialDestination();
    }

    private void FixedUpdate()
    {
        Roam();
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
        //Debug.Log("Agent Speed: " + agent.speed);

        if (Vector3.Distance(transform.position, pathPoints[currentIndex].position) < minDistance)
        {
            currentIndex = (currentIndex + 1) % pathPoints.Length;
        }

        agent.SetDestination(pathPoints[currentIndex].position);
        animator.SetFloat("vertical", agent.isStopped ? 0 : 1);

        //Debug.Log("Moving towards: " + pathPoints[currentIndex].name);
        //Debug.Log("Agent isStopped: " + agent.isStopped);
        //Debug.Log("Agent remaining distance: " + agent.remainingDistance);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("PlayerCar"))
        {
            Debug.Log("Activating ragdoll due to collision with PlayerCar");
            ActivateRagdoll();
        }
    }


    private void DeactivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            Debug.Log("Deactivating ragdoll: " + rigidBody.name);
            rigidBody.isKinematic = true;
        }

        Debug.Log("Reactivating animator and agent.");
        animator.enabled = true;
        agent.enabled = true;
        agent.updatePosition = true;
    }


    private void ActivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            Debug.Log("Activating ragdoll: " + rigidBody.name);
            rigidBody.isKinematic = false;

            Vector3 forceDirection = (rigidBody.transform.position - transform.position).normalized;
            rigidBody.AddForce(forceDirection * RagdollHitForce, ForceMode.Impulse);
        }

        Debug.Log("Deactivating animator and agent.");
        animator.enabled = false;
        agent.enabled = false;
        agent.updatePosition = false;

        // Stop NavMeshAgent movement explicitly
        agent.velocity = Vector3.zero;
    }
}
