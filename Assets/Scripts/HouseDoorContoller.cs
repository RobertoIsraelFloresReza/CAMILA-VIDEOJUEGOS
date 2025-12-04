using DefaultNamespace;
using UnityEngine;

public class HouseDoorContoller : MonoBehaviour, IInteractable
{
// ... (Variables de Transform, Angulo, Duration)
    [SerializeField] private Transform houseDoor;
    public float openAngle = -90f; 
    public float openDuration = 1.5f; 
    
    private bool keyIsAvailable = false; // El nuevo switch de estado
    private bool isOpening = false;
    private float startTime;
    private Collider doorCollider;
    
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    
    void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GlobalEvents.HouseKeyOn, KeyCollected);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(GlobalEvents.HouseKeyOn, KeyCollected);
    }

    private void KeyCollected()
    {
        keyIsAvailable = true;
        Debug.Log("Llave disponible. Esperando interacción con la puerta.");
    }

    // NUEVA LÓGICA: El jugador hace clic en la puerta
    public void Interact()
    {
        if (keyIsAvailable && !isOpening)
        {
            isOpening = true;
            startTime = Time.time;
            if (doorCollider != null)
                doorCollider.enabled = false;

            _startRotation = houseDoor.transform.rotation;
        
            _endRotation = _startRotation * Quaternion.Euler(0, openAngle, 0); 
            // =======================================================
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
