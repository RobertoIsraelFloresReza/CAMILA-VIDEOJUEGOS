using DefaultNamespace;
using UnityEngine;

public class HouseDoorContoller : MonoBehaviour, IInteractable
{
// ... (Variables de Transform, Angulo, Duration)
    [SerializeField] private Transform houseDoor;
    public float openAngle = -90f; 
    public float openDuration = 1.5f; 
    private bool keyUsed = false;
    
    private bool keyIsAvailable = false; // El nuevo switch de estado
    private bool isOpening = false;
    private float startTime;
    private Collider doorCollider;
    
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    
    [Header("Identificación Requerida")]
    public string requiredItemID;
    
    [Header("Evento Requerido")]
    public GlobalEvents requiredKeyEvent;
    
    void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }
    

    // NUEVA LÓGICA: El jugador hace clic en la puerta
    public void Interact()
    {
        // 1. Verificar el estado persistente en el GameManager
        bool keyIsAvailable = GameManager.Instance != null && 
                              GameManager.Instance.HasItem(requiredItemID);

        if (keyIsAvailable && !isOpening)
        {
            isOpening = true;
            startTime = Time.time;
            if (doorCollider != null)
                doorCollider.enabled = false;
            
            // Log de apertura
            Debug.Log($"Puerta abriéndose con llave {requiredItemID}.");

            _startRotation = houseDoor.transform.rotation;
            _endRotation = _startRotation * Quaternion.Euler(0, openAngle, 0); 
        }
        else if (!keyIsAvailable)
        {
            Debug.Log($"La puerta requiere la llave {requiredItemID}.");
        }
    }
    
    private void Update()
    {
        if (isOpening)
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / openDuration); 

            houseDoor.transform.rotation = Quaternion.Slerp(_startRotation, _endRotation, t);
        
            if (t >= 1.0f)
            {
                isOpening = false;
                Debug.Log("Puertas abiertas.");
            }
        }
    }

}
