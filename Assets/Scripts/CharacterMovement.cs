using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    // ========================================================================
    // SECCIÓN 1: CONFIGURACIÓN GENERAL (Variables Visibles en el Inspector)
    // ========================================================================

    [Header("1. Configuración de Cámara")]
    [Tooltip("Arrastra aquí tu Main Camera")]
    public Transform cameraTransform;
    [Range(0.1f, 10f)] public float mouseSensitivity = 2f;
    public bool invertY = false; // Invertir vista vertical
    public float minPitch = -80f; // Límite mirar abajo
    public float maxPitch = 80f;  // Límite mirar arriba

    [Header("2. Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeedMultiplier = 2f;    // Cuánto multiplica la velocidad al correr
    public float crouchSpeedMultiplier = 0.5f; // Cuánto reduce la velocidad al agacharse
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("3. Configuración de Agachado")]
    public float crouchHeight = 1.0f; // Altura del personaje agachado
    public float crouchTransitionSpeed = 10f; // Qué tan rápido baja/sube la cámara

    // ========================================================================
    // SECCIÓN 2: VARIABLES PRIVADAS (Lógica interna)
    // ========================================================================
    
    private CharacterController controller;
    private float originalHeight;         // Altura original (de pie)
    private Vector3 originalCameraCenter; // Posición original de la cámara
    
    private float cameraPitch = 0f;       // Rotación acumulada de la cámara (X)
    private float yVelocity = 0f;         // Velocidad vertical (Gravedad/Salto)
    private bool hasJumped = false;       // Control para evitar doble salto

    // ========================================================================
    // SECCIÓN 3: MÉTODOS DE UNITY (Start y Update)
    // ========================================================================

    private CmeraRecoil cameraRecoilScript;
    
    void Start()
    {
        // Obtener componentes
        controller = GetComponent<CharacterController>();

        // Bloquear y ocultar el cursor para jugar cómodo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Guardar valores iniciales para poder restaurarlos luego
        originalHeight = controller.height;
        
        if (cameraTransform != null)
        {
            originalCameraCenter = cameraTransform.localPosition;
            cameraPitch = cameraTransform.localEulerAngles.x;
        
            // Obtener la referencia al script de recoil
            cameraRecoilScript = cameraTransform.GetComponent<CmeraRecoil>();
        }
    }

    void Update()
    {
        HandleMouseLook();  // 1. Manejar la vista
        HandleMovement();   // 2. Manejar caminar/correr/agacharse
    }

    // ========================================================================
    // SECCIÓN 4: LÓGICA DETALLADA (Funciones separadas para orden)
    // ========================================================================

    /// <summary>
    /// Se encarga de rotar al personaje (lados) y la cámara (arriba/abajo).
    /// </summary>
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // A) Rotar el CUERPO del personaje horizontalmente (Eje Y)
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);

        if (cameraTransform != null)
        {
            float currentPitch=0; 
        
            // 2. Aplicar el input del mouse al pitch actual
            float rotationAmount = (mouseY * mouseSensitivity) * (invertY ? -1 : 1);
            cameraPitch -= rotationAmount;
            
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
        
            // 4. Aplicar la rotación final SUMANDO el RECOIL
            Vector3 finalRotation = new Vector3(cameraPitch, 0f, 0f);;

            if (cameraRecoilScript != null)
            {
                // Sumamos el recoil (que es un Vector3 de rotación) al ángulo de vista
                // El recoil se suma al pitch (X) y al yaw (Y)
                finalRotation.x += cameraRecoilScript.currentRecoil.x;
                finalRotation.y += cameraRecoilScript.currentRecoil.y;
            }

            // 5. Aplicar la nueva rotación combinada
            cameraTransform.localEulerAngles = finalRotation;
        }
    }

    /// <summary>
    /// Se encarga de mover al personaje, aplicar gravedad, saltos y estados (correr/agacharse).
    /// </summary>
    void HandleMovement()
    {
        // --- A. DETECTAR ENTRADAS (INPUTS) ---
        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D
        float vertical = Input.GetAxisRaw("Vertical");     // W, S
        
        bool isCrouching = Input.GetKey(KeyCode.LeftControl); // Agacharse
        bool isRunning = Input.GetKey(KeyCode.LeftShift);     // Correr
        bool isJumping = Input.GetKeyDown(KeyCode.Space);     // Saltar

        // --- B. CALCULAR VELOCIDAD ---
        // Lógica de prioridad: Si te agachas, ignoramos correr.
        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed *= crouchSpeedMultiplier;
        }
        else if (isRunning)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        // --- C. CALCULAR DIRECCIÓN ---
        // Vector relativo a hacia donde mira el personaje
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        // --- D. GRAVEDAD Y SALTO ---
        if (controller.isGrounded)
        {
            // Pequeña fuerza hacia abajo constante para mantener "pegado" al suelo al player
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
            // Aplicar gravedad en el aire
            yVelocity += gravity * Time.deltaTime;
        }

        // --- E. APLICAR MOVIMIENTO FINAL ---
        Vector3 finalVelocity = moveDirection * currentSpeed;
        finalVelocity.y = yVelocity; // Inyectamos la velocidad vertical calculada

        // Mover el CharacterController
        controller.Move(finalVelocity * Time.deltaTime);

        // --- F. AJUSTAR ALTURA (AGACHADO) ---
        HandleCrouchHeight(isCrouching);
    }
    /// <summary>
    /// Maneja la transición suave de altura y posición de cámara al agacharse.
    /// </summary>
    void HandleCrouchHeight(bool isCrouching)
    {
        // Determinar altura objetivo
        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        float currentHeight = controller.height;

        // 1. INTERPOLAR ALTURA DEL COLLIDER
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.height = currentHeight;

        // 2. AJUSTAR EL CENTRO (Center) del Collider
        // El centro debe ser la mitad de la altura actual del collider.
        // Esto asegura que la base del collider SIEMPRE esté en y=0, sin importar la altura.
        Vector3 targetCenter = new Vector3(0, currentHeight / 2f, 0); 
        controller.center = targetCenter; // No interpolar el centro, ajustarlo directamente para precisión

        // 3. AJUSTAR LA POSICIÓN DE LA CÁMARA
        if (cameraTransform != null)
        {
            // Posición y del centro de la cámara (ej. 0.8f)
            float cameraYOffset = originalCameraCenter.y; 

            // Calcular la diferencia de altura y aplicarla a la cámara
            float heightDifference = originalHeight - currentHeight;
        
            // Bajamos la cámara desde su posición original por la cantidad que bajó el CharacterController.
            Vector3 targetCamPos = new Vector3(
                originalCameraCenter.x, 
                cameraYOffset - heightDifference, // <- La clave del ajuste
                originalCameraCenter.z
            );

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * crouchTransitionSpeed);
        }
    }
    }
