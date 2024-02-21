using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private float time;

    public float rotationTime = 5f;
    public float minRotationAng = 45f;
    public float maxRotationAng = 90f;

    private bool isPursuing = false;

    private NewVisionCode visionScript;

    // Start is called before the first frame update
    void Start()
    {
        time = rotationTime;
        visionScript = GetComponent<NewVisionCode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPursuing)
        {
            time -= Time.deltaTime;

            if (time <= 0f)
            {
                RotateGuard();

            }
        }
    }

    private void RotateGuard()
    {
        float randomA = Random.Range(minRotationAng, maxRotationAng);
        transform.Rotate(Vector3.up, randomA);
        time = rotationTime;
    }

    public void StartPursuit()
    {
        isPursuing = true;
        visionScript.UpdatePursuit();

    }

    public void EndPursuit()
    {
        isPursuing = false;
        visionScript.EndPursuit(); // Llamar al método EndPursuit de NewVisionCode
        RotateGuard(); // Reiniciar la rotación cuando termine la persecución
    }
}