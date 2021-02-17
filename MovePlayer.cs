using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit: https://www.youtube.com/watch?v=_QajrabyTJc&t=97s  (basic move and jump code -- sprint, stamina, etc is my own)

public class MovePlayer : MonoBehaviour
{
    public CharacterController controller;

    static public float grav = -9.81f * 4.5f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    static public bool inAirKnock;

    static public Vector3 velocity;
    bool isGrounded;

    static public float health = 100f;
    static public float stamina = 100f;
    float speed = 12f;
    float runningBoost = 24f;
    bool sprinting;
    bool stamCharged = true;
    public float timer = -1;

    public staminaBar staminaBar;

    // Update is called once per frame
    void Update()
    {
        // check stamina and reduce to standard if necessary
        CheckStamina();
        
        // set boolean to know when the player is on the ground (uses groundCheck)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // if player is on the ground, coming down from the air, stop excess acceleration left from in-air acceleration
        SettlePosition();
    
        // Check input to see if jumping, if so then jump
        Jump();

        // if player is on the ground and LShift is pressed down, then they are sprinting
        Sprint();

        Move();
    }

    void Move()
    { 
        // get player movement (FB and LR)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // move player according to their input
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }

    void SettlePosition() {
        // if player is on the ground and still accellerating
        if (isGrounded && velocity.y < 0)
        {
            // stop downward acceleration
            velocity.y = 0f;

            // used when in air due to knockback -- when landed settle in xz-directions
            if ((Mathf.Abs(velocity.z) > 0f || Mathf.Abs(velocity.x) > 0f) && inAirKnock) {
                velocity.z = 0f;
                velocity.x = 0f;
                inAirKnock = false;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ///     Jumping stuff:                                                                       ///   
    ///     + may add wall jumping if time allows                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////

    void Jump()
    {
        // player can only jump when on the ground 
        if (Input.GetKeyDown("space") && isGrounded)
        {
            // calculate jump velocity based off of jump height and gravity
            velocity.y += Mathf.Sqrt(jumpHeight * (-2f) * grav);

        }

        // part of the grav^2; move the player
        velocity.y += grav * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ///     Sprinting stuff:                                                                     ///
    ///     x - After stamina is exhausted it must be fully recharged in order to sprint again   ///
    ///       - Must press shift again in order to sprint again                                  ///
    ///       - Play with player movement, must feel crisp but not floaty                        ///
    ////////////////////////////////////////////////////////////////////////////////////////////////
    void CheckStamina()
    {
        if (stamina >= 99)
        {
            stamCharged = true;
        }
        else if (stamina <= 0)
        {
            stamCharged = false;
            speed = 12f;
            timer = 4.75f;
        }

        if(timer >= 0)
            timer -= Time.deltaTime;
    }

    void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            sprinting = true;
        }

        // when they stop pressing LShift they no longer sprint
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprinting = false;
        }

        // if player is trying to run and they have stamina left and don't need to recharge stamina
        if (sprinting && (stamina > 0) && stamCharged && (timer < 0))
        {
            // decrease stamina by some propotion each frame they are sprinting
            stamina -= 15f * Time.deltaTime;
            staminaBar.SetStamina(stamina);

            // once they hit max speed then don't get faster
            if (speed < (24 - runningBoost * Time.deltaTime))
            {
                // increase speed by the boost, make it unaffected by frame rate
                speed += runningBoost * Time.deltaTime;
            }
        }

        // if they are not sprinting and they have less than 100 stamina, recharge it
        else if (stamina < 99)
        {
            // recharge stamina
            stamina += 25f * Time.deltaTime;
            staminaBar.SetStamina(stamina);

            // if they are not yet back to base movement speed, decrease
            if (speed > (12 + runningBoost * Time.deltaTime))
            {
                speed -= runningBoost * Time.deltaTime;
            }
        }
    }
}
