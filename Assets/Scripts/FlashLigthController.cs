using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLigthController : MonoBehaviour
{
    [Header("Configuración Principal")]
    public Light flashLight;
    public KeyCode toggleKey = KeyCode.F;
     
    public AudioSource toggleSound;


    [Header("Sistema de Batería")]
    public float maxBatteryLife = 100f; // Batería máxima en segundos
    public float currentBatteryLife;
    public float drainRate = 1f;

    private bool isOn = false;

    void Start()
    {
        // Empezar con la batería llena y la luz apagada
        currentBatteryLife = maxBatteryLife;
        flashLight.enabled = false;
        isOn = false;
        
    }


    void Update()
    {

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }

        // Drena la batería si la luz está encendida
        if (isOn && currentBatteryLife > 0)
        {
            currentBatteryLife -= drainRate * Time.deltaTime;


            if (currentBatteryLife <= 0)
            {
                currentBatteryLife = 0;
                TurnOff();
            }
        }

    }
    void ToggleFlashlight()
    {
        
        // Solo podemos encender si hay batería
        if (currentBatteryLife > 0)
        {
            // Invertir el estado
            isOn = !isOn;
            flashLight.enabled = isOn;

            if (toggleSound != null)
            {
                toggleSound.Play();
            }

        }
        else
        {
            // Si no hay batería, asegurarse de que esté apagada
            TurnOff();
        }
    }

    void TurnOff()
    {
        isOn = false;
        flashLight.enabled = false;
      
    }
    public void AddBattery(float amount)
    {
        currentBatteryLife += amount;
        if (currentBatteryLife > maxBatteryLife)
        {
            currentBatteryLife = maxBatteryLife;
        }
    }
}

