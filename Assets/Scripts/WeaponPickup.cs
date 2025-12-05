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
        var status = GlobalEvents.WeaponAdquired;
        EventManager.Invoke(status); // notificaion de que el switch esta activo
        GameManager.Instance.NotifyItemCollected(weaponItemID);
        weaponModelVisual.gameObject.SetActive(false);
    }
}
