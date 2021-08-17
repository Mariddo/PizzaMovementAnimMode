using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    public float walkSpeed = 3.0f;
    

    public float crouchMultiplier = 0.75f;

    [Header("Jumping")]
    public float jumpForce = 6.0f;

    public int defaultAdditionalJumps = 1;
    int additionalJumps = 1;

        //BetterJump Behaviors.
    [Header("Jumping Attributes")]
    [Tooltip("Jumping and Falling for the betterJump Code")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;


    [Header("Checking for Grounding")]
    public bool isGrounded = true;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;

    float lastTimeGrounded = 1.0f;

    
    Rigidbody2D rb;

    private PlayerActionControls playerActionControls;
    public float moveInput, jumpInput, lookInput, dashInput;


    [Header("Dashing Gear System")]
    public int gear;

    public float[] gearSpeeds;

    [Header("Direction")]
    public int direction;

    [Header ("Animator")]
    public Animator animator;

    [Header ("Turning Around")]
    //Turning: For High-Speed Turning at Gears 2 to 4.
    public bool turning;


    //hiSpeedTurn: This variable is needed for the high speed turning mechanic.
    public bool hiSpeedTurn;
    public bool turnInProgress;

    //turnSpeedMod:  We need this for making the player slow down as they begin to commit to their turn.
    public float turnSpeedMod;

    [Header ("Slowing Down")]
    public bool slowdown;

    void Awake()
    {
        playerActionControls = new PlayerActionControls();
        
    }

    private void OnEnable()
    {
        playerActionControls.Enable();
    }

    private void OnDisable()
    {
        playerActionControls.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        gear = 0;
        lastTimeGrounded = 0;

        direction = 1;

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        turning = false;
        hiSpeedTurn = false;
        turnInProgress = slowdown = false;

        turnSpeedMod = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        CheckForInput();

        ChangeDirection();
        HiSpeedFlip2();

        SetAnimParams();

        SetMovementSpeed();

        Jump();
    }

    void CheckForInput()
    {
        moveInput = playerActionControls.Land.Run.ReadValue<float>();

        jumpInput = playerActionControls.Land.Jump.ReadValue<float>();

        lookInput = playerActionControls.Land.Look.ReadValue<float>();

        dashInput = playerActionControls.Land.Dash.ReadValue<float>();
    }


    void ChangeDirection()
    {
        if(gear < 2)
        {
            if(direction == 1 && moveInput < 0)
            {
                direction = -1;
            }
            else if(direction == -1 && moveInput > 0)
            {
                direction = 1;
            }
        }
        else
        {
            Debug.Log("Change Direction at Gear: " + gear);

            //HiSpeedTurn will become true, and the player will begin to shift their momentum.
            if(((direction == 1 && moveInput < 0) || (direction == -1 && moveInput > 0)) && (!turnInProgress && !hiSpeedTurn))
            {
                hiSpeedTurn = true;
                turning = true;
                turnInProgress = true;
                direction = direction * -1;

            }


        }



    }


    void HiSpeedFlip2()
    {
        if(hiSpeedTurn)
        {
            if(turnInProgress == false)
            {
                
                hiSpeedTurn = false;
                turnInProgress = false;
            }

        }


    }


    void SetAnimParams()
    {
        animator.SetBool("Walk", (moveInput != 0));

        animator.SetBool("Jump", (!isGrounded));

        animator.SetBool("Dash", (dashInput != 0));

        animator.SetBool("Turn", (hiSpeedTurn != false));

    }

    void SetMovementSpeed()
    {
        float determinedSpeed = 0.0f;

        if(gear == 0 && moveInput != 0)
        {

            Debug.Log("Walking");
            rb.velocity = new Vector2(walkSpeed * direction , rb.velocity.y);
        }
        else if(dashInput != 0)
        {
            if(gear > 0 && gear < gearSpeeds.Length)
            {
                determinedSpeed = gearSpeeds[gear-1];
            }
            else
            {
                determinedSpeed = walkSpeed;
            }

            float turnDiagonal = turnInProgress ? -1: 1;

            rb.velocity = new Vector2(determinedSpeed * direction * turnSpeedMod * turnDiagonal, rb.velocity.y);
        }
        else if(slowdown)
        {
            if(gear > 0 && gear < gearSpeeds.Length)
            {
                determinedSpeed = gearSpeeds[gear-1];
            }
            else
            {
                determinedSpeed = walkSpeed;
            }
            
            rb.velocity = new Vector2(determinedSpeed * direction * turnSpeedMod, rb.velocity.y);



        }
        else if(dashInput == 0 && moveInput == 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        

    }

    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (colliders != null) {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        } else {
            if (isGrounded) {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }

        
    }

    void Jump() {


        if (jumpInput != 0 && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0) && !turnInProgress && !slowdown) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps--;
        }
    }

    void BetterJump() {
        if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }  

        //currentVerticalVelocity = rb.velocity.y; 
    }

}
