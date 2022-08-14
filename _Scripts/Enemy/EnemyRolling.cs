using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인스펙터에서 편하게 드래그해서 넣으려고 Prefab > Enemy > Rolls > Rolls 폴더에 스크립트가 있음
/// </summary>
public class EnemyRolling : MonoBehaviour
{
    enum rollingState { shooting, captured, onPan };
    rollingState currentState;

    /// <summary>
    /// projectile을 캡쳐하면 EnemyProjectile이 PanManager의 acquireFlavor실행
    /// 그 projectile의 flavorSO를 롤의 m_flavorSO에 전달
    /// 그러면 나중에 쳐내면 EnemyRolling는 RollType, FlavorType을 모두 가지고 있게 된다.
    /// </summary>
    public bool IsFlavored { get; set; }
    public FlavorSo m_FlavorSO { get; set; }
    public RollSO m_RollSo { get; set; }

    [Header("When Cleared")]
    public float horizontalSpeed;
    public float verticalSpeed;
    public float mass;
    public float gravity;
    public PhysicsMaterial2D physicsMat;

    [Header("When Dropped")]
    public float verticalDropSpeed;
    public float dropGravity;

    [Header("Resurrection")]
    [SerializeField] float resurrectionTime;
    float resurrentionTimeCounter;

    [Header("Die")]
    [SerializeField] bool dieOnCollision;
    [SerializeField] float dieTime;
    float dieTimeCounter;
    bool isDead;

    [Header("Hit Effects")]
    [SerializeField] GameObject hitEffect;
    [SerializeField] Transform hitEffectPoint;

    [Header("Time to Explode")]
    [SerializeField] float timeToExplode;
    float explodeCounter;

    // 캡쳐되는 순간 pan manager에서 acquireRoll이 실행해서 빈 슬롯을 검색하고 add시킴
    void Start()
    {
        currentState = rollingState.onPan;
        this.tag = "RollsOnPan";
        PanManager.instance.AcquireRoll(transform);
        explodeCounter = timeToExplode;
        resurrentionTimeCounter = resurrectionTime;
        dieTimeCounter = dieTime;
    }

    void Update()
    {
        
        // Pan Manager의 ClearRoll에서 모든 롤의 tag를 Rolling으로 적용.
        // Pna Manager에서 ClearRoll, DropRoll, DropAllRolls 등으로 shoot 상태로 들어옴
        
        if (this.tag == "Rolling" || this.tag == "RollFlavored")
        {
            currentState = rollingState.shooting;
        }
        switch (currentState)
        {
            case rollingState.shooting:
                CountDownExplosion();
                //CountDownResurrection();
                CountDownToDie();
                break;

            case rollingState.onPan:
                // 아무것도 하지 않음. PanSlot에서 AddRoll로 슬롯에 페어런트를 해버리기 때문
                break;
        }
        //CountDown();
    }

    /// <summary>
    /// pan manager에서 Roll에 Flavor를 줄 때 호출
    /// </summary>
    public void SetFlavor(bool _isflavored, FlavorSo _m_flavorSo)
    {
        IsFlavored = _isflavored;
        m_FlavorSO = _m_flavorSo;
    }

    /// <summary>
    /// 폭발을 생성한다, 폭발을 생성할 때 크기를 결정하는 numberOfRoll을 Explosion Flavor에 넘겨준다
    /// Flavor가 없다면 폭발이 일어나지 않는다
    /// </summary>
    void CountDownExplosion()
    {
        if (explodeCounter > 0)
        {
            explodeCounter -= Time.deltaTime;
            return;
        }
        Explode();
    }
    void CountDownResurrection()
    {
        if (resurrentionTimeCounter > 0)
        {
            resurrentionTimeCounter -= Time.deltaTime;
            return;
        }
        Resurrection();
    }
    void CountDownToDie()
    {
        if (dieTimeCounter > 0)
        {
            dieTimeCounter -= Time.deltaTime;
            return;
        }
        DestroyPrefab();
    }
    void Explode()
    {
        if (m_FlavorSO == null)
            return;
        if (m_FlavorSO.flavorType == Flavor.flavorType.none)
            return;

        FlavorSo _flavorSo = RecipeFlavor.instance.GetFlavourSo(m_FlavorSO.flavorType);
        Instantiate(_flavorSo.actionPrefab, transform.position, transform.rotation);
        DestroyPrefab();
    }
    void Resurrection()
    {
        // 부활 이펙트, 애니메이션

        Instantiate(m_RollSo.enemyPrefab, transform.position, Quaternion.identity);
        DestroyPrefab();
    }

    void DestroyPrefab()
    {
        if (isDead)
            return;
        Instantiate(m_RollSo.fragmentPrefab, transform.position, Quaternion.identity);
        isDead = true;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dieOnCollision == false)
            return;
        if (collision.CompareTag("Ground") 
            || collision.CompareTag("HurtBoxPlayer") 
            || collision.CompareTag("Enemy"))
        {
            DestroyPrefab();
        }
    }
}
