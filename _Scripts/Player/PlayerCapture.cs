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

    [Header("Toss Rolls")]
    [SerializeField] SlotPhysics slotPhysicsSet;

    [Header("Effects")]
    [SerializeField] GameObject HitRollEffect;
    [SerializeField] Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    void Start()
    {
        panAnim = GetComponent<Animator>();
        panManager = GetComponentInChildren<PanManager>();
        captureBox.gameObject.SetActive(false);
    }
    void Update()
    {
        Capture();
        HitRolls();
        TossRolls();
    }
    void Capture()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.Z) || Input.GetMouseButtonDown(0))  // 캡쳐 캔슬
                return;
            slotPhysicsSet.TossRolls();
            panAnim.Play("Pan_Capture");
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
    void TossRolls()
    {
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    if (slotPhysicsSet.IsRollsOnPan == false)
        //    {
        //        return;
        //    }
        //    slotPhysicsSet.TossRolls();
        //}
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