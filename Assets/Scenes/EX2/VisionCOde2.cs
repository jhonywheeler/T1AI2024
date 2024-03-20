using UnityEngine;
using UnityEngine.AI;

public class VisioCOde2 : MonoBehaviour
{
    public float VisionA = 45f; // Ángulo de visión del guardia
    public float VisionD = 10f; // Distancia de visión del guardia

    private bool detectedTrue = false;
    private Transform player; // Referencia al transform del jugador
    private float detectionTime = 0f;
    private NavMeshAgent agent; //Pues si funciono :D 

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
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

    private void OnDrawGizmos()
    {
        // Dibujar el cono de visión
        Vector3 forward = transform.forward * VisionD;
        Quaternion leftRotation = Quaternion.Euler(0, -VisionA / 2, 0);
        Quaternion rightRotation = Quaternion.Euler(0, VisionA / 2, 0);
        Vector3 leftDirection = leftRotation * forward;
        Vector3 rightDirection = rightRotation * forward;

        Gizmos.color = detectedTrue ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection);
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
                // Destruir el objeto con el tag "Player"
                Destroy(player.gameObject);
            }
            else
            {
                // Mover al agente hacia la posición del jugador
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Reiniciar el tiempo de detección si el jugador no está en el cono de visión
            detectionTime = 0f;
        }
    }
}
