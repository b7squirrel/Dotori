using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCapture : MonoBehaviour
{
    [Header("Pan Inventory")]
    Animator panAnim;
    PanManager panManager;
    PlayerTargetController playerTargetController;
    PlayerController playerController;

    [SerializeField] RollSO rollso;
    [SerializeField] FlavorSo flavorSo;

    [Header("Capture")]
    [SerializeField] float captureBoxOnTime;  // 캡쳐박스가 활성화 되는 시간

    [Header("Toss Rolls")]
    [SerializeField] SlotPhysics slotPhysicsSet;
    [SerializeField] float lengthSlowMotion;
    [SerializeField] float startSlowMotionTImeOffset; // Toss anim 시작 후 얼마 후에 슬로우모션 시작할지
    bool isTossing;  // 참일 떄만 슬로우모션이 발동되도록 (capture할 때)
    public bool IsCapturing { get; private set; } // player controller에서 움직임을 제어하기 위해 public

    // player target controller에서 계속 업데이트 되는 horizontal direction을 가져와서 캡쳐를 시작할 때 고정
    public float CaptureDirection; 
    public float CaptureDashSpeed;

    [Header("Effects")]
    [SerializeField] GameObject HitRollEffect;
    [SerializeField] Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    [Header("Debug")]
    [SerializeField] bool _isTossing;
    [SerializeField] bool _isCapturing;

    [SerializeField] bool IsSlowMotion; // 슬로우 모션 상태가 되는 조건을 만족했을 때
    [SerializeField] bool OnSlowMotion; // 슬로우 모션이 진행 중일 떄 (슬로우 모션 코루틴을 계속 실행시키지 않기 위해)

    void Start()
    {
        panAnim = GetComponent<Animator>();
        panManager = GetComponentInChildren<PanManager>();
        playerTargetController = GetComponentInParent<PlayerTargetController>();
        playerController = GetComponentInParent<PlayerController>();
        captureBox.gameObject.SetActive(false);
    }
    void Update()
    {
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Toss") == false)
        {
            isTossing = false;
        }
        Capture();
        HitRolls();
        //DodgeTurnPan();
        SlowMotion();
        Debugging();
    }

    private void Debugging()
    {
        _isCapturing = IsCapturing;
        _isTossing = isTossing;
    }

    void Capture()
    {
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Capture") == false)
        {
            IsCapturing = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //if (Input.GetKey(KeyCode.Z) || Input.GetMouseButtonDown(0))  // 캡쳐 캔슬
            //    return;
            if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Capture"))
                return;
            
            
            if (slotPhysicsSet.IsAnchorGrounded)  // 슬롯이 팬에 붙어 있다면 롤이 아래로 떨어지도록 속도 대입
            {
                slotPhysicsSet.TossRolls();
            }
            if (slotPhysicsSet.IsRollsOnPan == false) // 롤이 없다면 바로 캡쳐를 하도록
            {
                IsCapturing = true;
            }
            CaptureDirection = playerTargetController.GetMouseHorizontalDirection();
            Toss(false);
            panAnim.Play("Pan_Capture");
        }
    }
    /// <summary>
    /// 자동으로 토스가 됨
    /// </summary>
    public void Toss(bool _slowMotion)
    {
        //if (slotPhysicsSet.IsRollsOnPan == false) // 슬롯위에 롤이 없으면 Toss 입력 무시
        //    return;

        if (slotPhysicsSet.IsAnchorGrounded)  // 슬롯이 팬에 붙어 있다면 위쪽으로 올라가는 속도 대입
        {
            
            slotPhysicsSet.TossRolls();
        }
        if (_slowMotion)
        {
            StartSlowMotion();
        }
    }
    void SlowMotion()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsSlowMotion = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            IsSlowMotion = false;
            OnSlowMotion = false;
            SlowMotionManager.instance.StopSlowMotion();
        }
        if (IsSlowMotion == true && OnSlowMotion == false) //슬로우모션 조건을 만족하고 아직 슬로우 모션이 실행중이 아니라면
        {
            SlowMotionManager.instance.StartSlowMotion();
            OnSlowMotion = true;
        }
    }

    void DodgeTurnPan()
    {
        if (playerController.IsDodging)
        {
            panAnim.Play("Pan_DodgeTurn");
        }
    }

    void PanState()
    {

    }

    // animation events
    //void ExitDodgeTurnPan()
    //{
    //    if (slotPhysicsSet.IsRollsOnPan)
    //    {
    //        panAnim.Play("Pan_Pan");
    //    }
    //    else
    //    {
    //        panAnim.Play("Pan_Idle");
    //    }
    //}
    void HitRolls()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(1))
        {
            if (panManager.CountRollNumber() == 0)
                return;
            if (slotPhysicsSet.IsRollsOnPan == false)
                return;
            panAnim.Play("Pan_HitRoll");
            panManager.ClearRoll();
            EffectsClearRoll();
        }
    }
    
    // Capture의 마지막 프레임에서 애니메이션 이벤트로 실행
    void Panning()
    {
        if (panManager.CountRollNumber() == 0)
            return;
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
            return;
        panAnim.Play("Pan_Pan");
    }
    void CaptureBoxOn()
    {
        captureBox.gameObject.SetActive(true);
        StartCoroutine(DeactivateCaptureBox());
    }
    void CaptureBoxOff()
    {
        captureBox.gameObject.SetActive(false);
    }
    IEnumerator DeactivateCaptureBox()
    {
        yield return new WaitForSeconds(captureBoxOnTime);
        CaptureBoxOff();
        IsCapturing = false;
    }
    void StartSlowMotion()
    {
        StartCoroutine(StartSlowMotionCo());
    }
    IEnumerator StartSlowMotionCo()
    {
        yield return new WaitForSeconds(startSlowMotionTImeOffset);
        SlowMotionManager.instance.StartSlowMotion();
        StartCoroutine(StopSlowMotionCo());
    }
    IEnumerator StopSlowMotionCo()
    {
        yield return new WaitForSeconds(lengthSlowMotion);
        SlowMotionManager.instance.StopSlowMotion();
        isTossing = false;
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