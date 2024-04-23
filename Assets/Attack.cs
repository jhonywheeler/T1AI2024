using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float attackDamage = 10f; // Daño del ataque de la espada
    public CapsuleCollider swordCollider; // Referencia al Collider de la espada
    public AudioClip attackSound; // Sonido de ataque
    private AudioSource audioSource; // Referencia al AudioSource
    private Animator animator;
    private bool isAttacking = false; // Variable para controlar si el ataque está en curso

    private void Start()
    {
        swordCollider.enabled = false;
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        animator.SetBool("Atack", false);
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
            animator.SetBool("Atack", true);
        }
    }


    private void Attack()
    {

        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        swordCollider.enabled = true;
        isAttacking = true;


        Invoke("ResetAttack", 5f);
    }


    private void ResetAttack()
    {
        swordCollider.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerExit(Collider other)
    {
        
        {
            // Aplica daño al enemigo
            Debug.Log("Espada golpeó a " + other.name + " causando " + attackDamage + " de daño.");
        }
    }
}
