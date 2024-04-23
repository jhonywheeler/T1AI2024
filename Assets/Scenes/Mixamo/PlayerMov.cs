using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento del personaje
    private Animator animator; // Referencia al componente Animator

    void Start()
    {
        animator = GetComponent<Animator>(); // Obtener el componente Animator del GameObject
    }

    void Update()
    {
        // Obtener la entrada del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calcular la dirección del movimiento
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed * Time.deltaTime;

        // Mover el personaje
        transform.Translate(movement);

        // Verificar si hay entrada de movimiento y reproducir la animación de caminar
        if (movement.magnitude > 0)
        {
            animator.SetBool("isWalking", true); // Establecer el parámetro "isWalking" en true
        }
        else
        {
            animator.SetBool("isWalking", false); // Establecer el parámetro "isWalking" en false
        }
    }
}
