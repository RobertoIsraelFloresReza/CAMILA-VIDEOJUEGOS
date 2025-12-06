using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // ¡IMPORTANTE! Necesario para usar el componente Slider

public class SistemaDeVida : MonoBehaviour
{
    // Variables para la vida
    public float maxHealth = 100f;
    private float currentHealth;
    
    public UnityEvent OnDeath;

    public Slider healthBar; 
    
    private FinalZoneManager finalZoneManager;
    
    void Start()
    {
        currentHealth = maxHealth;
    ///    Debug.Log(gameObject.name + " listo. Vida inicial: " + currentHealth);
        finalZoneManager = FindObjectOfType<FinalZoneManager>();
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        Debug.Log(gameObject.name + " ha recibido " + damageAmount + " de daño. Vida restante: " + currentHealth);
        
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        
        if (finalZoneManager != null && CompareTag("Enemy"))
        {
            finalZoneManager.EnemyWasDefeated();
        }
        
        OnDeath.Invoke(); 
        
        if (!CompareTag("Player"))
        {
            EnemiesIA iaScript = GetComponent<EnemiesIA>();
             
            if (iaScript != null)
            {
                iaScript.Morir();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}