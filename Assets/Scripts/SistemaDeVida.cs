using UnityEngine;
using UnityEngine.UI; // ¡IMPORTANTE! Necesario para usar el componente Slider

public class SistemaDeVida : MonoBehaviour
{
    // Variables para la vida
    public int maxHealth = 100;
    private int currentHealth;

    // Referencia al objeto de la barra de vida
    public Slider healthBar; // AQUI ENLAZAREMOS EL SLIDER DESDE EL EDITOR
    
    void Start()
    {
        // Inicializa la vida del enemigo
        currentHealth = maxHealth;
        Debug.Log(gameObject.name + " listo. Vida inicial: " + currentHealth);
        
        // Configura la barra de vida al inicio
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Función pública llamada por la escopeta para aplicar daño
    public void TakeDamage(int damageAmount)
    {
        // Resta el daño de la vida actual
        currentHealth -= damageAmount;

        Debug.Log(gameObject.name + " ha recibido " + damageAmount + " de daño. Vida restante: " + currentHealth);
        
        // Actualiza el valor del Slider cada vez que recibe daño
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Verifica si la vida ha llegado a cero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Función para manejar la destrucción del enemigo
    void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        
        // Destruye la barra de vida y el enemigo
        if (healthBar != null)
        {
            // Opcional: Destruir el Canvas que contiene la barra de vida
            Destroy(healthBar.transform.parent.gameObject);
        }

        // Destruye el GameObject (el cubo)
        Destroy(gameObject);

        // Aquí podrías agregar efectos de explosión, sonido de muerte, etc.
    }
}