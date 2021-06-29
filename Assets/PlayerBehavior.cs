using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    public float walkSpeed = 3.0f;
    

    public float fallMultiplier = 1.05f;

    public float lowJumpMultiplier = 1.1f;

    public float crouchMultiplier = 0.75f;

    [Header("Jumping")]
    public float jumpForce = 6.0f;

    public int defaultAdditionalJumps = 1;
    int additionalJumps = 1;


    [Header("Checking for Grounding")]
    public bool isGrounded = true;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;

    float lastTimeGrounded;

    
    Rigidbody2D rb;

    private PlayerActionControls playerActionControls;
    public float moveInput, jumpInput, lookInput, dashInput;


    [Header("Dashing Gear System")]
    public int gear;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        CheckForInput();
    }

    void CheckForInput()
    {
        moveInput = playerActionControls.Land.Run.ReadValue<float>();

        jumpInput = playerActionControls.Land.Jump.ReadValue<float>();

        lookInput = playerActionControls.Land.Look.ReadValue<float>();

        dashInput = playerActionControls.Land.Dash.ReadValue<float>();
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
}
