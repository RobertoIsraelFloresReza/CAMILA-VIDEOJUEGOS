using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    /*-- Variables de movimiento --*/
    public Transform cameraTransform;
    private CharacterController controller;
    public float movementSpeed = 5f;
    public float rotationSpeed;
    public float gravity;
    public float jumpForce = 10f;
    private float movementNormalized;
    
    /*-- Variables de animación --*/
    public Animator animator;
    private readonly int movementSpeedHash = Animator.StringToHash("MovementSpeed");

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            gravity = Physics.gravity.y * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space)) gravity = jumpForce;
        }
        else
        {
            gravity += Physics.gravity.y * Time.deltaTime;
        }

        var gravityVector = Vector3.up * gravity;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
        var cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z);
        var direction = cameraForward * vertical + cameraRight * horizontal;
        movementNormalized = direction.normalized.magnitude;

        controller.Move((direction.normalized * movementSpeed + gravityVector) * Time.deltaTime);
        

        if(direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        animator.SetFloat(movementSpeedHash, movementNormalized);
    }
}
