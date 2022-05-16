using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanAttack : MonoBehaviour
{
    public static PlayerPanAttack instance;
    public Animator panAnim;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;
    public LayerMask rollLayers;

    public Transform hittingRollPoint;   // 이건 어디다 쓰는걸까?
    public Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    private PlayerCaptureBox playerCaptureBox;

    public float captureDuration;
    private float captureTimer;

    public Inventory inventory;
    public RollSO rollso;
    public FlavorSo flavorSo;

    public GameObject HitRollEffect;

    public float CaptureTimer
    {
        get { return captureTimer; }
        set { captureTimer = value; }
    }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        panAnim = GetComponent<Animator>();
        playerCaptureBox = GetComponentInChildren<PlayerCaptureBox>();
    }
    void Update()
    {
        if (captureTimer > 0f)
        {
            captureTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.Z))
            {
                return;
            }
            panAnim.Play("Pan_Capture");
            captureTimer = captureDuration;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (inventory.numberOfRolls > 0)
            {
                panAnim.Play("Pan_HitRoll");
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (PanManager.instance.CountRollNumber() == 0)
                return;
            PanManager.instance.ClearRoll();
            EffectsClearRoll();
        }
    }

    // enemy는 overlapBox로 캡쳐하고 projectile은 playercaptureBox에서 ontriggerenter2d로 감지해서 캡쳐함
    // 오버랩은 너무 순간이라서 projectile을 잡기에 적합하지 않음
    // 캡쳐 함수는 애니메이션 이벤트로 실행
    void Capture()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(playerCaptureBox.boxCol.bounds.center, playerCaptureBox.boxCol.bounds.size, 0, enemyLayers);
        if (hits != null)
        {
            foreach (Collider2D enemy in hits)
            {
                if (PanManager.instance.IsAvailableToCapture())  // Roll이 슬롯 갯수보다 작으면 캡쳐실행
                {
                    EnemyHealth _enemyHealth = enemy.GetComponent<EnemyHealth>();
                    
                    if (_enemyHealth != null)
                    {
                        _enemyHealth.GetRolled();
                        //if (_enemyHealth.IsStunned())  // 스턴 상태일 때만 캡쳐가능
                        //{
                        //    _enemyHealth.GetRolled();
                        //}
                    }
                }
            }
        }
        PanManager.instance.FlipRoll();
        //PanManager.instance.SwitchRolls();
    }
    void Panning()
    {
        // Capture의 마지막 프레임에서 애니메이션 이벤트로 실행
        if (!panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
        {
            if (inventory.numberOfRolls == 1)
            {
                panAnim.Play("Pan_Pan");
            }
            else if (inventory.numberOfRolls == 2)
            {
                panAnim.Play("Pan_Pan2");
            }
            else if (inventory.numberOfRolls == 3)
            {
                panAnim.Play("Pan_Pan3");
            }
            //if (inventory.InputSlots[0].GetRoll().rollSo.rollType != Roll.rollType.none)
            //{
            //    panAnim.Play("Pan_Pan");
            //}
        }
    }

    void EffectsClearRoll()
    {
        AudioManager.instance.Play("fire_explosion_01");
        AudioManager.instance.Play("pan_hit_03");
        GameManager.instance.StartCameraShake(8, .8f);
        GameManager.instance.TimeStop(.1f);
    }
}