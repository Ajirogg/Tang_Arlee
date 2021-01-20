using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_CC : MonoBehaviour
{
    [Header("Movement")]
    Vector3 lastMovement = Vector3.zero;
    Vector2 InputMovement;
    [SerializeField, Range(0,100)]
    float moveSpeed = 10;
    [SerializeField]
    CharacterController characterController;

    Vector3 moveDirection = Vector3.zero;

    [Space]
    [Space]
    [Header("Jump")]
    int jumpLeft = 1;
    [SerializeField, Range(0, 50)]
    float jumpForce = 22;
    bool isJumping = false;
    bool canJump = true;
    bool jumpOnAir = true;

    [Space]
    [Space]
    [SerializeField]
    Transform cam;
    [SerializeField]
    Transform body;


    Vector3 hitNormal;
    bool isSliding = false;
    [SerializeField]
    float slideFriction = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        moveDirection = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 forward = cam.forward;
        forward.y = 0;
        Vector3 right = cam.right;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        float velocityY = moveDirection.y;

        Debug.Log(new Vector3((1 - hitNormal.y) * hitNormal.x * slideFriction, 0, (1 - hitNormal.y) * hitNormal.z * slideFriction));


        if(isSliding)
            moveDirection = right * InputMovement.x * moveSpeed;
        else
            moveDirection = (forward * InputMovement.y + right * InputMovement.x) * moveSpeed;

        if ((forward * InputMovement.y + right * InputMovement.x) != Vector3.zero)
            lastMovement = (forward * InputMovement.y + right * InputMovement.x);

        body.rotation = Quaternion.LookRotation(lastMovement);

        moveDirection.y = velocityY;



        if (isSliding)
        {
            moveDirection.x += (1 - hitNormal.y) * hitNormal.x * slideFriction;
            moveDirection.z += (1 - hitNormal.y) * hitNormal.z * slideFriction;
        } 



        moveDirection.y = moveDirection.y + (Physics.gravity.y * 0.1f);

        characterController.Move(moveDirection * Time.deltaTime);

        if (characterController.isGrounded)
        {
            moveDirection.y = 0;
            canJump = true;
            jumpLeft = 1;
        } else
        {
            isSliding = !isSliding;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && canJump)
        {
            if (!characterController.isGrounded)
                --jumpLeft;

            if (jumpLeft == 0)
                canJump = false;



            if (jumpOnAir)
                moveDirection.y = jumpForce;
            else 
                moveDirection.y += jumpForce;
        }
    }

    public void GetMovementValues(InputAction.CallbackContext context)
    {
        InputMovement = context.ReadValue<Vector2>();
    }

    public void ChangeJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpOnAir = true;
            //jumpOnAir = !jumpOnAir; 

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isSliding = !(Vector3.Angle(Vector3.up, hit.normal) <= characterController.slopeLimit);
        hitNormal = hit.normal;
        //Debug.Log(Vector3.Angle(Vector3.up, hit.normal));
    }
}
