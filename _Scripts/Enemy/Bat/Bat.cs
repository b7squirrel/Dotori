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
    [SerializeField] float pushedBackDistance; // blocking 때 밀려나는 거리
    [SerializeField] float pushedBackForce; // blocking 때 밀려나는 힘
    [SerializeField] GameObject blockingEffect;
    [SerializeField] bool isDetectingPlayer;  // serialized for debugging

    bool isFacingLeft;
    float attackCounter;
    int currentIndex;
    Vector2 attackTarget;
    float horizontalDirection; // 뒤로 밀려날 방향 체크를 위해
    Vector2 pushedBackPosition; // 어디까지 뒤로 물러날지 

    Animator anim;
    Rigidbody2D theRB;

    [Header("Stunned")]
    EnemyHealth enemyHealth;
    [SerializeField] GameObject captureBox;

    [Header("Debug")]
    [SerializeField] GameObject debugDot;

    enum EnemyState { patrol, attack, backToPatrol, stunned, block }
    [SerializeField] EnemyState currentState;  // 디버깅 목적으로 serialized


    void Start()
    {
        anim = GetComponent<Animator>();
        theRB = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        foreach (var points in patrolPoints)
        {
            points.transform.parent = null;
        }
    }

    void Update()
    {
        if (enemyHealth.IsBlocking())
        {
            CaptureBoxActive(false);
            currentState = EnemyState.block;
        }
        else if (enemyHealth.IsStunned())
        {
            CaptureBoxActive(false);
            currentState = EnemyState.stunned;
        }
        else
        {
            SetAttackTarget();
            CheckDirection();
            CheckIsFacingPlayer();
        }
        

        switch (currentState)
        {
            case EnemyState.patrol:
                Patrol();
                break;
            case EnemyState.attack:
                Attack();
                break;
            case EnemyState.backToPatrol:

                break;
            case EnemyState.stunned:
                Stunned();
                break;
            case EnemyState.block:
                Blocking();
                break;
        }
    }
    void Patrol()
    {
        if (!IsPlayingAnim("Walk"))
        {
            anim.Play("Walk");
        }
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
            isFacingLeft = true;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180f, 0);
            isFacingLeft = false;

        }
    }

    /// <summary>
    /// 블락하고 뒤로 밀려날 때 어느 방향으로 밀려날지 결정하는 함수
    /// </summary>
    void CheckDirectionToPlayer()
    {
        if (transform.position.x >= PlayerController.instance.transform.position.x)
        {
            horizontalDirection = 1;
        }
        else
        {
            horizontalDirection = -1;
        }
    }

    void CaptureBoxActive(bool _value)
    {
        captureBox.gameObject.SetActive(_value);
    }
    void SetAttackTarget()
    {
        if (isDetectingPlayer == false)
            return;

        if (IsPlayingAnim("ReturnToIdle"))
            return;

        if (attackTarget == Vector2.zero)
        {
            attackTarget = PlayerController.instance.transform.position;
            Instantiate(debugDot, attackTarget, Quaternion.identity);
            anim.Play("Attack");
            currentState = EnemyState.attack;
        }
    }
    void Stunned()
    {
        if (!IsPlayingAnim("Stunned"))
        {
            anim.Play("Stunned");
        }
        theRB.bodyType = RigidbodyType2D.Dynamic;
        theRB.gravityScale = 5f;
        theRB.velocity = new Vector2(0, theRB.velocity.y);

        if (enemyHealth.IsStunned() == false)
        {
            theRB.gravityScale = 0f;
            CaptureBoxActive(true);
            currentState = EnemyState.patrol;
        }
    }

    void Blocking()
    {
        if (!IsPlayingAnim("Block"))  // block 상태로 들어가기 전 한 번만 실행 (Enter blockState)
        {
            anim.Play("Block");
            CheckDirectionToPlayer();
            CheckDirection();
            enemyHealth.WhiteFlash();
            pushedBackPosition = (Vector2)transform.position + new Vector2(horizontalDirection * pushedBackDistance, 0);
            Instantiate(blockingEffect, transform.position, Quaternion.identity);
        }
        if (Vector2.Distance((Vector2)transform.position, pushedBackPosition) > .2)
        {
            transform.position = Vector2.MoveTowards(transform.position, pushedBackPosition, pushedBackForce * Time.deltaTime);
        }

        if (enemyHealth.IsBlocking() == false)
        {
            CaptureBoxActive(true);
            currentState = EnemyState.patrol;
        }
    }

    /// <summary>
    /// 플레이어를 처음 감지하게 되면 플레이어의 위치를 attackTarget에 저장한다
    /// </summary>
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
            if (IsPlayingAnim("Walk"))
            {
                anim.Play("Walk");
            }

            isDetectingPlayer = false;
            attackTarget = Vector2.zero;
            attackCounter = attackCoolTime;
            currentState = EnemyState.patrol;
        }
    }
    
    bool IsPlayingAnim(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            return true;
        }
        return false;
    }
    void CheckIsFacingPlayer()
    {
        if (transform.position.x < PlayerController.instance.transform.position.x
            && isFacingLeft == false)
        {
            enemyHealth.IsfacingPlayer = true;
            return;
        }
        else if (transform.position.x > PlayerController.instance.transform.position.x
            && isFacingLeft == true)
        {
            enemyHealth.IsfacingPlayer = true;
            return;
        }
        enemyHealth.IsfacingPlayer = false;
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
