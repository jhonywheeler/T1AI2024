/*using UnityEngine;

public class PlayerMov2 : MonoBehaviour
{
    public float MovementSpeed = 10.0f;
    public float JumpPower = 10.0f;

    private Animator animator;
    private Rigidbody _rigidbody;
    private bool isMoving = false;
    private bool canJump = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Movimiento horizontal
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude > 0)
        {
            // Rotar el jugador para que mire en la dirección del movimiento
            transform.rotation = Quaternion.LookRotation(movement);

            // Actualizar la posición del jugador
            transform.position += movement * Time.deltaTime * MovementSpeed;

            // Indicar que el jugador se está moviendo
            isMoving = true;
        }
        else
        {
            // Verificar si el jugador ha dejado de moverse por completo
            if (_rigidbody.velocity.magnitude < 0.1f)
            {
                isMoving = false;
            }
        }

        // Salto
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Aplicar fuerza hacia abajo para simular la gravedad
        _rigidbody.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);

        // Actualizar la animación
        animator.SetBool("Run", isMoving);
    }



    private void Jump()
    {
        _rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.VelocityChange);
        canJump = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Chequear si la colisión fue con el suelo
        if (collision.gameObject.CompareTag("Floor"))
        {
            canJump = true;
        }
    }
}*/
using UnityEngine;

public class PlayerMov2 : MonoBehaviour
{
    public float MovementSpeed = 10.0f;
    public float JumpPower = 10.0f;
    public float JumpCooldown = 2.0f; // Tiempo de espera entre saltos

    //private Animator animator;
    private Rigidbody _rigidbody;
    private bool isMoving = false;
    //private bool canJump = true;
    private float jumpTimer = 0.0f;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Movimiento horizontal
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude > 0)
        {
            // Rotar el jugador para que mire en la dirección del movimiento
            transform.rotation = Quaternion.LookRotation(movement);

            // Actualizar la posición del jugador
            _rigidbody.velocity = movement * MovementSpeed;

            // Indicar que el jugador se está moviendo
            isMoving = true;
        }
        else
        {
            // Detener el movimiento si no hay entrada
            _rigidbody.velocity = Vector3.zero;
            isMoving = false;
        }

        // Actualizar parámetros de animación
        //animator.SetBool("Run", isMoving);

        // Actualizar el temporizador de salto
        jumpTimer += Time.deltaTime;

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && jumpTimer >= JumpCooldown)
        {
            Jump();
            jumpTimer = 0.0f; // Reiniciar el temporizador de salto
        }
    }

    private void Jump()
    {
        //animator.SetTrigger("Jump"); // Activar la animación de salto
        _rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.VelocityChange);
        //canJump = false;
    }
}