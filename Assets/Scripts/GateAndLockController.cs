using DefaultNamespace;
using UnityEngine;

public class GateAndLockController : MonoBehaviour, IInteractable
{
    [Header("Referencias de Puertas")]
    public GameObject leftDoor;  
    public GameObject rightDoor; 
    public GameObject lockObject; 

    [Header("Configuración de Animación")]
    public float openAngle = 90f; 
    public float openDuration = 1.5f; 
    private bool keyUsed = false;
    private bool isOpening = false;
    private float startTime;
    
    private Quaternion _startLeftRotation;
    private Quaternion _endLeftRotation;
    private Quaternion _startRightRotation;
    private Quaternion _endRightRotation;
    
    [Header("Evento Requerido")]
    public GlobalEvents requiredKeyEvent;
    
    [Header("Persistencia")]
    [Tooltip("ID único para esta puerta. Ej: 'Gate_Red_01'")]
    public string doorUniqueID;
    
    private void OnEnable()
    {
        EventManager.Subscribe(requiredKeyEvent, EnableLockInteraction);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(requiredKeyEvent, EnableLockInteraction);
    }
    private void EnableLockInteraction()
    {
        keyUsed = true;
        Debug.Log("Llave recogida. Candado listo para ser abierto.");
    }

    public void Interact()
    {
        if (keyUsed && !isOpening)
        {
            OpenDoors();
        }
    }
    
    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetObjectState(doorUniqueID))
        {
            Debug.Log($"[PERSISTENCIA] Puerta '{doorUniqueID}' ya abierta. Restaurando estado...");

            InitializeOpenState();
        }
        else
        {
            // Inicializamos las rotaciones iniciales si la puerta está cerrada
            _startLeftRotation = leftDoor.transform.localRotation;
            _startRightRotation = rightDoor.transform.localRotation;
        }
    }
    
    private void InitializeOpenState() // <--- NUEVA FUNCIÓN AUXILIAR
    {
        Quaternion tempStartLeft = leftDoor.transform.localRotation; 
        Quaternion tempEndLeft = tempStartLeft * Quaternion.Euler(0, openAngle, 0);
        Quaternion tempStartRight = rightDoor.transform.localRotation; 
        Quaternion tempEndRight = tempStartRight * Quaternion.Euler(0, -openAngle, 0);
        
        leftDoor.transform.localRotation = tempEndLeft;
        rightDoor.transform.localRotation = tempEndRight;

        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }
        GetComponent<Collider>().enabled = false;
        
        isOpening = false;
        keyUsed = true;
    }

    private void OpenDoors()
    {
        isOpening = true;
    
        if (lockObject != null)
        {
            lockObject.SetActive(false);
            GetComponent<Collider>().enabled = false; 
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetObjectState(doorUniqueID, true);
        }

      //  Debug.Log("Funcion OpenDoors, abriendo puertas");
        startTime = Time.time;
        
        _startLeftRotation = leftDoor.transform.localRotation; 
        _endLeftRotation = _startLeftRotation * Quaternion.Euler(0, openAngle, 0);

        _startRightRotation = rightDoor.transform.localRotation; 
        _endRightRotation = _startRightRotation * Quaternion.Euler(0, -openAngle, 0);
    }

    void Update()
    {
      //  Debug.Log("Variable is Opening: " + isOpening);
        if (isOpening)
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / openDuration); 

//            Debug.Log($"Animación en curso. Tiempo (t): {t:F2} / Duración: {openDuration}s");
    
            leftDoor.transform.localRotation = Quaternion.Slerp(_startLeftRotation, _endLeftRotation, t);
            rightDoor.transform.localRotation = Quaternion.Slerp(_startRightRotation, _endRightRotation, t);
    
            if (t >= 1.0f)
            {
                isOpening = false;
              //  Debug.Log("Puertas abiertas.");
            }
        }
    }
}
