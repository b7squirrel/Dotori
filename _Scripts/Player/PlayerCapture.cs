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
    PlayerData playerData;
    PlayerTargetController playerTargetController;
    PlayerController playerController;

    [SerializeField] RollSO rollso;
    [SerializeField] FlavorSo flavorSo;

    [Header("Capture")]
    [SerializeField] float captureBoxOnTime;  // 캡쳐박스가 활성화 되는 시간
    [SerializeField] float slowMotionTimeScale;

    [Header("Toss Rolls")]
    [SerializeField] SlotPhysics slotPhysicsSet;
    [SerializeField] float lengthSlowMotion;
    [SerializeField] float startSlowMotionTImeOffset; // Toss anim 시작 후 얼마 후에 슬로우모션 시작할지
    bool isTossing;  // 참일 떄만 슬로우모션이 발동되도록 (capture할 때)
    public bool IsCapturing { get; private set; } // player controller에서 움직임을 제어하기 위해 public

    // player target controller에서 계속 업데이트 되는 horizontal direction을 가져와서 캡쳐를 시작할 때 고정
    public float CaptureDirection { get; private set; } 
    public float CaptureDashSpeed { get; private set; }

    [Header("Effects")]
    [SerializeField] GameObject HitRollEffect;
    [SerializeField] Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    [Header("Debug")]
    [SerializeField] bool _isTossing;
    [SerializeField] bool _isCapturing;

    void Start()
    {
        panAnim = GetComponent<Animator>();
        panManager = GetComponentInChildren<PanManager>();
        playerTargetController = GetComponentInParent<PlayerTargetController>();
        playerController = GetComponentInParent<PlayerController>();
        playerData = GetComponentInParent<PlayerData>();
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
        DodgeTurnPan();
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

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z))
        {
            if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Capture"))
                return;

            IsCapturing = true;
            playerData.Play(PlayerData.soundType.captureSwing);
            //CaptureDirection = playerTargetController.GetMouseHorizontalDirection();
            Toss(true);
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

    void DodgeTurnPan()
    {
        if (IsCapturing)
            return;

        if (playerController.IsDodging)
        {
            if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_DodgeTurn") == false)
            {
                panAnim.Play("Pan_DodgeTurn");
            }
        }
    }

    void HitRolls()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
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
    
    void Panning()
    {
        if (panManager.CountRollNumber() == 0)
            return;
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
            return;
        panAnim.Play("Pan_Pan");
    }

    /// <summary>
    /// Capture의 마지막 프레임에서 애니메이션 이벤트로 실행
    /// Capture해서 롤이 팬 위에 있다면 Pan_Pan으로 그렇지 않다면 Pan_idle로
    /// Capture가 끝났는데 DodgeTurn 상태라면 Pan_DodgeTurn으로
    /// DodgeTurn 상태일 때는 항상 Roll은 공중에 떠있으니까 다른 경우의 수는 없다
    /// </summary>
    void ExitCapture()
    {
        if (panManager.CountRollNumber() == 0)
            panAnim.Play("Pan_Idle");

        if (playerController.IsDodging)
            panAnim.Play("Pan_DodgeTurn");

        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
            return;

        panAnim.Play("Pan_Pan");
    }

    void ResetPanState()
    {
        if (panManager.CountRollNumber() == 0)
            panAnim.Play("Pan_Idle");

        if (playerController.IsDodging)
            panAnim.Play("Pan_DodgeTurn");

        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan"))
            return;

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
        SlowMotionManager.instance.StartSlowMotion(slowMotionTimeScale);
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

        playerData.Play(PlayerData.soundType.hitRoll_01);
        playerData.Play(PlayerData.soundType.hitRoll_02);
        
        GameManager.instance.StartCameraShake(8, .8f);
        GameManager.instance.TimeStop(.1f);
    }
}