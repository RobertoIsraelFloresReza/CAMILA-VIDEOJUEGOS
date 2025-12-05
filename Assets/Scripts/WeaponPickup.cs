using DefaultNamespace;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [Tooltip("ID único del arma: ej. 'Escopeta'")]
    public string weaponItemID = "Escopeta";
    
    
    [SerializeField] private GameObject weaponModelVisual;
    public string interactionTag = "Player";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public void Interact()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyItemCollected(weaponItemID);
            Debug.Log($"[PICKUP] Arma '{weaponItemID}' notificada al GameManager.");
        }
        
        // 2. Oculta el modelo visual del arma en el mundo.
        if (weaponModelVisual != null)
        {
            weaponModelVisual.gameObject.SetActive(false);
        }
    }
}
