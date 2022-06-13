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
    public bool isFlavored;
    public FlavorSo m_flavorSO;  

    [Header("When Cleared")]
    public float horizontalSpeed;
    public float verticalSpeed;
    public float gravity;
    public PhysicsMaterial2D physicsMat;

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
    }

    void Update()
    {
        // Pan Manager의 ClearRoll에서 모든 롤의 tag를 Rolling으로 적용.
        if (this.tag == "Rolling" || this.tag == "RollFlavored")
        {
            currentState = rollingState.shooting;
        }
        switch (currentState)
        {
            case rollingState.shooting:
                CountDown();
                break;

            case rollingState.onPan:
                // 아무것도 하지 않음. PanSlot에서 AddRoll로 슬롯에 페어런트를 해버리기 때문
                break;
        }
        //CountDown();
    }
    
    // 만약 flavor가 있다면 폭발을 생성한다, 폭발을 생성할 때 크기를 결정하는 numberOfRoll을 Explosion Flavor에 넘겨준다
    
    void CountDown()
    {
        if (explodeCounter > 0)
        {
            explodeCounter -= Time.deltaTime;
            return;
        }
        Explode();
    }
    void Explode()
    {
        if (m_flavorSO != null)
        {
            var _explosion = Instantiate(m_flavorSO.actionPrefab, transform.position, transform.rotation);
            //_explosion.GetComponent<ExplosionFlavor>().numberOfRolls = 1;
        }
        DestroyPrefab();
    }
    public void DestroyPrefab()
    {
        Destroy(gameObject);
    }
}
