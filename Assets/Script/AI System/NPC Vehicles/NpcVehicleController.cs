using UnityEngine;
using UnityEngine.AI;

public class NpcVehicleController : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject path;
    private Transform[] pathPoints;
    public int index = 0;

    public float minDistance = 1;

    private void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // Initialize the array to hold all path points (children of the path GameObject)
        pathPoints = new Transform[path.transform.childCount];

        // Assign each child of path as a path point
        for (int i = 0; i < pathPoints.Length; i++)
        {
            pathPoints[i] = path.transform.GetChild(i);
        }
    }

    private void Update()
    {
        // Continuously call the Roam function to move the agent
        Roam();
    }

    private void Roam()
    {
        // Check if the agent is close enough to the current path point
        if (Vector3.Distance(transform.position, pathPoints[index].position) < minDistance)
        {
            // Increment the index to the next path point, cycling back to 0 if it exceeds the number of points
            index = (index + 1) % pathPoints.Length;
        }

        // Set the destination to the current path point
        agent.SetDestination(pathPoints[index].position);
    }
}
