using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator animator = null;
    

    [Header("Movement")]
    Vector2 inputMovement = Vector2.zero;
    [SerializeField, Range(0, 100)]
    float moveSpeed = 10;
    [SerializeField]
    CharacterController characterController;

    Vector3 moveDirection = Vector3.zero;
    //Last direction save (to keep looking the same point when stop moving)
    Vector3 lastDirection = Vector3.zero;

    [Space]
    [Space]
    [Header("Jump")]
    int maxJumpLeft = 0;    // Change the number to add jump (bug sometimes can't multiJump)
    int jumpLeft = 0;
    [SerializeField, Range(0, 50)]
    float jumpForce = 22;
    bool canJump = true;

    [Space]
    [Space]
    [SerializeField]
    Transform cam; // Camera transform
    [SerializeField]
    Transform body; // 3D Model 


    // Coffre loot (Maybe modify if visual add)
    private bool getFirstItem = false;
    private bool getSecondItem = false;
    private bool getThirdItem = false;

    // Data to slide the slope
    Vector3 hitNormal = Vector3.up;
    bool isSliding = false;
    float slideFriction = 1.5f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get forward and right vector of cam (movement in camera referential)
        Vector3 forward = cam.forward;
        forward.y = 0;
        Vector3 right = cam.right;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // recalculate the movement from camera referential)
        Vector3 movement = (forward * inputMovement.y + right * inputMovement.x) * moveSpeed;

        animator.SetFloat("Speed", movement.magnitude);

        
        // Stock last direction to keep looking even if not moving
        if(inputMovement != Vector2.zero)
        {
            lastDirection = movement;
        }

        moveDirection = new Vector3(movement.x, moveDirection.y, movement.z);

        // First part of sliding on slope 
        // Need to rotate the player to match the slope angle and remove the forward input to force slide down with minimal controls 
        if (isSliding)
        {
            moveDirection.x += (1 - hitNormal.y) * hitNormal.x * slideFriction;
            moveDirection.z += (1 - hitNormal.y) * hitNormal.z * slideFriction;
        }

        body.transform.rotation = Quaternion.LookRotation(lastDirection);
        characterController.Move(moveDirection * Time.deltaTime);

        // when touch the ground refill jump
        if (characterController.isGrounded)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("IsLanding", true);
            moveDirection.y = 0;
            canJump = true;
            jumpLeft = maxJumpLeft;
        } 
    }

    //apply the physics regularly (fixed deltatime)
    private void FixedUpdate()
    {
        moveDirection += (Physics.gravity * 0.1f);
        
    }

    //Link to input action for movement
    public void GetMovementValues(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    //Link to input action for jump button
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && canJump)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("IsLanding", false);
     
            // Useless here, usefull to add multi-jump
            if (!characterController.isGrounded)
                --jumpLeft;
       
            // if(jumpLeft <= 0)
            canJump = false;

            moveDirection.y = jumpForce;
            
        }
    }

    //Link to input action for interact button
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CheckIfChestInteract(Physics.OverlapBox(body.position + body.forward + body.up * 0.75f, new Vector3(0.375f, 0.5f, 0.375f)));
        }
    }


    // Verify if it's a chest non open and open it
    private void CheckIfChestInteract(Collider[] colliders)
    {
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Chest"))
            {
                GameObject chest = collider.gameObject;
                if (!chest.GetComponent<Chest>().isOpen)
                {
                    collider.gameObject.GetComponent<Chest>().OpenChest();

                    if (getFirstItem)
                    {
                        if (getSecondItem)
                        {
                            getThirdItem = true;
                        }
                        else
                        {
                            getSecondItem = true;
                        }
                    }
                    else
                    {
                        getFirstItem = true;
                    }

                    Debug.Log("1: " + getFirstItem + "  2: " + getSecondItem + "  3: " + getThirdItem);
                }
                
            } 
        }
    }


    //Check the angle of the floor hit
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isSliding = !(Vector3.Angle(Vector3.up, hit.normal) <= characterController.slopeLimit);
        hitNormal = hit.normal;
    }



    // Winning condition check
    public bool HasAllPartOfArmor()
    {
        return getFirstItem && getSecondItem && getThirdItem;
    }



    // VISUAL DEBUG for interaction zone
    void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(body.position + body.forward + body.up * 0.75f, new Vector3(0.75f, 1f, 0.75f));
    }
}
