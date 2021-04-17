﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    Vector2 inputMovement = Vector2.zero;
    [SerializeField, Range(0, 100)]
    float moveSpeed = 10;
    [SerializeField]
    CharacterController characterController;

    Vector3 moveDirection = Vector3.zero;
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
    Transform cam;
    [SerializeField]
    Transform body;


    // Data for slide the slope
    Vector3 hitNormal = Vector3.up;
    bool isSliding = false;
    float slideFriction = 1.5f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Debug.Log(jumpLeft);

        Vector3 forward = cam.forward;
        forward.y = 0;
        Vector3 right = cam.right;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * inputMovement.y + right * inputMovement.x) * moveSpeed;
        if(inputMovement != Vector2.zero)
        {
            lastDirection = movement;
        }

        moveDirection = new Vector3(movement.x, moveDirection.y, movement.z);


        if (isSliding)
        {
            moveDirection.x += (1 - hitNormal.y) * hitNormal.x * slideFriction;
            moveDirection.z += (1 - hitNormal.y) * hitNormal.z * slideFriction;
        }

        body.transform.rotation = Quaternion.LookRotation(lastDirection);

        characterController.Move(moveDirection * Time.deltaTime);

        if (characterController.isGrounded)
        {
            moveDirection.y = 0;
            canJump = true;
            jumpLeft = maxJumpLeft;
        } 
    }

    private void FixedUpdate()
    {
        moveDirection += (Physics.gravity * 0.1f);
    }

    public void GetMovementValues(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && canJump)
        {
            if (!characterController.isGrounded)
                --jumpLeft;
       
                canJump = false;

            moveDirection.y = jumpForce;
        }
    }

    public void Interract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CheckIfChestInterract( Physics.OverlapBox(body.position + body.forward, new Vector3(0.375f, 1f, 0.375f)));
        }
    }

    private void CheckIfChestInterract(Collider[] colliders)
    {
        Debug.Log(colliders.Length);
        foreach(Collider collider in colliders)
        {
            Debug.Log(collider.gameObject.tag);

            if (collider.gameObject.CompareTag("Chest"))
            {
                Debug.Log(collider.gameObject.GetComponent<Chest>().OpenChest().name);
            } else
            {
                Debug.Log("I can't do anything with this...");
            }
        }
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isSliding = !(Vector3.Angle(Vector3.up, hit.normal) <= characterController.slopeLimit);
        hitNormal = hit.normal;
        // Debug.Log(Vector3.Angle(Vector3.up, hit.normal));
    }

    void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(body.position + body.forward, new Vector3(0.75f, 2f, 0.75f));
    }
}
