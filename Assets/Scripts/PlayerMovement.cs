using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D playerRB;
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerBoxCollider;

    private float moveInput;
    [SerializeField]private float moveSpeed = 5.0f;
    [SerializeField]private float jumpForce = 5.0f;

    private static readonly int idleAnimation = Animator.StringToHash("IdleAnimation");
    private static readonly int walkAnimation = Animator.StringToHash("WalkAnimation");
    private static readonly int jumpAnimation = Animator.StringToHash("JumpAnimation");

    [SerializeField]private float extraBoxHeight = 1.0f;
    [SerializeField]private LayerMask groundLayer;

    [SerializeField]private ParticleSystem dustFX;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Movement.Jump.performed += _ => OnJump();
    }

    private void Update()
    {
        // Function?
        moveInput = playerInput.Movement.Move.ReadValue<float>();

        FlipPlayerSprite();
        PlayerDustFX();
        SetAnimation(GetAnimation(playerRB.velocity));
    }

    private void FixedUpdate()
    {
        IsGrounded();
        // Function
        // To be changed to AddForce() 
        playerRB.velocity = new Vector2(moveInput * moveSpeed, playerRB.velocity.y);
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Movement.Jump.performed -= _ => OnJump();
    }

    //////////////////////////////////////////////// Custom Functions ////////////////////////////////////////////////

    // Movement Function 
    /* 
     * NotImplementedException 
     */

    private void OnJump()
    {
        // Only jump when grounded
        /* Can be changed to a bool value and it's own function
         * something like jumpRequested = false 
         * OnjumpInput() => jumpRequested = true
         * DoJump() => AddForce() + jumpRequested = False;
         */
        if (IsGrounded())
        {
            playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Ground Check Function
    private bool IsGrounded()
    {
        RaycastHit2D groundCheck = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, extraBoxHeight, groundLayer);

        Color rayColor = Color.gray;
        if (groundCheck.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(playerBoxCollider.bounds.center + new Vector3(playerBoxCollider.bounds.extents.x, 0), Vector2.down * (playerBoxCollider.bounds.extents.y + extraBoxHeight), rayColor);
        Debug.DrawRay(playerBoxCollider.bounds.center - new Vector3(playerBoxCollider.bounds.extents.x, 0), Vector2.down * (playerBoxCollider.bounds.extents.y + extraBoxHeight), rayColor);
        Debug.DrawRay(playerBoxCollider.bounds.center - new Vector3(playerBoxCollider.bounds.extents.x, playerBoxCollider.bounds.extents.y), Vector2.right * (playerBoxCollider.bounds.extents.y + extraBoxHeight), rayColor);
        /*
        Debug.Log(groundCheck.collider);
        */

        return groundCheck.collider != null;
    }

    private void FlipPlayerSprite()
    {
        if (moveInput != 0f)
        {
            if (moveInput < 0f)
            {
                playerSprite.flipX = true;
            }
            else
            {
                playerSprite.flipX = false;
            }
        }
    }
    private void PlayerDustFX()
    {
        if (moveInput != 0f && IsGrounded())
        {
            dustFX.Play();
        }
        if (!IsGrounded())
        {
            dustFX.Stop();
        }
    }
    private int GetAnimation(Vector2 velocity)
    {
        /// Player moving but not jumping/falling
        if (moveInput != 0 && playerRB.velocity.y == 0)
        {
            return walkAnimation;
        }
        if (playerRB.velocity.y != 0)
        {
            return jumpAnimation;
        } 
        else
        {
            return idleAnimation;
        }
    }
    private void SetAnimation(int animation)
    {
        playerAnimator.CrossFade(animation, 0);
    }
}
