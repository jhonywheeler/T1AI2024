using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeOrg : MonoBehaviour
{
    public Transform agent; // El agente que tiene esta visión de cono
    public float visionAngle = 45f; // Ángulo de visión del cono
    public float visionDistance = 10f; // Distancia máxima de visión

    private bool isDetected = false; // Indica si el agente ha detectado algo

    // Método para detectar objetos dentro del cono de visión
    private void DetectObjects()
    {
        isDetected = false; // Reiniciamos la detección
        
            // Obtenemos el vector que apunta desde el agente hacia adelante
            Vector3 directionToAgent = agent.position - transform.position;

            // Calculamos el ángulo entre la dirección al agente y la dirección hacia adelante del cono
            float angleToAgent = Vector3.Angle(transform.forward, directionToAgent);

            // Si el ángulo está dentro del rango del cono y el agente está dentro de la distancia de visión
            if (angleToAgent < visionAngle / 2 && directionToAgent.magnitude < visionDistance)
            {
                // El agente ha sido detectado
                isDetected = true;
            }
    }

    // Método para dibujar el cono de visión y mostrar visualmente la detección
    private void OnDrawGizmos()
    {
        // Dibujamos el cono de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionDistance);

        // Si el agente está detectando algo, cambiamos el color del cono a rojo
        if (isDetected)
        {
            Gizmos.color = Color.red;
        }
        else // Si no está detectando nada, el color del cono es verde
        {
            Gizmos.color = Color.green;
        }

        // Dibujamos el cono de visión
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionDistance);
    }

    // Método que se llama en cada frame
    private void Update()
    {
        DetectObjects(); // Detectamos objetos en el cono de visión
    }
}