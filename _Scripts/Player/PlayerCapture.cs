using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCapture : MonoBehaviour
{
    [Header("Pan Inventory")]
    Animator panAnim;
    PanManager panManager;
    [SerializeField] RollSO rollso;
    [SerializeField] FlavorSo flavorSo;

    [Header("Capture")]
    [SerializeField] float captureBoxOnTime;  // 캡쳐박스가 활성화 되는 시간

    [Header("Toss Rolls")]
    [SerializeField] SlotPhysics slotPhysicsSet;
    [SerializeField] float lengthSlowMotion;
    [SerializeField] float startSlowMotionTImeOffset; // Toss anim 시작 후 얼마 후에 슬로우모션 시작할지
    bool isTossing;  // 참일 떄만 슬로우모션이 발동되도록 (capture할 때)
    bool isCapturing;

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
        captureBox.gameObject.SetActive(false);
    }
    void Update()
    {
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Toss") == false)
        {
            isTossing = false;
        }
        Toss();
        Capture();
        HitRolls();
        Debugging();
    }

    private void Debugging()
    {
        _isCapturing = isCapturing;
        _isTossing = isTossing;
    }

    void Capture()
    {
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Capture") == false)
        {
            isCapturing = false;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetMouseButtonDown(1))
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
                isCapturing = true;
            }
            //if (isTossing) // 순서가 중요함. 이전 프레임에서 toss를 했다면 이제는 capture가 가능함
            //{
            //    isCapturing = true;
            //}
            //if (slotPhysicsSet.IsRollsOnPan)  // 순서가 중요함. 롤이 팬 위에 있을 때는 Toss를 발동시킴
            //{
            //    isTossing = true;
            //}
            //if (isCapturing)
            //{
            //    panAnim.Play("Pan_Capture");
            //}
            //else if (isTossing) // isCapturing이 거짓이고 isTossing만 참이라면 Tossing
            //{
            //    panAnim.Play("Pan_Toss");
            //}
            panAnim.Play("Pan_Capture");
        }
    }
    void Toss()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (slotPhysicsSet.IsRollsOnPan == false) // 슬롯위에 롤이 없으면 Toss 입력 무시
                return;
            
            if (slotPhysicsSet.IsAnchorGrounded)  // 슬롯이 팬에 붙어 있다면 위쪽으로 올라가는 속도 대입
            {
                panAnim.Play("Pan_Toss");
                slotPhysicsSet.TossRolls();
            }
            
            StartSlowMotion();
        }
    }
    void HitRolls()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
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
        isCapturing = false;
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