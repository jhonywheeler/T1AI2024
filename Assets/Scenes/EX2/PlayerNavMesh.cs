using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    private Vector3 initialPosition; 

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) 
        {
         
            navMeshAgent.Warp(initialPosition);
            Debug.Log("Volviendo a la posición inicial.");
        }
        else
        {
          
            navMeshAgent.destination = movePositionTransform.position;
        }
    }
}