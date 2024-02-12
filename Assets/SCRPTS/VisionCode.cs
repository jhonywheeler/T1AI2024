using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCode : MonoBehaviour
{
    // Variables declaradas
    public Transform Object;

    public float VisionA = 45f;

    public float VisionD = 10f;

    private bool DetectedTrue = false;

    private void FindVisibleTargets()
    {
        DetectedTrue = false; // Reset detection


        Vector3 directionToTarget = Object.position - transform.position;  // Vector hacia el objetivo


        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); //Calculo del angulo del vector cono a objetivo

        if (angleToTarget < VisionA / 2 && directionToTarget.magnitude < VisionD) // Angulo dentro del angulo del cono y obj a distancia de vision

        {
            DetectedTrue = true; // El objetivo está detectado
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar el cono :D
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, 0.2f);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, VisionA / 2, 0) * transform.forward * VisionD);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -VisionA / 2, 0) * transform.forward * VisionD);

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * VisionD);

        Gizmos.DrawLine(transform.position + transform.forward * VisionD, transform.position + Quaternion.Euler(0, VisionA / 2, 0) * transform.forward * VisionD);

        Gizmos.DrawLine(transform.position + transform.forward * VisionD, transform.position + Quaternion.Euler(0, -VisionA / 2, 0) * transform.forward * VisionD);


        if (DetectedTrue)
        {
            Gizmos.color = Color.red; // Rojo si detectado
        }
        else
        {
            Gizmos.color = Color.green; // Verde si no detectado
        }


        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, VisionA / 2, 0) * transform.forward * VisionD);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -VisionA / 2, 0) * transform.forward * VisionD);

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * VisionD);
    }

    private void Update()
    {
        FindVisibleTargets(); // Detectar objetos en el cono de visión
    }
}
