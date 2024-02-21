using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour
{

    private Vector3 intialPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        intialPosition = transform.position;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = intialPosition;

    }
}
