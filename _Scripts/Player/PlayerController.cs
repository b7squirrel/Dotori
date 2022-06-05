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

    [Header("Ground Check")]
    bool isGrounded;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    
    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpRememberTime;
    float jumpRemember;
    [SerializeField] float coyoteTIme;
    float coyoteTimeCounter;
    bool canDoubleJump;

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
    [SerializeField] GameObject dustTrail;
    [SerializeField] GameObject dustJump;
    [SerializeField] GameObject dustExtraJump;
    [SerializeField] GameObject dustLand;
    [SerializeField] GameObject debugDot;
    ParticleSystem.EmissionModule footEmission;
    bool wasGrounded;

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
        GroundCheck();
        Jump();
        GenerateDustTrail();
        GenerateLandEffect();
        SetAnimationState();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .1f, whatIsGround);
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
        if (IsDodging)
        {
            if (isGrounded)
            {
                float _distance = Mathf.Abs(transform.position.x - newDodgeStepTarget.x);
                if (_distance > .1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, newDodgeStepTarget, dodgeSpeed * Time.deltaTime);
                    return;
                }
            }
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

    void Jump()
    {
        if (isGrounded)
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

        if (coyoteTimeCounter <= 0)
        {
            if (Input.GetKeyDown(KeyCode.X) && canDoubleJump)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce * 1.2f);
                canDoubleJump = false;
                GenerateJumpEffect();
            }
        }
    }

    void ManageCoyoteTime()
    {
        if (isGrounded)
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
            dustTrail.gameObject.SetActive(true);
        }
        else
        {
            footEmission.rateOverTime = 0;
            dustTrail.gameObject.SetActive(false);
        }
    }

    void GenerateJumpEffect()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, .1f, whatIsGround);
        Vector2 contactPoint = hit.point;
        DebugRay(Color.red);
        if (hit)
        {
            DebugRay(Color.green);
            Instantiate(dustJump, contactPoint, Quaternion.identity);
            return;
        }
        if (canDoubleJump == false)
        {
            Instantiate(dustExtraJump, groundCheck.position, Quaternion.identity);
        }
    }

    void DebugRay(Color _color)
    {
        Debug.DrawRay(groundCheck.position, Vector2.down, _color);
    }

    void GenerateLandEffect()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, .1f, whatIsGround);
        Vector2 contactPoint = hit.point;
        if (!wasGrounded && isGrounded)
        {
            Instantiate(dustLand, contactPoint, Quaternion.identity);
        }

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
