using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewVisionCode : MonoBehaviour
{
    // Variables declaradas
    public Transform Object;
    public Transform Guard;
    public float VisionA = 45f; // �ngulo de visi�n del guardia
    public float VisionD = 10f; // Distancia de visi�n del guardia
    public float movementSpeed = 0.3f; // Velocidad de movimiento del guardia

    public Vector3 initialPos; // Posici�n inicial del guardia
    public float pursuitDuration = 5f; // Duraci�n de la persecuci�n

    private bool DetectedTrue = false;
    private Vector3 detectedPosition;
    private float pursuitTimer = 0f; // Temporizador de persecuci�n
    private bool isPursuing = false;

    void Start()
    {
        initialPos = Guard.position;
    }

    // M�todo para encontrar objetivos dentro del campo de visi�n

    private void FindVisibleTargets()
    {
        DetectedTrue = false; // Reinicia la detecci�n


        Vector3 directionToTarget = Object.position - transform.position;  // Vector hacia el objetivo

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); //Calculo del angulo del vector cono a objetivo

        if (angleToTarget < VisionA / 2 && directionToTarget.magnitude < VisionD) // Angulo dentro del angulo del cono y obj a distancia de vision
        {
            DetectedTrue = true; // El objetivo est� detectado
            detectedPosition = Object.position;
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar el cono :D
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        if (DetectedTrue)
        {
            Gizmos.color = Color.red; // Rojo si detectado
            DrawGizmo(transform.position, VisionA, VisionD, 2.0f); // Aumentar el ancho del cono a 2.0f
        }
        else
        {
            Gizmos.color = Color.green; // Verde si no detectado
            DrawGizmo(transform.position, VisionA, VisionD, 1.0f);
        }
    }

    private void Update()
    {
        FindVisibleTargets(); // Detectar objetos en el cono de visi�n

        if (DetectedTrue && !isPursuing)
        {
            StartPursuit(); // Inicia la persecuci�n
        }
        else if (isPursuing) // Si el guardia est� persiguiendo
        {
            UpdatePursuit();  // Actualiza la persecuci�n
        }
    }

    // M�todo para iniciar la persecuci�n

    private void StartPursuit()
    {
        isPursuing = true; // Marca que el guardia est� persiguiendo
        pursuitTimer = pursuitDuration;
    }

    // M�todo para actualizar la persecuci�n

    private void UpdatePursuit()
    {
        // Si el tiempo de persecuci�n es mayor que cero

        if (pursuitTimer > 0)
        {
            Vector3 direction = detectedPosition - transform.position;
            direction.y = 0f;
            transform.position += direction.normalized * movementSpeed * Time.deltaTime;
            pursuitTimer -= Time.deltaTime;
        }
        else
        {
            isPursuing = false; // Marca que la persecuci�n ha terminado
            transform.position = initialPos;
        }
    }

    // M�todo para manejar colisiones con el infiltrador

    private void OnCollisionEnter(Collision collision)
    {
        // Si el guardia est� persiguiendo al infiltrador y se produce una colisi�n con �l

        if (isPursuing && collision.gameObject.CompareTag("Infiltrator"))
        {
            Destroy(collision.gameObject);
            isPursuing = false;
            transform.position = initialPos;
        }
    }

    // M�todo para dibujar los gizmos originales con un ancho personalizado
    private void DrawGizmo(Vector3 position, float angle, float distance, float width)
    {
        Vector3 rightDirection = Quaternion.Euler(0, angle / 2, 0) * transform.forward * distance;
        Vector3 leftDirection = Quaternion.Euler(0, -angle / 2, 0) * transform.forward * distance;
        Vector3 rightPerpendicular = Quaternion.Euler(0, 90, 0) * rightDirection.normalized * width;
        Vector3 leftPerpendicular = Quaternion.Euler(0, -90, 0) * leftDirection.normalized * width;

        // Dibuja l�neas que representan los l�mites del cono de visi�n

        Gizmos.DrawLine(position, position + rightDirection + rightPerpendicular);
        Gizmos.DrawLine(position, position + leftDirection + leftPerpendicular);
    }
}