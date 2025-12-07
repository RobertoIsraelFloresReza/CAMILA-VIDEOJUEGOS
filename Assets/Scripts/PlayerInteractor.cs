using DefaultNamespace;
using TMPro;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{[Header("Configuración del Raycast")]
     [Tooltip("Distancia máxima para interactuar.")]
     public float interactionDistance = 3f; 
     
     [Tooltip("La capa de los objetos interactuables (llaves, puertas, etc.).")]
     public LayerMask interactableLayer;
     
     [Header("Configuración de UI")]
     private TextMeshProUGUI interactionText;
     private GameObject promptObject;
 
     private Camera playerCamera; 
     
     [Header("Configuración de Interacción")]
     [SerializeField] private Transform raycastOriginFixed;
     void Start()
     {
         playerCamera = Camera.main; 
         promptObject = GameObject.Find("MensajeInteraccion"); 

         if (promptObject != null)
         {
             interactionText = promptObject.GetComponent<TextMeshProUGUI>();

              promptObject.SetActive(false); 
         }
      }
 
     void LateUpdate()
     {
         if (playerCamera == null || promptObject == null || interactionText == null) 
         return;

         bool hitInteractable = false; 
         
         Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
         RaycastHit hit;
         
         
        // Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red, 1f);
         
         if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
         {
            // Debug.Log("Raycast golpeó algo en la capa correcta: " + hit.collider.name);
             IInteractable interactable = hit.collider.GetComponent<IInteractable>();

             if (interactable == null && hit.collider.transform.parent != null)
             {
                 interactable = hit.collider.transform.parent.GetComponent<IInteractable>();
             }

             if (interactable != null)
             {
                // Debug.Log("¡Objeto interactuable encontrado! Activando UI.");
                 hitInteractable = true;
                
                 if (!promptObject.activeSelf)
                 {
                     promptObject.SetActive(true);
                    
                     string objectName = hit.collider.name;
                     if (hit.collider.transform.parent != null)
                     {
                         objectName = hit.collider.transform.parent.name;
                     }
                    
                     interactionText.text = $"Presiona [ E ] para interactuar con {objectName}";
                 }

                 if (Input.GetKeyDown(KeyCode.E))
                 {
                     interactable.Interact();
                     promptObject.SetActive(false);
                 }
             }
         }
        
         if (!hitInteractable && promptObject.activeSelf)
         {
             promptObject.SetActive(false);
         }
     }
}
