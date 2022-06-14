using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Attack Box에서 isParried 와 contactPoint를 제어함
/// Player Capture Box에서 isCaptured를 제어함
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    public bool IsParried { get; set; }  // Player Attack Box 로부터 패리되었음을 전달 받음.
    public bool IsCaptured { get; set; } // Player Capture Box 로부터 캡쳐되었음을 전달 받고 이 스크립트에서 getRolled를 구현
    public Vector2 ContactPoint { get; set; } // Player Parry Box 로부터 전달받음. parry 된 지점을 시작점으로 하기 위한 변수

    [SerializeField] float moveSpeed;
    [SerializeField] float deflectionSpeed;
    [SerializeField] float homingTime; // 반사되어서 타겟에 도달하기까지 걸리는 시간
    Vector2 moveDirection;
    Rigidbody2D theRB;
    Vector2 initialPoint; // parry 되었을 때 다시 되돌아 오기 위한 위치값
    bool isFlying; // parry 되어서 날아가는 상태. 아무것도 안함. 

    [SerializeField] float deflectionDelayTime;
    bool isDelayed;

    // Buffer Time
    bool isHittingPlayer;
    [SerializeField] float captureBufferTime;
    [SerializeField] float parriedBufferTime;
    float captureBufferCounter;
    float parriedBufferCounter;
    bool isCaptureBufferChecked;  // 한 번 버퍼가 체크되면 다시 CheckCaptureBuffer 함수 내의 플레이어를 죽이는 명령을 실행하지 않도록
    bool isParriedBufferChecked;  // 한 번 버퍼가 체크되면 다시 CheckParriedBuffer 함수 내의 플레이어를 죽이는 명령을 실행하지 않도록

    [SerializeField] FlavorSo flavorSo;
    [SerializeField] GameObject deflectionHitEffect;
    [SerializeField] GameObject fireParticle;
    [SerializeField] GameObject fireParticleCore;
    GameObject smoke;
    GameObject debris;

    [SerializeField] PlayerTargetController playerTargetController;

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        // 플레이어의 피봇이 bottom이므로 살짝 높은 곳을 향해 날아가게 한다
        moveDirection = (PlayerController.instance.transform.position - transform.position + new Vector3(0f, .7f, 0f)).normalized * moveSpeed;
        initialPoint = new Vector2(transform.position.x, transform.position.y - 1f);
        IsParried = false;
        isFlying = false;
        smoke = Instantiate(fireParticle, transform.position, Quaternion.identity);
        debris = Instantiate(fireParticleCore, transform.position, Quaternion.identity);
        captureBufferCounter = captureBufferTime;
        parriedBufferCounter = parriedBufferTime;
        playerTargetController = FindObjectOfType<PlayerTargetController>();
    }

    /// <summary>
    /// 캡쳐되었다면 Flavor시키고 파괴. 
    /// 반사되어 날아가는 중이라면 아무것도 하지 않음. 
    /// 패리되지 않았다면 플레이어를 향해 날아감. 
    /// 그 외는 패리되었다는 의미이므로 피드백을 생성
    /// </summary>
    void Update()
    {
        CheckCaptureBuffer();
        CheckParriedBuffer();

        Particles();
        PauseProjectileOnHit();

        if (IsCaptured)
        {
            GetFlavored();
            DestroyProjectile();
            return;
        }
        if (isFlying)
            return;
        if (IsParried == false)
        {
            theRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
            return;
        }
        FeedbackOnParried();
    }

    /// <summary>
    /// 플레이어의 Hurt Box에 닿았을 때 z캡쳐 여부를 보고 플레이어를 죽이거나 살림
    /// isHittingPlayer가 참일때만 capture buffer counter를 감소시킴. 
    /// </summary>
    void CheckCaptureBuffer()
    {
        if (isCaptureBufferChecked)
            return;
        if (isHittingPlayer == false)
            return;

        captureBufferCounter -= Time.deltaTime;

        if (captureBufferCounter > 0)
        {
            isCaptureBufferChecked = true;
            return;
        }
        PlayerHealthController.instance.isDead = true;
        DestroyProjectile();
    }
    /// <summary>
    /// 플레이어의 Hurt Box에 닿았을 때 패링 여부를 보고 플레이어를 죽이거나 살림
    /// isHittingPlayer가 참일때만 parried buffer counter를 감소시킴. 
    /// </summary>
    void CheckParriedBuffer()
    {
        if (isParriedBufferChecked)
            return;
        if (isHittingPlayer == false)
            return;

        parriedBufferCounter -= Time.deltaTime;

        if (parriedBufferCounter > 0)
        {
            isParriedBufferChecked = true;
            return;
        }
        PlayerHealthController.instance.isDead = true;
        DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            if (this.gameObject.CompareTag("ProjectileEnemy"))
                isHittingPlayer = true;
        }
        if (collision.CompareTag("Ground"))
        {
            DestroyProjectile();
        }
        if (collision.CompareTag("Enemy"))
        {
            if (this.gameObject.CompareTag("ProjectileDeflected"))
            {
                DestroyProjectile();
            }
        }
    }

    void GetFlavored()
    {
        AudioManager.instance.Play("GetRolled_01");
        PanManager.instance.AcquireFlavor(flavorSo);
    }
    private void DestroyProjectile()
    {
        // 이펙트 사운드 추가하기
        Destroy(smoke);
        Destroy(debris);
        Destroy(gameObject);
    }
    void Particles()
    {
        smoke.transform.position = Vector2.MoveTowards(smoke.transform.position,
                transform.position, 5f);
        debris.transform.position = Vector2.MoveTowards(debris.transform.position,
    transform.position, 5f);
    }
    void PauseProjectileOnHit()
    {
        if (isDelayed)
        {
            theRB.velocity = new Vector2(0, 0);
        }
    }
    IEnumerator DelayDeflection()
    {
        isDelayed = true;
        
        yield return new WaitForSeconds(deflectionDelayTime);
        isDelayed = false;
        Deflection();
    }
    void Deflection()
    {
        Transform effectPoint = transform;
        effectPoint.position += new Vector3(2f, .7f, 0f);
        effectPoint.eulerAngles = new Vector3(transform.rotation.x, PlayerController.instance.transform.rotation.y, -10f);

        //theRB.velocity = CalculateVelecity(initialPoint, (Vector2)ContactPoint, homingTime);
        Vector2 _mouseDirection = playerTargetController.GetMouseDirection();
        theRB.velocity = deflectionSpeed * _mouseDirection;
    }
    Vector2 CalculateVelecity(Vector2 _target, Vector2 _origin, float time)
    {
        Vector2 distance = _target - _origin;

        float Vx = distance.x / time;
        float Vy = distance.y / time + 0.5f * Mathf.Abs(Physics2D.gravity.y) * time;

        Vector2 result;
        result.x = Vx;
        result.y = Vy;

        return result;
    }
    void FeedbackOnParried()
    {
        isFlying = true;
        this.gameObject.tag = "ProjectileDeflected";
        AudioManager.instance.Play("pan_hit_05");
        AudioManager.instance.Play("FIre_Parried");
        theRB.gravityScale = 1f;
        Instantiate(deflectionHitEffect, transform.position, Quaternion.identity);
        StartCoroutine(DelayDeflection());

        GameManager.instance.StartCameraShake(6, 1.3f);
        GameManager.instance.TimeStop(.2f);
    }
}
