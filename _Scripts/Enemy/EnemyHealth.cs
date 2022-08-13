using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("RollType")]
    [SerializeField] Roll.rollType myRollType;
    [SerializeField] Flavor.flavorType myFlavorType;

    EnemyData enemyData;
    // ������� ���� serialized
    [SerializeField] bool isStunned;
    [SerializeField] bool isParried;
    [SerializeField] bool isBlocking;
    [SerializeField] bool knockBack;
    [SerializeField] bool isOnBouncer;

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

    [Header("On Bounder")]
    [SerializeField] float bouncerTime;
    float bouncerTimeCounter;
    Vector2 bouncerForce;

    [Header("Effects")]
    [SerializeField] GameObject dieEffect;
    [SerializeField] GameObject dieBones;
    [SerializeField] Transform dieEffectPoint;

    [Header("White Flash")]
    [SerializeField] GameObject mSprite;  // �ڽ��� ��������Ʈ�� ����� �ֱ�
    [SerializeField] Material whiteMat;
    [SerializeField] float blinkingDuration;
    Material initialMat;
    SpriteRenderer theSR;

    public bool IsfacingPlayer { get; set; }
    
    private void Start()
    {
        currentHP = maxHP;
        theSR = mSprite.GetComponent<SpriteRenderer>();
        theRB = GetComponentInParent<Rigidbody2D>();
        enemyData = GetComponentInParent<EnemyData>();
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
        if (isOnBouncer)
        {
            if (bouncerTimeCounter > 0)
            {
                bouncerTimeCounter -= Time.deltaTime;
                return;
            }
            bouncerTimeCounter = bouncerTime;
            isOnBouncer = false;
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
            GameManager.instance.StartCameraShake(4, .5f);
            Die();
        }

        if (collision.CompareTag("Explosion"))
        {
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
        if (isParried) // parried �� stunned�� ���� ���ÿ� �ɸ��� ��츦 �����ϱ� ����
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

        // ������ �� flavor�� ������ �ִ� ���̶�� flavor�� �ѱ�� ����, �׷��� ������ roll�� ��
        if (myFlavorType != Flavor.flavorType.none)
        {
            FlavorSo _flavorSo = RecipeFlavor.instance.GetFlavourSo(myFlavorType);
            PanManager.instance.AcquireFlavor(_flavorSo);
        }
        else
        {
            RollSO _rollSo = RecipeRoll.instance.GetRollSo(myRollType);
            GameObject _roll = Instantiate(_rollSo.rollPrefab[0], dieEffectPoint.position, transform.rotation);
            _roll.GetComponent<EnemyRolling>().m_RollSo = _rollSo;  // ��Ȱ�� �� ������ rollSo
        }
        
        Die();
    }

    // Player Capture Box�� ������
    public void PlayerCaptureBoxIn()
    {
        if (isBlocking)
        {
            SetBlockState(true);
            return;
        }
            
        if (canBlockCapture) // ĸ�ĸ� ����ϴ� ���ΰ�?
        {
            // �׷��ٸ� ���ϻ���, �и��� ���°� �� �� �ƴϸ鼭, �տ��� ���ݴ����� ��
            if (!isStunned && !isParried && IsfacingPlayer) 
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

        GetRolled(); // ĸ�ĸ� ����ϴ� ���� �ƴ϶�� �׳� GetRolled
    }
    public void OnBouncer(Vector2 _bouncerForceVector)
    {
        bouncerForce = _bouncerForceVector;
        isOnBouncer = true;
        bouncerTimeCounter = bouncerTime;
    }
    public Vector2 GetBouncerForce()
    {
        return bouncerForce;
    }
    public bool CheckIsOnBouncer()
    {
        return isOnBouncer;
    }

    public void Die()
    {
        Instantiate(dieBones, dieEffectPoint.position, transform.rotation);
        Instantiate(dieEffect, transform.position, transform.rotation);
        enemyData.PlayDieSound();
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

    // ��ü���� �����ϴ� �Լ���
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
            //theRB.bodyType = RigidbodyType2D.Kinematic;
            StartCoroutine(WhiteFlashCo());
            return;
        }
    }
    public void SetKnockBackState(bool state)
    {
        knockBack = state;
        if (knockBack)
        {
            //theRB.bodyType = RigidbodyType2D.Kinematic;
            StartCoroutine(WhiteFlashCo());
            return;
        }
    }
    public void SetBlockState(bool state)
    {
        isBlocking = state;
        if (isBlocking)
        {
            //theRB.bodyType = RigidbodyType2D.Kinematic;
            return;
        }
    }
    
}
