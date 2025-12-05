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
    
    [Header("Persistencia")]
    [Tooltip("ID único para esta puerta. Ej: 'BedroomDoor_Lvl1'")]
    public string doorUniqueID;
    
    [Header("Evento Requerido")]
    public GlobalEvents requiredKeyEvent;
    
    void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }
    
    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetObjectState(doorUniqueID))
        {
            houseDoor.transform.localRotation = Quaternion.Euler(0, openAngle, 0); 
            if (doorCollider != null)
                doorCollider.enabled = false;
        
            Debug.Log($"[PERSISTENCIA] Puerta '{doorUniqueID}' restaurada a estado abierto.");
        }
    }
    

    public void Interact()
    {
        bool keyIsAvailable = GameManager.Instance != null && 
                              GameManager.Instance.HasItem(requiredItemID);

        if (keyIsAvailable && !isOpening)
        {
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetObjectState(doorUniqueID, true);
            }
            
            
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
