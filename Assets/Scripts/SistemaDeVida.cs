using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // ¡IMPORTANTE! Necesario para usar el componente Slider

public class SistemaDeVida : MonoBehaviour
{
    // Variables para la vida
    public float maxHealth = 100f;
    private float currentHealth;
    
    public UnityEvent OnDeath;

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
    public void TakeDamage(float damageAmount)
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
    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        
        // Despacha el evento para que otros sistemas reaccionen (ej. animaciones, reinicio de escena).
        OnDeath.Invoke(); 
        
        // Opcional: Desactivar el componente para evitar más lógica (excepto en el jugador, donde queremos reiniciar).
        if (!CompareTag("Player")) 
        {
            // Desactiva el enemigo para que la IA deje de funcionar
            gameObject.SetActive(false); 
        }
    }
}