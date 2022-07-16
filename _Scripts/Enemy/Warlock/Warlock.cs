using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : MonoBehaviour
{
    EnemyHealth enemyHealth;

    bool isDetecting;  //플레이어를 감지. 가장 넓은 구간
    bool isFacingRight;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float retreatSpeed;


    [Header("Attack")]
    [SerializeField] Transform castingPoint; // projectile을 생성할 곳
    [SerializeField] Transform castingRayCastPoint;
    [SerializeField] float distanceToRetreat;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float shootCoolTime;
    float shootCounter;

    [Header("Retreat")]
    [SerializeField] Transform retreatPoint;
    [SerializeField] float jumpForce;
    [SerializeField] float retreatCoolTime;
    Vector2 whereToRetreat;
    float retreatCounter;
    bool detectingPlayer;   //retreat해야 하는 지점까지 플레이어가 들어왔을 때
    bool canRetreat;

    [SerializeField] Transform detectingWallPoint; // 뒤에 벽이 있으면 점프하지 않도록
    [SerializeField] float distanceToWall;
    [SerializeField] LayerMask groundMask;
    RaycastHit2D hitWall;
    bool detectingWall;

    [SerializeField] GameObject projectile;
    [SerializeField] float shootAnticTime;

    Rigidbody2D theRB;
    Animator anim;
    RaycastHit2D hit;

    enum EnemyState { attack, retreat, idle, stunned}
    [SerializeField] EnemyState currentState; // 디버그 목적으로 serialized

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        anim = GetComponent<Animator>();
        retreatCounter = 0f;
        currentState = EnemyState.idle;
    }
    void Update()
    {
        if (enemyHealth.IsStunned())
        {
            currentState = EnemyState.stunned;
        }
        else
        {
            Direction();
            AttackCoolTIme();
            CheckingDistance();
            DetectingWall();

            DetectingPlayer();
        }

        // 모든 상태는 끝나면 Idle상태로 들어감. 
        switch (currentState)
        {
            case EnemyState.attack:
                PlayAnimation("Warlock_Attack");
                break;

            case EnemyState.retreat:
                Retreat();
                break;

            case EnemyState.idle:

                PlayAnimation("Warlock_Idle");
                if (canRetreat)
                {
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce); // 플레이어를 감지하면 y축 초기속도로 한 번 힘을 가해줌. 
                    currentState = EnemyState.retreat;
                }
                else if (isDetecting && shootCounter <= 0)
                {
                    currentState = EnemyState.attack;
                }
                break;

            case EnemyState.stunned:
                anim.Play("Warlock_Stunned");
                theRB.velocity = new Vector2(0, theRB.velocity.y);

                if (enemyHealth.IsStunned() == false)
                {
                    currentState = EnemyState.idle;
                }
                break;
        }
    }

    /// <summary>
    /// 벽을 감지하면 그냥 idle 상태로
    /// retreat point로 이동하면 idle 상태로
    /// </summary>
    void Retreat()
    {
        if (detectingWall)
        {
            canRetreat = false;
            retreatCounter = retreatCoolTime;
            currentState = EnemyState.idle;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, whereToRetreat, retreatSpeed * Time.deltaTime);

        if (Mathf.Abs(Vector2.Distance(transform.position, whereToRetreat)) < .5f || detectingWall)
        {
            canRetreat = false;
            retreatCounter = retreatCoolTime;
            currentState = EnemyState.idle;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetecting = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetecting = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            isDetecting = false;
        }
    }
    void Direction()
    {
        
        if (PlayerController.instance.transform.position.x - transform.position.x < 0)
        {
            isFacingRight = false;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (PlayerController.instance.transform.position.x - transform.position.x > 0)
        {
            isFacingRight = true;
            transform.eulerAngles = new Vector3(0, 180f, 0);
        }
    }
    void AttackCoolTIme()
    {
        if (shootCounter > 0)
        {
            shootCounter -= Time.deltaTime;
        }
    }

    void AnticShoot() // animation event
    {
        
        AudioManager.instance.Stop("Energy_01"); // 이전에 재생되고 있는 에너지 사운드를 중단
        AudioManager.instance.Play("Energy_01");
    }
    void Shoot() // aniamtion event
    {
        AudioManager.instance.Stop("Energy_01");
        AudioManager.instance.Play("FireSpell_01");
        Instantiate(projectile, castingPoint.position, Quaternion.identity);
    }
    void ExitAttack() // animation event
    {
        shootCounter = shootCoolTime;
        currentState = EnemyState.idle;
    }
    
    // retreat할 만큼 player가 가까이 왔는지 체크
    void CheckingDistance()
    {
        float _retreatDistance = distanceToRetreat;

        if (isFacingRight)
        {
            _retreatDistance = -_retreatDistance;
        }
        Vector2 _endPoint = (Vector2)castingRayCastPoint.position + Vector2.left * _retreatDistance;
        hit = Physics2D.Linecast(castingRayCastPoint.position, _endPoint, playerMask);

        if (hit)
        {
            detectingPlayer = true;
            Debug.DrawLine(castingRayCastPoint.position, hit.point, Color.yellow);
        }
        else
        {
            detectingPlayer = false;
            Debug.DrawLine(castingRayCastPoint.position, _endPoint, Color.blue);
        }
    }
    void DetectingWall()
    {
        float _distanceToWall = distanceToWall;
        if (isFacingRight)
        {
            _distanceToWall = -_distanceToWall;
        }
        Vector2 _endPoint = (Vector2)detectingWallPoint.position + Vector2.right * _distanceToWall;
        hitWall = Physics2D.Linecast(detectingWallPoint.position, _endPoint, groundMask);

        if (hitWall)
        {
            detectingWall = true;
            Debug.DrawLine(detectingWallPoint.position, hitWall.point, Color.yellow);
        }
        else
        {
            detectingWall = false;
            Debug.DrawLine(detectingWallPoint.position, _endPoint, Color.blue);
        }
    }
    // 
    /// <summary>
    /// Retreat을 하기 위한 조건들 검사
    /// retreat 쿨타임이 차지 않았거나, 플레이어를 감지하지 못했거나, 뒤에 벽이 있다면 retreat하지 않음
    /// </summary>
    void DetectingPlayer()
    {
        if (retreatCounter > 0)
        {
            retreatCounter -= Time.deltaTime;
            return;
        }
        if (detectingPlayer == false)
            return;
        if (detectingWall)
            return;

        retreatCounter = retreatCoolTime;
        whereToRetreat = retreatPoint.position;
        
        canRetreat = true;
    }
    
    void PlayAnimation(string _animation)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            anim.Play(_animation);
        }
    }

    void Gravity()
    {
        if (theRB.velocity.y > 0)
        {
            theRB.gravityScale = 5f;
        }
        else if (theRB.velocity.y < 0)
        {
            theRB.gravityScale = 8f;
        }
    }
}
