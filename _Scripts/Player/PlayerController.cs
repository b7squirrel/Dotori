using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    PlayerCapture playercapture;
    PlayerTargetController playerTargetController;
    PlayerData playerData;

    [SerializeField] float moveSpeed;
    Rigidbody2D theRB;
    Animator anim;
    float currentDirection;
    float staticDirection;
    float directionOnDOdgeTurn;
    float direcitonOnCapture;

    [SerializeField] bool isGrounded;
    [SerializeField] bool canDoubleJump;
    [SerializeField] bool isOnSlope;
    [SerializeField] bool isJumping;
    [SerializeField] bool isDodgeTurn;
    [SerializeField] bool isOnBouncer;
    [SerializeField] bool isCapturing;

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
    Vector2 slopeNormalPerp;
    float slopeDownAngle;
    float slopeDownAngleOld;

    [Header("Debug")]
    [SerializeField] float xInput; // 디버깅용
    [SerializeField] float friction;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpRememberTime;
    [SerializeField] float coyoteTIme;
    float jumpRemember;
    float coyoteTimeCounter;

    [Header("Parry")]
    [SerializeField] Transform parryStepTarget;
    [SerializeField] float parryDashSpeed;
    Vector2 newParryStepTarget;

    [Header("Dodge")]
    [SerializeField] float dodgeSpeed;
    [SerializeField] SlotPhysics slotPhysicsSet;
    [SerializeField] float DodgeNumberCoolTime;
    [SerializeField] int maxNumberOfDodge;
    [SerializeField] CapsuleCollider2D playerCollisionBox;
    [SerializeField] float dodgeCoolTime;
    PlayerHurtBox playerHurtBox;
    float dodgeCoolTimeCounter;
    int dodgeNumberCounter;
    float dodgeNumberCoolTimeCounter;
    bool onDownKey;  // 아래 버튼이 눌러져 있는지 여부

    [Header("Capture")]
    [SerializeField] float captureMoveSpeed;

    [Header("Bouncer")]
    [SerializeField] float bouncerTime;
    float bouncerTimeCounter;
    Vector2 bouncerForce;

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
    public bool IsDodging { get { return isDodgeTurn; } }
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
        playercapture = GetComponentInChildren<PlayerCapture>();
        playerTargetController = GetComponent<PlayerTargetController>();
        playerHurtBox = GetComponentInChildren<PlayerHurtBox>();
        playerData = GetComponent<PlayerData>();
        footEmission = dustTrailParticle.emission;
        dodgeNumberCounter = maxNumberOfDodge;
    }

    void Update()
    {
        DirectionCheck();
        Flip();
        Gravity();
        SurroundingCheck();

        Dodge();
        Jump();  
        GenerateDustTrail();
        GenerateLandEffect();
        ManageContactStates();
        SetAnimationState();
    }

    private void FixedUpdate()
    {
        SlopeCheck();
        Move();  
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
        // 회피
        if (isDodgeTurn == false)
        {
            directionOnDOdgeTurn = staticDirection;
        }
        if (isDodgeTurn)
        {
            theRB.velocity = new Vector2(directionOnDOdgeTurn * dodgeSpeed, theRB.velocity.y);
            return;
        }

        // 캡쳐
        if (isCapturing == false)
        {
            direcitonOnCapture = staticDirection;
        }
        if (isCapturing)
        {
            theRB.velocity = 
                new Vector2(playercapture.CaptureDirection * captureMoveSpeed, theRB.velocity.y);
            return;
        }

        // 경사면
        if (isGrounded && isOnSlope && isJumping == false)
        {
            theRB.velocity = -currentDirection * moveSpeed * slopeNormalPerp;
            return;
        }

        // 바운서
        if (isOnBouncer)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Bouncer") == false)  // 최초에만 실행하도록
            {
                //theRB.AddForce(bouncerForce, ForceMode2D.Impulse);
                anim.Play("Player_Bouncer");
                theRB.velocity = bouncerForce;
            }
            if (bouncerTimeCounter > 0)
            {
                bouncerTimeCounter -= Time.deltaTime;
            }
            else
            {
                anim.Play("Player_Idle");
                isOnBouncer = false;
            }
            return;
        }
        // 일반
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
    /// 비스듬히 밑이 입력되면 dodge turn. dodge turn이 재생되고 있지 않으면 isDodgeTurn은 false
    /// 롤이 팬 위에 있다면(Is rolls on pan) 회피는 작동하지 않음
    /// 회피 갯수가 남아 있지 않다면 회피는 작동하지 않음 
    /// </summary>
    void Dodge()
    {
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            onDownKey = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            onDownKey = false;
        }

        CoolingDodgeNumber();
        CoolingDodge();

        // 회피 갯수가 없거나 회피한지 얼마되지 않았으면 회피가 되지않음
        if (dodgeNumberCounter <= 0)
            return;
        if (dodgeCoolTimeCounter > 0)
            return;
        
        if (Input.GetAxisRaw("Horizontal") != 0 && onDownKey)
        {
            playercapture.Toss(true); // Toss 함수 안에서 롤이 팬 위에 있는지 없는지 검사함
            anim.Play("Player_Dodge");
            playerData.Play(PlayerData.soundType.dash);
        }
    }
    void CoolingDodgeNumber()
    {
        if (dodgeNumberCounter >= maxNumberOfDodge)
        {
            dodgeNumberCounter = maxNumberOfDodge;
            return;
        }
            
        if (dodgeNumberCoolTimeCounter < DodgeNumberCoolTime)
        {
            dodgeNumberCoolTimeCounter += Time.deltaTime;
            return;
        }
        dodgeNumberCounter++;
        dodgeNumberCoolTimeCounter = 0;
    }
    void CoolingDodge()
    {
        if (dodgeCoolTimeCounter > 0)
        {
            dodgeCoolTimeCounter -= Time.deltaTime;
            return;
        }
        
    }
    void OnIsDodgeTurn()
    {
        isDodgeTurn = true;
        playerCollisionBox.gameObject.layer = 18;  // 18. PlayerDodging
        playerHurtBox.gameObject.SetActive(false);
        dodgeNumberCounter--;
        dodgeNumberCoolTimeCounter = 0; // 회피를 한 번 깎아먹은 시점부터 쿨링 시작
        dodgeCoolTimeCounter = dodgeCoolTime; // 회피 쿨타임 초기화

    }
    void OffIsDodgeTurn()
    {
        isDodgeTurn = false;
        playerCollisionBox.gameObject.layer = 3;  // 3. Player
        playerHurtBox.gameObject.SetActive(true);
        onDownKey = false; // 계속 누르고 있으면 회피가 되는 것을 방지

    }

    void Flip()
    {
        if (playercapture.IsCapturing == false)
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
        else
        {
            if (playercapture.CaptureDirection > 0)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else if (playercapture.CaptureDirection < 0)
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
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
            playerData.Play(PlayerData.soundType.jump);
            GenerateJumpEffect();
        }

        if (coyoteTimeCounter < 0)
        {
            if (Input.GetKeyDown(KeyCode.W) && canDoubleJump)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce * 1.2f);
                playerData.Play(PlayerData.soundType.jump);
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

    public void OnBouncer(Vector2 _bouncerForceVector)
    {
        bouncerForce = _bouncerForceVector;
        isOnBouncer = true;
        bouncerTimeCounter = bouncerTime;
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