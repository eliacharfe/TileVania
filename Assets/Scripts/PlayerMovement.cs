using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   Vector2 moveInput;
   Rigidbody2D myRigidbody;
   Animator myAnimator;
   CapsuleCollider2D myCapsuleCollider;

   [SerializeField] float runSpeed = 10f;
   [SerializeField] float jumpSpeed = 25f;
   [SerializeField] float climbSpeed = 5f;
   [SerializeField] float gravityScaleStart = 8f;
   [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
   [SerializeField] GameObject bullet;
   [SerializeField] Transform gun;

   bool isAlive = true;
   
    void Start()
    {
        myRigidbody = GetComponent< Rigidbody2D>();
        gravityScaleStart = myRigidbody.gravityScale ;
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if(!isAlive)
        { return;}
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value)
    {
        if(!isAlive)
        { return;}
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void OnMove(InputValue value)
    {
        if(!isAlive)
        { return;}
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }
     
    void OnJump(InputValue value)
    {
        if(!isAlive)
        { return;}
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
           return;

         if(value.isPressed)
         {
             myRigidbody.velocity += new Vector2(0f, jumpSpeed);
         }
    }

   
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x),1f);
    }

     void ClimbLadder()
    {
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = gravityScaleStart;
             return;
        }
          
        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x,moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);

    }

    void Die() 
    {
        if(myRigidbody.IsTouchingLayers(LayerMask.GetMask("Enemies", "Spikes")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
        }
    }

   

}
