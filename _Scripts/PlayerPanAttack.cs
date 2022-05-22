using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanAttack : MonoBehaviour
{
    public static PlayerPanAttack instance;
    public LayerMask enemyLayers;
    public LayerMask rollLayers;
    public LayerMask projectileLayers;

    Animator panAnim;

    [Header("Pan Inventory")]
    PanManager panManager;
    [SerializeField] RollSO rollso;
    [SerializeField] FlavorSo flavorSo;

    [Header("Effects")]
    [SerializeField] GameObject HitRollEffect;
    [SerializeField] Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        panAnim = GetComponent<Animator>();
        panManager = GetComponentInChildren<PanManager>();
        captureBox.gameObject.SetActive(false);
        //boxCol = GetComponentInChildren<PlayerCaptureBox>().GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.Z))
                return;
            panAnim.Play("Pan_Capture");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (panManager.IsAvailableToCapture())
            {
                //panAnim.Play("Pan_HitRoll");
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (panManager.CountRollNumber() == 0)
                return;
            PanManager.instance.ClearRoll();
            EffectsClearRoll();
        }
    }

    // Capture의 마지막 프레임에서 애니메이션 이벤트로 실행
    void Panning()
    {
        if (PanManager.instance.CountRollNumber() == 0)
            return;
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
            return;
        panAnim.Play("Pan_Pan");
    }
    
    void CaptureBoxOn()
    {
        captureBox.gameObject.SetActive(true);
    }

    void CaptureBoxOff()
    {
        captureBox.gameObject.SetActive(false);
    }

    void EffectsClearRoll()
    {
        Instantiate(HitRollEffect, captureBox.position, Quaternion.identity);
        AudioManager.instance.Play("fire_explosion_01");
        AudioManager.instance.Play("pan_hit_03");
        GameManager.instance.StartCameraShake(8, .8f);
        GameManager.instance.TimeStop(.1f);
    }
}