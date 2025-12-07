using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; 

public class SistemaDeVida : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
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
    
    private bool isDead = false;

    private void Die()
    {if (isDead) return; 
        isDead = true;
        Debug.Log($"{gameObject.name} ha muerto.");
        
        FinalZoneManager fzm = FindObjectOfType<FinalZoneManager>();
        if (fzm != null)
        {
            PersistentID pid = GetComponent<PersistentID>();
            if (pid != null)
            {
                fzm.RegisterDefeat(pid.GetID()); 
            }
            else
            {
                fzm.EnemyWasDefeated(); 
            }
        }
        
        OnDeath.Invoke(); 
        
        if (!CompareTag("Player"))
        {
            EnemiesIA iaScript = GetComponent<EnemiesIA>();
            Debug.Log($"[VIDA DEBUG] Enemigo con ID {GetComponent<PersistentID>()?.GetID()} está muriendo UNA SOLA VEZ.");
             
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