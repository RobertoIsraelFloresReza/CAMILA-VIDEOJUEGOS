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

    private void OpenDoors()
    {
        isOpening = true;
    
        if (lockObject != null)
        {
            lockObject.SetActive(false);
            GetComponent<Collider>().enabled = false; 
        }

        Debug.Log("Funcion OpenDoors, abriendo puertas");
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
