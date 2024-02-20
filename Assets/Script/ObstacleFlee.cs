using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFlee : MonoBehaviour
{
    public string obstacleTag = "Obstacle";
    public float avoidanceRadius = 2f;
    public float fleeForce = 5f;

    // Esta funci�n se llama cuando otro objeto entra en el �rea de detecci�n jeje

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(obstacleTag))
        {
            Vector3 fleeDirection = transform.position - other.transform.position;

            fleeDirection.Normalize();
            GetComponent<Rigidbody>().AddForce(fleeDirection, ForceMode.Acceleration);
        }
    }
}
