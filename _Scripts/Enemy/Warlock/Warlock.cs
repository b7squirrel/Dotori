using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : MonoBehaviour
{
    public float moveSpeed, retreatSpeed;

    private bool isDetecting;  //플레이어를 감지. 가장 넓은 구간
    private bool isFacingRight;

    public float shootCoolTime;
    private float shootCounter;
    public Transform castingPoint;
    //public GameObject energy;
    public Transform castingRayCastPoint;
    public float distanceToRetreat;
    public LayerMask playerMask;

    private bool detectingPlayer;   //retreat해야 하는 지점까지 플레이어가 들어왔을 때
    public float retreatCoolTime;
    private float retreatCounter;
    private Vector2 whereToRetreat;
    public Transform retreatPoint;
    public float jumpForce;
    private bool isRetreating;

    public Transform detectingWallPoint; // 뒤에 벽이 있으면 점프하지 않도록
    public float distanceToWall;
    private RaycastHit2D hitWall;
    public LayerMask groundMask;
    private bool detectingWall;

    public GameObject projectile;
    public float shootAnticTime;

    private Rigidbody2D theRB;
    private Animator anim;
    private RaycastHit2D hit;

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        retreatCounter = 0f;
    }
    void Update()
    {
        if(GetComponentInChildren<EnemyHealth>().IsStunned())  // 스턴 상태라면 계속 이 반복문에 머물도록
        {
            anim.Play("Warlock_Stunned");
        }
        else // 스턴 상태가 아니라면 아래를 실행
        {
            CheckingDistance();
            DetectingPlayer();
            DetectingWall();
            Retreat();

            if (isDetecting)
            {
                Direction();

                if (shootCounter < 0)
                {
                    StartCoroutine(Shoot());
                    shootCounter = shootCoolTime;
                }
                else
                {
                    shootCounter -= Time.deltaTime;
                }
            }
        }
    }
    void ResetStunnedState()
    {
        //animation event
        GetComponentInChildren<EnemyHealth>().SetStunState(false);
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
    IEnumerator Shoot()
    {
        anim.Play("Warlock_Attack");
        AudioManager.instance.Stop("Energy_01"); // 이전에 재생되고 있는 에너지 사운드를 중단
        AudioManager.instance.Play("Energy_01");
        yield return new WaitForSeconds(shootAnticTime);
        AudioManager.instance.Stop("Energy_01");
        AudioManager.instance.Play("FireSpell_01");
        Instantiate(projectile, castingPoint.position, Quaternion.identity);
    }
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
    void DetectingPlayer()
    {
        if (retreatCounter <= 0)
        {
            if (detectingPlayer)
            {
                if (!detectingWall)
                {
                    isRetreating = true;
                    retreatCounter = retreatCoolTime;
                    whereToRetreat = retreatPoint.position;
                    detectingPlayer = false;
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce); // 플레이어를 감지하면 y축 초기속도로 한 번 힘을 가해줌. 
                }
            }
        }
        else
        {
            retreatCounter -= Time.deltaTime;
        }
    }
    void Retreat()
    {
        if (isRetreating)
        {
            transform.position = Vector2.MoveTowards(transform.position, whereToRetreat, retreatSpeed * Time.deltaTime);

            if (Mathf.Abs(Vector2.Distance(transform.position, whereToRetreat)) < .5f)
            {
                isRetreating = false;
            }
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
