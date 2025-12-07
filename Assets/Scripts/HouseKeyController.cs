using UnityEngine;
using System;
using DefaultNamespace;

public class HouseKeyController : MonoBehaviour, IInteractable
{
    [Header("Identificación de la Llave")]
    public string keyID; 
    
    [SerializeField] private Transform keyModel;
    
    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasItem(keyID))
        {
            if (keyModel != null)
            {
                keyModel.gameObject.SetActive(false);
            }
            GetComponent<Collider>().enabled = false;
            Debug.Log($"[PERSISTENCIA] Llave '{keyID}' ya en posesión, desactivada.");
        }
    }
    
    public void Interact()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyItemCollected(keyID);
        }
        
        Debug.Log($"Llave {keyID} recogida.");

        if (keyModel != null)
        {
            keyModel.gameObject.SetActive(false);
        }
        
        if (GameManager.Instance != null)
        {
            if (keyID == "Key0")
            {
                GameManager.Instance.SetCurrentObjective("KEY_DOOR_A_ACQUIRED");
            }
            else
            {
                GameManager.Instance.SetCurrentObjective("NEW_DOOR_KEY");
            }
        }
    }
}
