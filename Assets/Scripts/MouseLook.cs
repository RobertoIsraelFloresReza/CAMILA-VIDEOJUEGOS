using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo sistema

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private float xRotation = 0f; // Rotación vertical (arriba/abajo)
    private float yRotation = 0f; // Rotación horizontal (izquierda/derecha)

    void Start()
    {
        // Bloquea el cursor en el centro y lo esconde
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Revisa si hay un mouse conectado
        if (Mouse.current == null)
        {
            return;
        }

        // Obtiene el movimiento "delta" (cuánto se movió) del mouse
        float mouseX = Mouse.current.delta.ReadValue().x * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.ReadValue().y * mouseSensitivity * Time.deltaTime;

        // --- Rotación Vertical (Pitch) ---
        // Restamos mouseY para que subir el mouse mire arriba
        xRotation -= mouseY;
        // Limitamos la vista para no dar la vuelta (ej. -90 a 90 grados)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // --- Rotación Horizontal (Yaw) ---
        // Sumamos mouseX para la vista 360°
        yRotation += mouseX;

        // Aplicamos ambas rotaciones a la cámara
        // Euler(X, Y, Z)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}