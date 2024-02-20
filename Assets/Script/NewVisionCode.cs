using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewVisionCode : MonoBehaviour
{
    // Variables declaradas
    public Transform Object;
    public Transform Guard;
    public float VisionA = 45f; // Ángulo de visión del guardia
    public float VisionD = 10f; // Distancia de visión del guardia
    public float movementSpeed = 0.3f; // Velocidad de movimiento del guardia

    public Vector3 initialPos; // Posición inicial del guardia
    public float pursuitDuration = 5f; // Duración de la persecución

    private bool DetectedTrue = false;
    private Vector3 detectedPosition;
    private float pursuitTimer = 0f; // Temporizador de persecución
    private bool isPursuing = false;

    void Start()
    {
        initialPos = Guard.position;
    }

    // Método para encontrar objetivos dentro del campo de visión

    private void FindVisibleTargets()
    {
        DetectedTrue = false; // Reinicia la detección


        Vector3 directionToTarget = Object.position - transform.position;  // Vector hacia el objetivo

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); //Calculo del angulo del vector cono a objetivo

        if (angleToTarget < VisionA / 2 && directionToTarget.magnitude < VisionD) // Angulo dentro del angulo del cono y obj a distancia de vision
        {
            DetectedTrue = true; // El objetivo está detectado
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
        FindVisibleTargets(); // Detectar objetos en el cono de visión

        if (DetectedTrue && !isPursuing)
        {
            StartPursuit(); // Inicia la persecución
        }
        else if (isPursuing) // Si el guardia está persiguiendo
        {
            UpdatePursuit();  // Actualiza la persecución
        }
    }

    // Método para iniciar la persecución

    private void StartPursuit()
    {
        isPursuing = true; // Marca que el guardia está persiguiendo
        pursuitTimer = pursuitDuration;
    }

    // Método para actualizar la persecución

    private void UpdatePursuit()
    {
        // Si el tiempo de persecución es mayor que cero

        if (pursuitTimer > 0)
        {
            Vector3 direction = detectedPosition - transform.position;
            direction.y = 0f;
            transform.position += direction.normalized * movementSpeed * Time.deltaTime;
            pursuitTimer -= Time.deltaTime;
        }
        else
        {
            isPursuing = false; // Marca que la persecución ha terminado
            transform.position = initialPos;
        }
    }

    // Método para manejar colisiones con el infiltrador

    private void OnCollisionEnter(Collision collision)
    {
        // Si el guardia está persiguiendo al infiltrador y se produce una colisión con él

        if (isPursuing && collision.gameObject.CompareTag("Infiltrator"))
        {
            Destroy(collision.gameObject);
            isPursuing = false;
            transform.position = initialPos;
        }
    }

    // Método para dibujar los gizmos originales con un ancho personalizado
    private void DrawGizmo(Vector3 position, float angle, float distance, float width)
    {
        Vector3 rightDirection = Quaternion.Euler(0, angle / 2, 0) * transform.forward * distance;
        Vector3 leftDirection = Quaternion.Euler(0, -angle / 2, 0) * transform.forward * distance;
        Vector3 rightPerpendicular = Quaternion.Euler(0, 90, 0) * rightDirection.normalized * width;
        Vector3 leftPerpendicular = Quaternion.Euler(0, -90, 0) * leftDirection.normalized * width;

        // Dibuja líneas que representan los límites del cono de visión

        Gizmos.DrawLine(position, position + rightDirection + rightPerpendicular);
        Gizmos.DrawLine(position, position + leftDirection + leftPerpendicular);
    }
}