using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilRunner : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float idleTime;
    [SerializeField] float keepRunningTime;
    [SerializeField] Transform castingForWallPoint;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float jumpForce;

    [Header("CheckSurroundings")]
    [SerializeField] float distanceToWallForJump;

    Vector2 direction;
    float idleTimeCounter;
    float currentDirection; // player를 지나쳤는지 감지하기 위해.
    bool isFacingLeft = true;
    bool isDetectingPlyer;
    bool isDetectingWall;
    bool isGrounded;
    Rigidbody2D theRB;
    Animator anim;
    EnemyHealth enemyhealth;

    enum enemyState { run, ready, idle, turn, onBouncer, stunned}
    enemyState currentState;
    [SerializeField] enemyState startingState;

    private void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyhealth = GetComponentInChildren<EnemyHealth>();
        currentState = startingState;
        CheckingPlayerPosition();
        currentDirection = direction.x;

    }
    private void Update()
    {
        if (enemyhealth.CheckIsOnBouncer())
        {
            currentState = enemyState.onBouncer;
        }

        switch (currentState)
        {
            case enemyState.ready:
                Ready();
                break;
            case enemyState.idle:
                Idle();
                break;
            case enemyState.run:
                GroundCheck();
                CheckingPlayerPosition();
                Run();
                FacingDirection();
                Flip();
                CheckWallForJump();
                break;
            case enemyState.turn:
                Turn();
                break;
            case enemyState.onBouncer:
                Bouncer();
                break;
            case enemyState.stunned:
                break;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            theRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, groundMask);
    }

    bool PastPlayer()
    {
        if (currentDirection != direction.x)
            return true;
        return false;
    }

    void CheckingPlayerPosition()
    {
        direction =
            (PlayerController.instance.transform.position - transform.position);
        if (direction.x > 0)
        {
            direction = new Vector2(1, 0);
            return;
        }
        direction = new Vector2(-1, 0);
    }
    void FacingDirection()
    {
        if (theRB.velocity.x < 0)
        {
            isFacingLeft = true;
        }
        else if (theRB.velocity.x > 0)
        {
            isFacingLeft = false;
        }
    }
    void Flip()
    {
        if (isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180f, 0);
        }
    }
    void CheckWallForJump()
    {
        float _distanceToWallForJump = distanceToWallForJump;
        if (isFacingLeft == false)
        {
            _distanceToWallForJump = -_distanceToWallForJump;
        }
        Vector2 _endPoint = 
            (Vector2)castingForWallPoint.position + Vector2.left * _distanceToWallForJump;
        RaycastHit2D _hit =
            Physics2D.Linecast(castingForWallPoint.position, _endPoint, groundMask);
        if (_hit)
        {
            Debug.DrawLine(castingForWallPoint.position, _endPoint, Color.green);
            isDetectingWall = true;
            Jump();
        }else
        {
            Debug.DrawLine(castingForWallPoint.position, _endPoint, Color.red);
            isDetectingWall = false;
        }

    }
    void Run()
    {
        if (IsPlayingAnimation("Run") == false) // Enter Run State
        {
            currentDirection = direction.x;
            anim.Play("Run");
        }
        if (PastPlayer()) // Exit Run State
        {
            KeepRunning();
        }
        theRB.velocity = new Vector2(currentDirection * moveSpeed, theRB.velocity.y);
    }
    void Idle()
    {
        if (IsPlayingAnimation("Idle") == false)  // Enter Idle State
        {
            anim.Play("Idle");
            idleTimeCounter = idleTime;
        }
        if (idleTimeCounter > 0)
        {
            idleTimeCounter -= Time.deltaTime;
        }
        else
        {
            currentState = enemyState.turn;  // Exit Idle State
        }
        theRB.velocity = new Vector2(0, theRB.velocity.y);
    }

    void KeepRunning()
    {
        StartCoroutine(KeepRunningCo());
    }
    IEnumerator KeepRunningCo()
    {
        yield return new WaitForSeconds(keepRunningTime);
        currentState = enemyState.idle;
    }
    void Turn()
    {
        if (IsPlayingAnimation("Turn") == false) // Enter Turn State
        {
            anim.Play("Turn");
        }
        if (IsPlayingAnimation("Run")) // Exit Turn State, turn 애니가 끝나면 Run으로 Transition.
        {

            currentState = enemyState.run;
            currentDirection = direction.x;
        }
    }
    void Ready()
    {
        if (IsPlayingAnimation("Ready") == false) // Enter Ready State
        {
            anim.Play("Ready");
        }
        if (isDetectingPlyer == false)
            return;
        currentState = enemyState.run; // Exit Ready State
    }
    void Bouncer()
    {
        if (IsPlayingAnimation("Bouncer") == false)
        {
            anim.Play("Bouncer");
            theRB.velocity = enemyhealth.GetBouncerForce();
        }
        if (enemyhealth.CheckIsOnBouncer() == false)
        {
            currentState = enemyState.ready;
        }
    }

    bool IsPlayingAnimation(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
            return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlyer = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlyer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlyer = false;
        }
    }
}
