using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 initialPosition;
    private bool isMoving = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude > 0)
        {
            navMeshAgent.Move(movement * navMeshAgent.speed * Time.deltaTime);
            isMoving = true;

            transform.LookAt(transform.position + movement);
        }
        else
        {
            isMoving = false;
        }

        animator.SetBool("Run", isMoving);

        if (Input.GetKeyDown(KeyCode.F))
        {
            navMeshAgent.Warp(initialPosition);
        }
    }
}
