using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoulFighter : MonoBehaviour
{
    enum enemyState { idle, follow, attackAntic, attack, stunned, parried, block };
    [SerializeField] private enemyState currentState;

    Animator anim;
    Rigidbody2D theRB;

    [Header("Detecting")]
    [SerializeField] Transform castPoint;
    [SerializeField] LayerMask action;
    [SerializeField] BoxCollider2D detectingArea;
    Vector2 _center, _size;
    [SerializeField] float _debugAlpha;

    bool canSeePlayer;

    [HideInInspector]
    bool isFacingLeft;
    bool isPlayerToLeft, wasPlayerToLeft;

    [Header("Follow")]
    [SerializeField] float moveSpeed;
    [SerializeField] float timeToStopFollowing;
    bool isDetecting; // 시야에서 사라진 플레이어를 쫒는 구간을 위한 플래그
    bool isSearching; // 플레이어가 시야에서 사라졌고 isDetecting도 false일 때 stopFollowingPlayer 함수를 계속 호출하려 들어가지 못하도록 하는 플래그
    bool isChangingDirection;

    [Header("Stunned")]
    EnemyHealth _enemyHealth;

    [Header("Attack")]
    [SerializeField] float attackDistnace;   // raycast 거리 설정
    [SerializeField] float attackCoolTime;
    [SerializeField] float attackStepForce;
    float attackCounter;

    [Header("HitBox")]
    [SerializeField] GameObject attackBox;

    private void Start()
    {
        anim = GetComponent<Animator>();
        theRB = GetComponent<Rigidbody2D>();
        _enemyHealth = GetComponentInChildren<EnemyHealth>();
        currentState = enemyState.follow;
        isDetecting = false;
        isFacingLeft = true;
        wasPlayerToLeft = isPlayerToLeft;
        attackBox.gameObject.SetActive(false);
    }
    bool IsPlayingAnim(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            return true;
        }
        return false;
    }

    private void Update()
    {
        SetStunnedState();
        SetParriedState();
        SetBlockState();
        DetectingArea();

        switch (currentState)
        {
            case enemyState.attack:
                if (IsPlayingAnim("Goul_Fighter_Attack"))
                    return;
                if (IsAttackRange() == false)
                {
                    if (!IsPlayingAnim("Goul_Fighter_Attack"))
                    {
                        // 공격 모션 중이라면 그 모션이 끝이 날 때까지 기다림
                        // Attack 모션은 끝이나면 walk 모션으로 들어감.
                        // walk모션이 재생되고 있지만 Follow가 아니라 attack모드이므로 제자리에서 걷는데 플레이어에게 겹칠만큼 다가가지 않기 때문에 좋다
                        currentState = enemyState.follow;
                    }
                    else
                    {
                        currentState = enemyState.attack;
                    }
                    return;
                }
                // 공격 모션 중이 아니면서 공격범위 안에 플레이어가 있다면
                if (attackCounter > 0f)
                {
                    attackCounter -= Time.deltaTime;
                }
                else
                {
                    attackCounter = attackCoolTime;
                    Attack();
                }
                break;

            case enemyState.follow:

                attackCounter = 0f; // canAttack 상태가 되었을 때 바로 공격할 수 있도록 미리 초기화 시켜둠

                if (canSeePlayer)
                {
                    isDetecting = true;
                }
                else
                {
                    if (isDetecting)  // 플레이어가 시야에서 사라졌지만 아직 플레이어를 느끼고 있다면
                    {
                        if (!isSearching)
                        {
                            // 플레이어가 시야에서 사라지더라도 당분간은 플레이어를 쫒아다니도록
                            isSearching = true;  // stopFollowingPlayer 코루틴으로 계속 들어가버리는 것을 방지
                            StartCoroutine(StopFollowingPlayer());
                        }
                    }
                }

                if (isDetecting)
                {
                    FollowPlayer();
                }

                if (IsAttackRange())
                {
                    currentState = enemyState.attack;
                }

                break;

            case enemyState.stunned:

                theRB.velocity = new Vector2(0, theRB.velocity.y);

                if (_enemyHealth.IsStunned() == false)
                {
                    currentState = enemyState.follow;
                }

                break;

            case enemyState.parried:

                theRB.velocity = new Vector2(0, theRB.velocity.y);

                if (_enemyHealth.IsParried() == false)
                {
                    currentState = enemyState.follow;
                }
                break;

            case enemyState.block:

                theRB.velocity = new Vector2(0, theRB.velocity.y);

                if (_enemyHealth.IsBlocking() == false)
                {
                    currentState = enemyState.follow;
                }
                break;
        }
    }

    Vector2 GetDirection()
    {
        if (isFacingLeft)
        {
            return new Vector2(-1, 0);
        }
        return new Vector2(1, 0);
    }
    void CheckIsFollowing()
    {
        if (IsPlayingAnim("Goul_Fighter_Walk"))
        {
            currentState = enemyState.follow;
        }
    }

    void SetStunnedState()
    {
        if (_enemyHealth.IsStunned())
        {
            if (!IsPlayingAnim("Goul_Fighter_Stunned"))
            {
                anim.Play("Goul_Fighter_Stunned");
            }
            AttackBoxOff();
            currentState = enemyState.stunned;
        }
    }

    void SetParriedState()
    {
        if (_enemyHealth.IsParried())
        {
            if (!IsPlayingAnim("Goul_Fighter_Parried"))
            {
                anim.Play("Goul_Fighter_Parried");
            }
            AttackBoxOff();
            currentState = enemyState.parried;
        }
    }

    void SetBlockState()
    {
        if (_enemyHealth.IsBlocking())
        {
            if (!IsPlayingAnim("Goul_Fighter_Block"))
            {
                anim.Play("Goul_Fighter_Block");
            }
            AttackBoxOff();
            currentState = enemyState.block;
        }
    }

    bool IsAttackRange()
    {
        float _castDistance = attackDistnace;
        bool _canAttackPlayer = false;

        if (isFacingLeft)
        {
            // 스프라이트가 뒤집히면 Line도 반대 방향으로 쏘도록
            _castDistance = -_castDistance;
        }

        Vector2 _endPosition = castPoint.position + Vector3.right * _castDistance;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, _endPosition, action);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("HurtBoxPlayer"))
            {
                _canAttackPlayer = true;
            }
            else
            {
                _canAttackPlayer = false;
            }

            Debug.DrawLine(castPoint.position, hit.point, Color.red); // 자신의 앞에 무엇인가를 감지하면 yellow
        }
        else
        {
            Debug.DrawLine(castPoint.position, _endPosition, Color.yellow); // 아무것도 감지하지 못할 때는 blue
        }

        anim.SetBool("canAttack", _canAttackPlayer); // 플레이어가 공격 가능한 범위에 있다면 attack 모션으로 가도록
        return _canAttackPlayer;
    }

    void Attack()
    {
        if (!IsPlayingAnim("Goul_Fighter_Stunned"))
        {
            anim.Play("Goul_Fighter_Attack");
        }
    }

    //animation event
    void AttackStep()
    {
        //theRB.AddForce(attackStepForce * GetDirection(), ForceMode2D.Impulse);
    }

    void AttackBoxOn()
    {
        attackBox.gameObject.SetActive(true);
    }

    void AttackBoxOff()
    {
        attackBox.gameObject.SetActive(false);
    }

    void FollowPlayer()
    {
        if (transform.position.x - PlayerController.instance.transform.position.x > 0)
        {
            isPlayerToLeft = true;
        }
        else if (transform.position.x - PlayerController.instance.transform.position.x < 0)
        {
            isPlayerToLeft = false;
        }

        if (isPlayerToLeft != wasPlayerToLeft)
        {
            isChangingDirection = true;
        }

        if (isChangingDirection)
        {
            StartCoroutine(DirectionChange());
        }
        else
        {
            if (transform.position.x < PlayerController.instance.transform.position.x)
            {
                // 플레이어의 왼쪽에 있으므로 오른쪽으로 이동해야 함, 오른쪽으로 돌아보도록, 오른쪽으로 보게 되므로 isFacingLeft = false
                isFacingLeft = false;
                anim.Play("Goul_Fighter_Walk");
                theRB.velocity = new Vector2(moveSpeed, theRB.velocity.y);
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (transform.position.x > PlayerController.instance.transform.position.x)
            {
                //플레이어의 오른쪽에 있으므로 왼쪽으로 이동해야 함, 왼쪽으로 돌아보도록
                isFacingLeft = true;
                anim.Play("Goul_Fighter_Walk");
                theRB.velocity = new Vector2(-moveSpeed, theRB.velocity.y);
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        wasPlayerToLeft = isPlayerToLeft;
    }

    IEnumerator DirectionChange()
    {
        //anim.Play("Goul_Turn");
        yield return new WaitForSeconds(.5f);
        isChangingDirection = false;
    }

    IEnumerator StopFollowingPlayer()
    {
        yield return new WaitForSeconds(timeToStopFollowing);
        theRB.velocity = new Vector2(0, theRB.velocity.y);
        anim.Play("Goul_Fighter_Idle");
        isDetecting = false;
        isSearching = false;
    }

    void DetectingArea()
    {
        _center = detectingArea.GetComponent<BoxCollider2D>().bounds.center;
        _size = detectingArea.GetComponent<BoxCollider2D>().bounds.size;

        bool _isDetectingPlayer = Physics2D.OverlapBox(_center, _size, 0f, action);
        if(_isDetectingPlayer)
        {
            canSeePlayer = true;
        }
        else
        {
            canSeePlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(detectingArea.GetComponent<BoxCollider2D>())
        {
            _center = detectingArea.GetComponent<BoxCollider2D>().bounds.center;
            _size = detectingArea.GetComponent<BoxCollider2D>().bounds.size;
            Gizmos.color = new Color(0, 1, 1, _debugAlpha);
            Gizmos.DrawCube(_center, _size);
        }
    }
}
