using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] float moveSpeed;
    Rigidbody2D theRB;
    Animator anim;
    float currentDirection;
    float staticDirection;

    [SerializeField] bool isGrounded;
    [SerializeField] bool canDoubleJump;
    [SerializeField] bool isOnSlope;
    [SerializeField] bool isJumping;
    [SerializeField] bool isDodgeTurn;

    [Header("Ground Check")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform ledgeCheck;
    [SerializeField] float groundCheckRadius;

    [Header("Ground Check Gizmo parameters")]
    Color gizmoColorNotTouching = Color.red;
    Color gizmoColorIsTouching = Color.green;

    [Header("Wall Sliding")]
    [SerializeField] float wallSlidingSpeed;

    [Header("Slope")]
    [SerializeField] float slopeCheckDistance;
    [SerializeField] PhysicsMaterial2D playerFriction;

    [Header("Debug")]
    [SerializeField] float xInput; // 디버깅용
    [SerializeField] float friction;

    Vector2 slopeNormalPerp;
    float slopeDownAngle;
    float slopeDownAngleOld;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpRememberTime;
    float jumpRemember;
    [SerializeField] float coyoteTIme;
    float coyoteTimeCounter;

    [Header("Parry")]
    [SerializeField] Transform parryStepTarget;
    [SerializeField] float parryDashSpeed;
    Vector2 newParryStepTarget;

    [Header("Dodge")]
    [SerializeField] Transform dodgeStepTarget;
    [SerializeField] float dodgeSpeed;
    Vector2 newDodgeStepTarget;

    [Header("Particle")]
    [SerializeField] ParticleSystem dustTrailParticle;
    [SerializeField] GameObject dustJump;
    [SerializeField] GameObject dustExtraJump;
    [SerializeField] GameObject dustLand;
    [SerializeField] GameObject debugDot;
    ParticleSystem.EmissionModule footEmission;
    bool wasGrounded;
    bool wasTouchingWall;
    Vector2 wallContactPoint;
    Vector2 GroundContactPoint;

    public bool IsGrounded { get { return isGrounded; } }
    public bool IsAttacking { get; set; }
    public bool IsDodging { get; set; }
    public bool IsOnSlope
    {
        get { return isOnSlope; }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footEmission = dustTrailParticle.emission;
    }

    void Update()
    {
        DirectionCheck();
        Flip();
        Gravity();
        SurroundingCheck();

        Jump();  // state
        GenerateDustTrail();
        GenerateLandEffect();
        ManageContactStates();
        SetAnimationState();
    }

    private void FixedUpdate()
    {
        SlopeCheck();
        Move();  // state
    }

    void SurroundingCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }

    void SlopeCheck()
    {
        Vector2 checkPos = groundCheck.position;
        SlopeCheckVertical(checkPos);
    }

    void SlopeCheckHorizontal(Vector2 checkPos)
    {

    }
    void SlopeCheckVertical(Vector2 checkPos)
    {
        xInput = Input.GetAxisRaw("Horizontal");
        friction = theRB.sharedMaterial.friction;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit == false)
        {
            isOnSlope = false;
            theRB.isKinematic = false;
            return;
        }

        slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
        slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (isGrounded && slopeDownAngle != 0f)
        {
            isOnSlope = true;
        }
        else
        {
            isOnSlope = false;
        }

        if (isOnSlope && Input.GetAxisRaw("Horizontal") == 0.0f)
        {
            theRB.isKinematic = true;
        }
        else
        {
            theRB.isKinematic = false;
        }

        Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
        Debug.DrawRay(hit.point, hit.normal, Color.green);

    }

    void DirectionCheck()
    {
        currentDirection = Input.GetAxisRaw("Horizontal");

        // -1과 1만 있는 staticDirection
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            staticDirection = Input.GetAxisRaw("Horizontal");
        }
    }

    void Move()
    {
        if (isGrounded && isOnSlope && isJumping == false)
        {
            //theRB.velocity = new Vector2(-currentDirection * moveSpeed * slopeNormalPerp.x, -currentDirection * moveSpeed * slopeNormalPerp.y);
            theRB.velocity = -currentDirection * moveSpeed * slopeNormalPerp;
            return;
        }
        theRB.velocity = new Vector2(currentDirection * moveSpeed, theRB.velocity.y);
    }

    /// <summary>
    /// 패리 대쉬할 위치 설정
    /// </summary>
    public void SetParryStepTarget()
    {
        newParryStepTarget = parryStepTarget.position;
    }

    /// <summary>
    /// 닷지할 위치 설정
    /// </summary>
    public void SetDodgeStepTarget()
    {
        newDodgeStepTarget = dodgeStepTarget.position;
    }

    void Flip()
    {
        if (currentDirection > 0f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (currentDirection < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColorNotTouching;
        if (isGrounded)
            Gizmos.color = gizmoColorIsTouching;
        //Gizmos.DrawWireCube(groundCheck.position, new Vector2(.55f, .34f));
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void Jump()
    {
        if (isGrounded || isOnSlope)
        {
            canDoubleJump = true;
        }

        jumpRemember -= Time.deltaTime;
        ManageCoyoteTime();

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpRemember = jumpRememberTime;
        }

        if (jumpRemember > 0f && coyoteTimeCounter > 0)
        {
            isJumping = true;
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            GenerateJumpEffect();
        }

        if (coyoteTimeCounter < 0)
        {
            if (Input.GetKeyDown(KeyCode.W) && canDoubleJump)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce * 1.2f);
                GenerateExtraJumpDust();
                canDoubleJump = false;
            }
        }

        if (theRB.velocity.y < .1f)
        {
            isJumping = false;
        }
    }

    void ManageCoyoteTime()
    {
        if (isGrounded || isOnSlope)
        {
            coyoteTimeCounter = coyoteTIme;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    public float GetPlayerDirection()
    {
        return staticDirection;
    }
    void Gravity()
    {
        if (theRB.velocity.y > 0 || coyoteTimeCounter > 0 || isGrounded)
        {
            theRB.gravityScale = 7f;
        }
        else if (theRB.velocity.y < 0)
        {
            theRB.gravityScale = 6f;
        }
    }

    void GenerateDustTrail()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && isGrounded)
        {
            footEmission.rateOverTime = 5;
        }
        else
        {
            footEmission.rateOverTime = 0;
        }
    }

    void GenerateJumpEffect()
    {
        if (wasGrounded && !isGrounded)
        {
            Instantiate(dustJump, GroundContactPoint, Quaternion.identity);
            return;
        }
        if (wasTouchingWall)
        {
            Instantiate(dustJump, wallContactPoint, Quaternion.identity);
            return;
        }
    }
    void GenerateExtraJumpDust()
    {
        if (canDoubleJump == true && !isGrounded)
        {
            Instantiate(dustExtraJump, groundCheck.position, Quaternion.identity);
        }
    }

    void GenerateLandEffect()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, .1f, whatIsGround);
        Vector2 contactPoint = hit.point;
        if (!wasGrounded && isGrounded)
        {
            Instantiate(dustLand, contactPoint, Quaternion.identity);
        }
    }

    void ManageContactStates()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, whatIsGround);
        RaycastHit2D hitWall = Physics2D.Raycast(wallCheck.position, new Vector2(staticDirection, 0), 1f, whatIsGround);
        GroundContactPoint = hitGround.point;
        wallContactPoint = hitWall.point;

        wasGrounded = isGrounded;
    }

    void SetAnimationState()
    {
        if (Input.GetAxisRaw("Horizontal") == 0)  // idle
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isGrounded", true);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0 && Mathf.Abs(theRB.velocity.y) <= .1f)  // walk
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isGrounded", true);
        }

        if (Mathf.Abs(theRB.velocity.y) < 0.1f && isGrounded)  // not falling
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
            anim.SetBool("isGrounded", true);
        }
        else if (theRB.velocity.y > 0.1f && !isOnSlope && !isGrounded)  // jump
        {
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            anim.SetBool("isGrounded", false);
        }
        else if (theRB.velocity.y < 0 && !isOnSlope && !isGrounded)  // falling

        {
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrounded", false);
        }
    }
}