using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
public class CharacterControllerMovement : MonoBehaviour
{

    PlayerInputs playerInputs;
    public float speed = 5f;
    public float gravity = -9.18f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    Vector3 currentMovement;
    Vector2 currentMovementInput;
    bool isMovementPressed;

    bool running = false;
    Animator animator;

    float rotationFactorPerFrame = 1f;
    public static Func<bool> OnAttack;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //Events

        playerInputs.CharacterController.Move.performed += context => { setMovement(context);};
        playerInputs.CharacterController.Move.canceled += context => { setMovement(context);};

        playerInputs.CharacterController.PrimaryAttack.started += context => 
        {
            if (OnAttack != null && OnAttack.Invoke())
            {
                animator.SetTrigger("primaryAttack");
            }
        };

        playerInputs.CharacterController.EquipTorch.started += context =>
        {
            

            bool equipped = animator.GetBool("flashlightEquipped");
            animator.SetBool("flashlightEquipped", !equipped);
            Debug.Log("Torch toggled " + equipped);
        };

        playerInputs.CharacterController.SecondaryAttack.started += context => { Debug.Log("Secondary Attacked!"); animator.SetTrigger("secondaryAttack"); };

        playerInputs.CharacterController.Sprint.performed += context => { running = true; };
        playerInputs.CharacterController.Sprint.canceled += context => { running = false; };

        playerInputs.CharacterController.Dodge.started += context => { Debug.Log("Dodged!"); };
    }

    private void OnEnable()
    {
        playerInputs.CharacterController.Enable();
    }

    private void OnDisable()
    {
        playerInputs.CharacterController.Disable();
    }

    private void FixedUpdate()
    {
        controller.Move(currentMovement * speed * Time.deltaTime);

        bool isWalking = currentMovementInput.magnitude > 0.1f;
        bool isRunning = isWalking && running;
     
        if (!isWalking)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        else if (isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        rotation();

    }

    private void setMovement(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void rotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        }
    }
}
