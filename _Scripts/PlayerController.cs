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

    [Header("Parry")]
    [SerializeField] Transform parryStepTarget;
    [SerializeField] float parryDashSpeed;
    Vector2 newParryStepTarget;

    [Header("Dodge")]
    [SerializeField] Transform dodgeStepTarget;
    [SerializeField] float dodgeSpeed;
    Vector2 newDodgeStepTarget;

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
    }

    void Update()
    {
        DirectionCheck();
        Flip();
        GroundCheck();
        Gravity();
        Jump();
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
        if (IsAttacking)
        {
            if (Mathf.Abs(theRB.velocity.y) < .1f)
            {
                float _distance = Mathf.Abs(transform.position.x - newParryStepTarget.x);
                if (_distance > .1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, newParryStepTarget, parryDashSpeed * Time.deltaTime);
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
        jumpRemember -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X))
        {
            jumpRemember = jumpRememberTime;
        }

        if(isGrounded && jumpRemember > 0f)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        }
    }

    public float GetPlayerDirection()
    {
        return staticDirection;
    }
    void Gravity()
    {
        if (theRB.velocity.y > 0)
        {
            theRB.gravityScale = 7f;
        }
        else if (theRB.velocity.y < 0)
        {
            theRB.gravityScale = 11f;
        }
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
