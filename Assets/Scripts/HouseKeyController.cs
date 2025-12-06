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
    }
}
