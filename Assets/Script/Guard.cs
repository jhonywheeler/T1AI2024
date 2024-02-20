using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private float time;

    public float rotationTime = 5f;
    public float minRotationAng = 45f;
    public float maxRotationAng = 90f;
    
    // Start is called before the first frame update
    void Start()
    {
        time = rotationTime;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f)
        {
            float randomA = Random.Range(minRotationAng, maxRotationAng);
            transform.Rotate(Vector3.up, randomA);
            time = rotationTime;
        }

    }
}
