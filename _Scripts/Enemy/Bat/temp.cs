using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCoolTime;
    [SerializeField] float attackCoolTime;
    bool isDetectingPlayer;

    float jumpCounter;
    float attackCounter;
    int currentIndex;
    Vector2 attackTarget;

    SpriteRenderer theSR;
    Animator anim;
    Rigidbody2D theRB;

    [Header("Stunned")]
    EnemyHealth enemyHealth;
    [SerializeField] GameObject captureBox;

    [Header("Debug")]
    [SerializeField] GameObject debugDot;

    enum EnemyState { patrol, attack, backToPatrol, stunned }
    EnemyState currentState;


    void Start()
    {
        theSR = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        theRB = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        foreach (var points in patrolPoints)
        {
            points.transform.parent = null;
        }
        jumpCounter = jumpCoolTime;
    }

    void Update()
    {
        CheckIsStunned();
        SetAttackTarget();
        CheckDirection();

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
        }
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

    void CheckIsStunned()
    {
        if (enemyHealth.IsStunned())
        {
            if (!IsPlayingAnim("Stunned"))
            {
                anim.Play("Stunned");
            }
            CaptureBoxActive(false);
            currentState = EnemyState.stunned;
        }
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
        }
    }
    void Stunned()
    {
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
        }
    }

    void CaptureBoxActive(bool _value)
    {
        captureBox.gameObject.SetActive(_value);
    }
    bool IsPlayingAnim(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            return true;
        }
        return false;
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
