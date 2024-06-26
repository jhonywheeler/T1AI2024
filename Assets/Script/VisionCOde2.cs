using UnityEngine;
using UnityEngine.AI;

public class VisioCOde2 : MonoBehaviour
{
    public float VisionA = 45f; // �ngulo de visi�n del guardia
    public float VisionD = 10f; // Distancia de visi�n del guardia
    public Transform returnDestination; // Coordenada a la que el jugador debe regresar

    private bool detectedTrue = false;
    private Transform player; // Referencia al transform del jugador
    private float detectionTime = 0f;
    private NavMeshAgent agent; // Componente NavMeshAgent del guardia
    private Animator animator; // Componente Animator del guardia

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Obtener el componente Animator
    }

    private void FindVisibleTargets()
    {
        detectedTrue = false;

        if (player != null)
        {
            Vector3 directionToTarget = player.position - transform.position;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < VisionA / 2 && directionToTarget.magnitude < VisionD)
            {
                detectedTrue = true;
            }
        }
    }

    private void Update()
    {
        FindVisibleTargets();

        if (detectedTrue)
        {
            detectionTime += Time.deltaTime;

            // Si el jugador es detectado durante al menos 3 segundos
            if (detectionTime >= 3f)
            {
                // En vez de destruir el objeto del jugador, establece su destino como la coordenada de retorno
                if (returnDestination != null)
                {
                    agent.SetDestination(returnDestination.position);
                }
            }
            else
            {
                // Mover al agente hacia la posici�n del jugador
                agent.SetDestination(player.position);

                // Activar la animaci�n "Move" en el Animator
                animator.SetBool("Runn", true);
            }
        }
        else
        {
            // Reiniciar el tiempo de detecci�n si el jugador no est� en el cono de visi�n
            detectionTime = 0f;

            // Detener la animaci�n "Move" en el Animator
            animator.SetBool("Runn", false);
        }
    }
}
