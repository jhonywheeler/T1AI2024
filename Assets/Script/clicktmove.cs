using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Ref
[RequireComponent(typeof(Rigidbody))]

public class clicktmove : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClickAction; //Click del mouse accion
    [SerializeField]
    private float playerSpeed = 10f; //v player
    private float rotationSpeed = 3f; //v de rotacion

    private Camera mainCamera;
    private Coroutine coroutine;
    private Vector3 targetPosition;
    private Vector3 initialPosition;

    private CharacterController characterController;
    private Rigidbody rb;
    private int groundLayer; //capa suelo xd

    private RespawnObject respawnObject;

    private void Awake()
    {
        respawnObject = GetComponent<RespawnObject>();
        mainCamera = Camera.main;
        //characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>(); //obtiene el componente rb
        groundLayer = LayerMask.NameToLayer("Ground"); //obtiene la capa suelo (Por su nombre)
        initialPosition = transform.position; //Almacenamos la pos
    }

    private void OnEnable()
    {
        mouseClickAction.Enable(); //habilita la accion del clicl del mouse
        mouseClickAction.performed += Move; //Asigna move al evento 
         
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
    }

    //CAMBIO
    private void Move(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); //Crea un rayo desde la posicion del mouse ne la pantalla
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider && hit.collider.gameObject.layer == groundLayer) 
        {
            if (coroutine != null) StopCoroutine(coroutine); //Stop a la corutina si ya esta haciendola
            coroutine = StartCoroutine(PlayerMoveTowards(hit.point)); //Inicia para mover el player a la posicion
            targetPosition = hit.point; //Establece la position
        }
    }

    //CAMBIO
    private IEnumerator PlayerMoveTowards(Vector3 target)
    {
        while (true)
        {
            Vector3 direction = target - transform.position; //Calculo de la dir hacia el punto obj
            float distanceToTarget = direction.magnitude; ///Calcula la distancia al obj

            if (distanceToTarget <= 0.1f) // Verifica la proximidad del player al obj
            {
                rb.velocity = Vector3.zero; // Detiene el Mov del player
                transform.position = target; // Establece la posicion del jugador en el obj
                break; // Exit del loop
            }

            // Mueve el jugador hacia el objetivo
            rb.velocity = direction.normalized * playerSpeed;

            // Mantener el objeto en posición vertical, evitando la rotación hacia abajo
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            targetRotation.eulerAngles = new Vector3(0f, targetRotation.eulerAngles.y, 0f); // Mantener la rotación en el eje Y
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1);//Radio de 1 y dibuja la esfera
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
        Invoke("RespawnWithDelay", 0.1f);

    }

    private void RespawnWithDelay()
    {
        respawnObject.Respawn();
    }
}
