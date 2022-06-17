using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCoolTime;
    [SerializeField] float attackCoolTime;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask whatIsGround;
    bool isGrounded;
    bool isDetectingPlayer;

    float jumpCounter;
    float attackCounter;
    int currentIndex;
    Vector2 attackTarget; 

    SpriteRenderer theSR;


    void Start()
    {
        theSR = GetComponentInChildren<SpriteRenderer>();
        foreach (var points in patrolPoints)
        {
            points.transform.parent = null;
        }
        jumpCounter = jumpCoolTime;
    }

    void Update()
    {
        SetAttackTarget();
        Attack();
        GroundCheck();
        Patrol();
        CheckDirection();
        //Jump();
    }
    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentIndex].position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, patrolPoints[currentIndex].position) < .5f)
        {
            currentIndex++;

            if (currentIndex >= patrolPoints.Length)
            {
                currentIndex = 0;
            }
        }
    }

    void CheckDirection()
    {
        if (transform.position.x > patrolPoints[currentIndex].position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180f, 0);
        }
    }

    //void Jump()
    //{
    //    if (jumpCounter > 0)
    //    {
    //        jumpCounter -= Time.deltaTime;
    //        return;
    //    }
    //    if (isGrounded == false)
    //        return;
    //    jumpCounter = jumpCoolTime;
    //    theRB.velocity = new Vector2(theRB.velocity.x, -jumpForce);
    //}

    /// <summary>
    /// 플레이어를 처음 감지하게 되면 플레이어의 위치를 attackTarget에 저장한다
    /// </summary>
    void SetAttackTarget()
    {
        if (isDetectingPlayer == false)
            return;

        if (attackTarget == Vector2.zero)
        {
            attackTarget = PlayerController.instance.transform.position;
        }
    }
    void Attack()
    {
        if (attackCounter > 0)
        {
            attackCounter -= Time.deltaTime;
            return;
        }
            
        if (attackTarget == Vector2.zero)
            return;
        
        transform.position = Vector2.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, attackTarget) < 1f)
        {
            isDetectingPlayer = false;
            attackTarget = Vector2.zero;
            attackCounter = attackCoolTime;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, whatIsGround);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlayer = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetectingPlayer = false;
        }
    }
}
