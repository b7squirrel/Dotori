using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornFly : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCoolTime;
    [SerializeField] float attackCoolTime;
    [SerializeField] float pushedBackDistance; // blocking �� �з����� �Ÿ�
    [SerializeField] float pushedBackForce; // blocking �� �з����� ��
    [SerializeField] GameObject blockingEffect;
    [SerializeField] bool isDetectingPlayer;  // serialized for debugging

    bool isFacingLeft;
    float attackCounter;
    int currentIndex;
    Vector2 attackTarget;
    float horizontalDirection; // �ڷ� �з��� ���� üũ�� ����
    Vector2 pushedBackPosition; // ������ �ڷ� �������� 

    Animator anim;
    Rigidbody2D theRB;

    [Header("Stunned")]
    EnemyHealth enemyHealth;
    [SerializeField] GameObject captureBox;

    [Header("Debug")]
    [SerializeField] GameObject debugDot;

    enum EnemyState { patrol, attack, backToPatrol, stunned, block }
    [SerializeField] EnemyState currentState;  // ����� �������� serialized


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
    /// �����ϰ� �ڷ� �з��� �� ��� �������� �з����� �����ϴ� �Լ�
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
        if (!IsPlayingAnim("Block"))  // block ���·� ���� �� �� ���� ���� (Enter blockState)
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
    /// �÷��̾ ó�� �����ϰ� �Ǹ� �÷��̾��� ��ġ�� attackTarget�� �����Ѵ�
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