using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float cameraSensitivity = 50;
    public Transform cameraTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    void Update()
    {
        Vector3 direction = target.position - cameraTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cameraTransform.rotation = targetRotation;

        float mouseDelta = Input.mousePositionDelta.x * Time.deltaTime;
//        float mouseY = Input.mousePositionDelta.y * Time.deltaTime;
        
        transform.Rotate(0, mouseDelta * cameraSensitivity, 0);

        //transform de la camara | transforma el cubo
        transform.position = target.position;
        cameraTransform.localPosition = offset;
    }
}