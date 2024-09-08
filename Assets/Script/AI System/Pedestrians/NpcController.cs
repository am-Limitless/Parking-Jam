using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
}
