using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo sistema

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private float xRotation = 0f; 
    private float yRotation = 0f; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        float mouseX = Mouse.current.delta.ReadValue().x * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.ReadValue().y * mouseSensitivity * Time.deltaTime;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}