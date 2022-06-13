using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("RollType")]
    [SerializeField] Roll.rollType myRollType;

    bool isStunned;
    bool isParried;
    bool knockBack;

    [Header("HP")]
    [SerializeField] int maxHP;
    int currentHP;

    [Header("Stunned")]
    [SerializeField] float stunnedTime;
    float stunnedTImeCounter;

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
        initialMat = theSR.material;
        parriedTimeCounter = parriedTime;
        stunnedTImeCounter = stunnedTime;
    }

    private void Update()
    {
        if (isParried)
        {
            if (parriedTimeCounter > 0)
            {
                parriedTimeCounter -= Time.deltaTime;
            }
            else
            {
                parriedTimeCounter = parriedTime;
                SetParriedState(false);
            }
        }
        if (isStunned)
        {
            if (stunnedTImeCounter > 0)
            {
                stunnedTImeCounter -= Time.deltaTime;
            }
            else
            {
                stunnedTImeCounter = stunnedTime;
                SetStunState(false);
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackBoxPlayer"))
        {
            if(!isStunned)
            {
                AudioManager.instance.Play("pan_hit_05");
                TakeDamage();
            }
        }
        if (collision.CompareTag("ProjectileDeflected"))
        {
            AudioManager.instance.Play("Goul_Die_01");
            GameManager.instance.StartCameraShake(4, .5f);
            GameManager.instance.TimeStop(.2f);
            Die();
        }

        if (collision.CompareTag("Explosion"))
        {
            AudioManager.instance.Play("Goul_Die_01");
            Die();
        }

        if (collision.CompareTag("RollFlavored"))
        {
            SetStunState(true);
        }

        if (collision.CompareTag("Rolling"))
        {
            SetStunState(true);
        }
    }

    public void TakeDamage()
    {
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
    IEnumerator WhiteFlash()
    {
        theSR.material = whiteMat;
        yield return new WaitForSecondsRealtime(blinkingDuration);
        theSR.material = initialMat;
    }

    public bool IsStunned()
    {
        if (isStunned)
        {
            return true;
        }
        return false;
    }

    public bool IsParried()
    {
        if (isParried)
        {
            return true;
        }
        return false;
    }
    public bool IsKnockBacked()
    {
        if (knockBack)
        {
            return true;
        }
        return false;
    }

    public void SetStunState(bool state)
    {
        isStunned = state;
        isParried = false;
    }
    public void SetParriedState(bool state)
    {
        isParried = state;
        isStunned = false;
    }
    public void SetKnockBackState(bool state)
    {
        knockBack = state;
    }
}
