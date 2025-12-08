using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{

    [Header("1. Configuración de Cámara")]
    [Tooltip("Arrastra aquí tu Main Camera")]
    public Transform cameraTransform;
    [Range(0.1f, 10f)] public float mouseSensitivity = 2f;
    public bool invertY = false; 
    public float minPitch = -80f; 
    public float maxPitch = 80f;  
    
    [Header("1. Configuración de Cámara")]
    [Tooltip("Arrastra aquí tu objeto Pivot/Container de la cámara (el padre de Main Camera)")]
    public Transform cameraPivotTransform;

    [Header("2. Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeedMultiplier = 2f;    
    public float crouchSpeedMultiplier = 0.5f; 
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("3. Configuración de Agachado")]
    public float crouchHeight = 1.0f; 
    public float crouchTransitionSpeed = 10f; 

    private CharacterController controller;
    private float originalHeight;         
    private Vector3 originalCameraCenter; 
    
    private float cameraPitch = 0f;      
    private float yVelocity = 0f;        
    private bool hasJumped = false;       

    private CmeraRecoil cameraRecoilScript;
    
    void Start()
    {
        
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeight = controller.height;
        
        if (cameraPivotTransform == null)
        {
            if (transform.childCount > 0)
            {
                cameraPivotTransform = transform.GetChild(0);
            }
        }
    
        if (cameraPivotTransform != null)
        {
            cameraTransform = cameraPivotTransform.GetComponentInChildren<Camera>().transform;

            originalCameraCenter = cameraPivotTransform.localPosition; 
            cameraPivotTransform.localPosition = originalCameraCenter; 

            cameraPitch = 0f; 
            
            cameraPivotTransform.localEulerAngles = Vector3.zero;
        
            cameraRecoilScript = cameraPivotTransform.GetComponent<CmeraRecoil>();
        }
    }

    void Update()
    {
        HandleMouseLook();  
        HandleMovement();   
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    
        if (cameraRecoilScript != null)
        {
            transform.Rotate(Vector3.up * cameraRecoilScript.currentRecoil.y); 
        }
    
        if (cameraPivotTransform != null) 
        {
        
            Vector3 finalRotation = new Vector3(cameraPitch, 0f, 0f);

            if (cameraRecoilScript != null)
            {
                finalRotation.x += cameraRecoilScript.currentRecoil.x;
            }

            cameraPivotTransform.localEulerAngles = finalRotation; 
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D
        float vertical = Input.GetAxisRaw("Vertical");     // W, S
        
        bool isCrouching = Input.GetKey(KeyCode.LeftControl); // Agacharse
        bool isRunning = Input.GetKey(KeyCode.LeftShift);     // Correr
        bool isJumping = Input.GetKeyDown(KeyCode.Space);     // Saltar

        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed *= crouchSpeedMultiplier;
        }
        else if (isRunning)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 moveDirection = (right * horizontal + forward * vertical).normalized;

        if (controller.isGrounded)
        {
            yVelocity = -9.81f; 
            hasJumped = false;

            if (isJumping)
            {
                yVelocity = jumpForce;
                hasJumped = true;
            }
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = moveDirection * currentSpeed;
        finalVelocity.y = yVelocity; 

        controller.Move(finalVelocity * Time.deltaTime);

        HandleCrouchHeight(isCrouching);
    }
    
    void HandleCrouchHeight(bool isCrouching)
    {
        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        float currentHeight = controller.height;

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.height = currentHeight;

        Vector3 targetCenter = new Vector3(0, currentHeight / 2f, 0); 
        controller.center = targetCenter; 

        if (cameraPivotTransform != null)
        {
            float heightDifference = originalHeight - controller.height;
            float targetCamY = originalCameraCenter.y - heightDifference; 

            Vector3 targetPivotPos = new Vector3(
                originalCameraCenter.x,
                targetCamY, 
                originalCameraCenter.z  
            );

            cameraPivotTransform.localPosition = Vector3.Lerp(
                cameraPivotTransform.localPosition, 
                targetPivotPos, 
                Time.deltaTime * crouchTransitionSpeed
            );
        }
    }
    }
