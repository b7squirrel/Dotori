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

    bool isGrounded;
    bool canDoubleJump;
    bool isTouchingWall;
    bool isWallSliding;
    bool isTouchingLedge;
    bool ledgeDetected;

    [Header("Ground Check")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform ledgeCheck;

    [Header("Ground Check Gizmo parameters")]
    Color gizmoColorNotTouching = Color.red;
    Color gizmoColorIsTouching = Color.green;

    [Header("Wall Sliding")]
    [SerializeField] float wallSlidingSpeed;

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

    public bool IsAttacking { get; set; }
    public bool IsDodging { get; set; }

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
        CheckIfWallSliding();
        Jump();
        GenerateDustTrail();
        GenerateLandEffect();
        ManageContactStates();
        SetAnimationState();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void SurroundingCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(.55f, .34f), 0, whatIsGround);
        isTouchingWall = Physics2D.OverlapBox(wallCheck.position, new Vector2(.2f, .1f), 0, whatIsGround);
        isTouchingLedge = Physics2D.OverlapBox(ledgeCheck.position, new Vector2(.2f, .1f), 0, whatIsGround);
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
        if (IsDodging && isGrounded)
        {
            float _distance = Mathf.Abs(transform.position.x - newDodgeStepTarget.x);
            if (_distance > .1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, newDodgeStepTarget, dodgeSpeed * Time.deltaTime);
                return;
            }
        }
        if (isWallSliding)
        {
            theRB.velocity = new Vector2(currentDirection * moveSpeed, -wallSlidingSpeed);
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

    void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && theRB.velocity.y < 0)
        {
            isWallSliding = true;
            return;
        }
        isWallSliding = false;
    }

    void CheckIfDetectingLedge()
    {
        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColorNotTouching;
        if (isGrounded)
            Gizmos.color = gizmoColorIsTouching;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(.55f, .34f));

        Gizmos.color = gizmoColorNotTouching;
        if (isTouchingWall)
            Gizmos.color = gizmoColorIsTouching;
        Gizmos.DrawWireCube(wallCheck.position, new Vector2(.2f, .1f));

        Gizmos.color = gizmoColorNotTouching;
        if (isTouchingLedge)
            Gizmos.color = gizmoColorIsTouching;
        Gizmos.DrawWireCube(ledgeCheck.position, new Vector2(.2f, .1f));
    }

    void Jump()
    {
        if (isGrounded || isWallSliding)
        {
            canDoubleJump = true;
        }

        jumpRemember -= Time.deltaTime;
        ManageCoyoteTime();

        if (Input.GetKeyDown(KeyCode.X))
        {
            jumpRemember = jumpRememberTime;
        }

        if (jumpRemember > 0f && coyoteTimeCounter > 0)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            GenerateJumpEffect();
        }

        if (coyoteTimeCounter < 0)
        {
            if (Input.GetKeyDown(KeyCode.X) && canDoubleJump)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce * 1.2f);
                GenerateExtraJumpDust();
                canDoubleJump = false;
            }
        }
    }

    void ManageCoyoteTime()
    {
        if (isGrounded || isTouchingWall)
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
            theRB.gravityScale = 11f;
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
        if (canDoubleJump == true && !isGrounded && !isTouchingWall)
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
        wasTouchingWall = isTouchingWall;
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
        else if (theRB.velocity.y > 0.1f)  // jump
        {
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            anim.SetBool("isGrounded", false);
        }
        else if (theRB.velocity.y < 0)  // falling

        {
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrounded", false);
        }
    }
}
