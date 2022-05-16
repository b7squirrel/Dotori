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
    public float moveSpeed;
    private Vector2 moveDirection;
    private Rigidbody2D theRB;

    private Vector2 initialPoint; // parry 되었을 때 다시 되돌아 오기 위한 위치값
    public Vector2 contactPoint; // parry 된 지점을 시작점으로 하기 위한 변수
    public bool isParried; // projectile이 발사되었는지 parry 되었는지 판단
    public float homingTime; // 반사되어서 타겟에 도달하기까지 걸리는 시간
    private bool isFlying; // parry 되어서 날아가는 상태. 아무것도 안함. 

    public bool isCaptured; // 캡쳐되었음을 전달 받고 이 스크립트에서 getRolled를 구현
    public float gettingInSpeed; // 팬 위로 올라가는 속도

    public float deflectionDelayTime;
    bool isDelayed;

    public FlavorSo flavorSo;

    public GameObject deflectionHitEffect;
    public GameObject smokeRed;
    public GameObject debris;
    private GameObject _smoke;
    private GameObject _debris;

    private bool hit;

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        moveDirection = (PlayerController.instance.transform.position - transform.position).normalized * moveSpeed;
        initialPoint = new Vector2(transform.position.x, transform.position.y - 1f);
        isParried = false;
        isFlying = false;
        _smoke = Instantiate(smokeRed, transform.position, Quaternion.identity);
        _debris = Instantiate(debris, transform.position, Quaternion.identity);
    }

    void Update()
    {
        MoveParticles();
        PauseProjectileOnHit();

        if (isCaptured)
        {
            GetFlavored();
            DestroyProjectile();
        }
        else
        {
            if (!isFlying)
            {
                if (!isParried)
                {
                    theRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
                }
                else
                {
                    if (!hit)
                    {
                        hit = true;
                    }
                    isFlying = true;
                    AudioManager.instance.Play("pan_hit_05");
                    AudioManager.instance.Play("FIre_Parried");
                    theRB.gravityScale = 1f;
                    Instantiate(deflectionHitEffect, transform.position, Quaternion.identity);
                    StartCoroutine(DelayDeflection());

                    GameManager.instance.StartCameraShake(6, 1.3f);
                    GameManager.instance.TimeStop(.2f);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            DestroyProjectile();
        }
        if (collision.CompareTag("Ground"))
        {
            DestroyProjectile();
        }
        if (collision.CompareTag("Enemy"))
        {
            if (gameObject.CompareTag("ProjectileDeflected"))
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
        Destroy(_smoke);
        Destroy(_debris);
        Destroy(gameObject);
    }
    void MoveParticles()
    {
        _smoke.transform.position = Vector2.MoveTowards(_smoke.transform.position,
                transform.position, 5f);
        _debris.transform.position = Vector2.MoveTowards(_debris.transform.position,
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
        this.gameObject.tag = "ProjectileDeflected";
        yield return new WaitForSeconds(deflectionDelayTime);
        isDelayed = false;
        Deflection();
    }
    void Deflection()
    {
        Transform effectPoint = transform;
        effectPoint.position += new Vector3(2f, .7f, 0f);
        effectPoint.eulerAngles = new Vector3(transform.rotation.x, PlayerController.instance.transform.rotation.y, -10f);

        theRB.velocity = CalculateVelecity(initialPoint, (Vector2)contactPoint, homingTime);

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
}
