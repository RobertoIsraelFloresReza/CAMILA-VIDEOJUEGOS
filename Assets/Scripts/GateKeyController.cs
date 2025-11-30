using UnityEngine;

public class GateKeyController : MonoBehaviour
{
    [Header("Evento Asociado")]
    public GlobalEvents keyOpensEvent;
    
    [SerializeField] private Transform keyModel;
     public string interactionTag = "Player";
    
    private void OnMouseDown()
    {// Notifica el evento específico (ej: Gate1On)
        EventManager.Invoke(keyOpensEvent); 
        
        // Desaparece la llave
        if (keyModel != null)
        {
            keyModel.gameObject.SetActive(false);
        }
    }
}
