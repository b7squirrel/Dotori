using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("RollType")]
    [SerializeField] Roll.rollType myRollType;

    // 디버깅을 위해 serialized
    [SerializeField] bool isStunned;
    [SerializeField] bool isParried;
    [SerializeField] bool isBlocking;
    [SerializeField] bool knockBack;

    [Header("HP")]
    [SerializeField] int maxHP;
    int currentHP;

    [Header("Can this enemy block Capture?")]
    [SerializeField] bool canBlockCapture;
    [SerializeField] float blockTime;
    float blockTimeCounter;

    [Header("Stunned")]
    [SerializeField] float stunnedTime;
    float stunnedTImeCounter;
    Rigidbody2D theRB;

    [Header("Parried")]
    [SerializeField] float parriedTime;
    float parriedTimeCounter;

    [Header("Effects")]
    [SerializeField] GameObject dieEffect;
    [SerializeField] GameObject dieBones;
    [SerializeField] Transform dieEffectPoint;

    [Header("White Flash")]
    [SerializeField] GameObject mSprite;  // 자신의 스프라이트를 끌어다 넣기
    [SerializeField] Material whiteMat;
    [SerializeField] float blinkingDuration;
    Material initialMat;
    SpriteRenderer theSR;
    
    private void Start()
    {
        currentHP = maxHP;
        theSR = mSprite.GetComponent<SpriteRenderer>();
        theRB = GetComponentInParent<Rigidbody2D>();
        initialMat = theSR.material;
        parriedTimeCounter = parriedTime;
        stunnedTImeCounter = stunnedTime;
        blockTimeCounter = blockTime;
    }

    private void Update()
    {
        if (isParried)
        {
            if (parriedTimeCounter > 0)
            {
                parriedTimeCounter -= Time.deltaTime;
                return;
            }
            parriedTimeCounter = parriedTime;
            SetParriedState(false);
        }
        if (isStunned)
        {
            if (stunnedTImeCounter > 0)
            {
                stunnedTImeCounter -= Time.deltaTime;
                return;
            }
            stunnedTImeCounter = stunnedTime;
            SetStunState(false);
        }
        if (isBlocking)
        {
            if (canBlockCapture == false)
                return;
            if (blockTimeCounter > 0)
            {
                blockTimeCounter -= Time.deltaTime;
                return;
            }
            blockTimeCounter = blockTime;
            SetBlockState(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackBoxPlayer"))
        {
            if(!isStunned)
            {
                TakeDamage();
            }
        }
        if (collision.CompareTag("ProjectileDeflected"))
        {
            AudioManager.instance.Play("Goul_Die_01");
            GameManager.instance.StartCameraShake(4, .5f);
            Die();
        }

        if (collision.CompareTag("Explosion"))
        {
            AudioManager.instance.Play("Goul_Die_01");
            Die();
        }

        if (collision.CompareTag("RollFlavored"))
        {
            TakeDamage();
        }

        if (collision.CompareTag("Rolling"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isParried) // parried 와 stunned가 거의 동시에 걸리는 경우를 배제하기 위해
            return;
        
        AudioManager.instance.Play("pan_hit_05");
        SetStunState(true);
        SetKnockBackState(true);
    }
    public void GetRolled()
    {
        AudioManager.instance.Stop("Energy_01");
        AudioManager.instance.Play("GetRolled_01");

        isStunned = false;

        RollSO _rollSo = RecipeRoll.instance.GetRollSo(myRollType);
        Instantiate(_rollSo.rollPrefab[0], dieEffectPoint.position, transform.rotation);
        Die();
    }

    // Player Capture Box가 닿으면
    public void PlayerCaptureBoxIn()
    {
        if (isBlocking)
        {
            SetBlockState(true);
            return;
        }
            
        if (canBlockCapture) // 캡쳐를 블락하는 적인가?
        {
            if (!isStunned && !isParried) // 그렇다면 스턴상태, 패리된 상태가 둘 다 아닐때만 블락
            {
                SetBlockState(true);
                return;
            }
            else
            {
                GetRolled();
                return;
            }
        }

        GetRolled(); // 캡쳐를 블락하는 적이 아니라면 그냥 GetRolled
    }

    public void Die()
    {
        Instantiate(dieBones, dieEffectPoint.position, transform.rotation);
        Instantiate(dieEffect, transform.position, transform.rotation);
        AudioManager.instance.Play("Goul_Die_01");
        AudioManager.instance.Stop("Energy_01");
        currentHP = maxHP;
        isStunned = false;
        Destroy(transform.parent.gameObject);
    }
    IEnumerator WhiteFlashCo()
    {
        theSR.material = whiteMat;
        yield return new WaitForSecondsRealtime(blinkingDuration);
        theSR.material = initialMat;
    }

    public void WhiteFlash()
    {
        StartCoroutine(WhiteFlashCo());
    }

    // 본체에서 참조하는 함수들
    public bool IsStunned()
    {
        if (isStunned)
            return true;
        return false;
    }
    public bool IsParried()
    {
        if (isParried)
            return true;
        return false;
    }
    public bool IsKnockBacked()
    {
        if (knockBack)
            return true;
        return false;
    }
    public bool IsBlocking()
    {
        if (isBlocking)
            return true;
        return false;
    }
    public void SetStunState(bool state)
    {
        isStunned = state;
        
        if (isStunned)
        {
            isParried = false;
            knockBack = false;
            isBlocking = false;

            StartCoroutine(WhiteFlashCo());
            return;
        }
        theRB.bodyType = RigidbodyType2D.Dynamic;
    }
    public void SetParriedState(bool state)
    {
        isParried = state;
        if (isParried)
        {
            theRB.bodyType = RigidbodyType2D.Kinematic;
            StartCoroutine(WhiteFlashCo());
            return;
        }
    }
    public void SetKnockBackState(bool state)
    {
        knockBack = state;
        if (knockBack)
        {
            theRB.bodyType = RigidbodyType2D.Kinematic;
            StartCoroutine(WhiteFlashCo());
            return;
        }
    }
    public void SetBlockState(bool state)
    {
        isBlocking = state;
        if (isBlocking)
        {
            theRB.bodyType = RigidbodyType2D.Kinematic;
            return;
        }
    }
    
}
