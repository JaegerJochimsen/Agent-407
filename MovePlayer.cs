using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit: https://www.youtube.com/watch?v=_QajrabyTJc&t=97s  (basic move and jump code -- sprint, stamina, etc is my own)

public class MovePlayer : MonoBehaviour
{
    public CharacterController controller;

    public float grav = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform enemyAttackCheck;
    public float enemyDistance = 1f;
    public LayerMask enemyMask;
    public float enemyDamage = 20f;

    float knockBack = 4f;
    float knockUp = 1.5f;
    bool inAirKnock;

    Vector3 velocity;
    bool isGrounded;

    static public float health = 100f;
    static public float stamina = 100f;
    float speed = 12f;
    float runningBoost = 24f;
    bool sprinting;
    bool enemyAttacking;
    bool stamCharged;
    public float timer = 5;

    // Update is called once per frame
    void Update()
    {
        // check stamina and reduce to standard if necessary
        CheckStamina();
        
        // set boolean to know when the player is on the ground (uses groundCheck)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // set boolean to know if an enemy is within attack range 
        enemyAttacking = Physics.CheckSphere(enemyAttackCheck.position, enemyDistance, enemyMask);

        // if player is on the ground, coming down from the air, stop excess acceleration left from in-air acceleration
        SettlePosition();
    
        // Check input to see if jumping, if so then jump
        Jump();

        // if player is on the ground and LShift is pressed down, then they are sprinting
        Sprint();

        // If the enemy is attacking then reduce health and knock back
        if (enemyAttacking)
        {
            BeAttacked();
        }

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
            timer = 5f;
        }
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
            stamina -= 25f * Time.deltaTime;

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
            stamina += 30f * Time.deltaTime;

            // if they are not yet back to base movement speed, decrease
            if (speed > (12 + runningBoost * Time.deltaTime))
            {
                speed -= runningBoost * Time.deltaTime;
            }
        }
    }

    ///////////////////////////////////////////////
    ///               Enemy Attack              ///
    ///////////////////////////////////////////////

    void BeAttacked()
    {
        //Find all the enemies and determine which are close enough to be contacting:
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            // if we find an enemy that IS within striking distance and is attacking, then we will apply damage and knockback
            if (gameObj.name == "Enemy" && (((gameObj.transform.position - controller.transform.position).z <= 1f && (gameObj.transform.position - controller.transform.position).x <= 1f)))
            {
                velocity.x += enemyAI.dir.x* knockBack;
                velocity.z += enemyAI.dir.z* knockBack;
                velocity.y += Mathf.Sqrt(knockUp* (-2f) * grav);
                velocity.y += grav* Time.deltaTime;
                controller.Move(velocity* Time.deltaTime);
                // set bool so that when we land we can stop moving back
                inAirKnock = true;
            }
        }


        // if we have health left then subtract from it, else we die (IMPLEMENT PLAYER DEATH FUNCT AND SCREEN)
        if (health > enemyDamage)
        {
            health -= enemyDamage;
        }
        else
        {
            health = 0;
        }
    }
}
