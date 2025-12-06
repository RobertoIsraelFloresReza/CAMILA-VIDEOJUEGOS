using DefaultNamespace;
using UnityEngine;

public class GateKeyController : MonoBehaviour, IInteractable
{
    [Header("Identificación de la Llave")]
    public string keyID;
    
    [Header("Evento Asociado")]
    public GlobalEvents keyOpensEvent;
    
    [SerializeField] private Transform keyModel;
     public string interactionTag = "Player";
    
    public void Interact()
    {
        EventManager.Invoke(keyOpensEvent); 
        
        // Desaparece la llave
        if (keyModel != null)
        {
            keyModel.gameObject.SetActive(false);
        }
    }
}
