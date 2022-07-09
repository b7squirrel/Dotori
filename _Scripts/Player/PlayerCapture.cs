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
    [SerializeField] Transform anchor;
    [SerializeField] float tossForce;
    [SerializeField] float tossGravity;
    [SerializeField] float tossGravityScale;

    [Header("Toss Debug")]
    [SerializeField] bool isAnchorGrounded;
    [SerializeField] Transform anchorInitialPoint;
    [SerializeField] float tossVelocity;

    [field: SerializeField]
    public bool IsRollsOnPan { get; private set; }

    [Header("Effects")]
    [SerializeField] GameObject HitRollEffect;
    [SerializeField] Transform captureBox;  // 여기서 HItRoll Effect를 발생시키기

    void Start()
    {
        panAnim = GetComponent<Animator>();
        panManager = GetComponentInChildren<PanManager>();
        captureBox.gameObject.SetActive(false);

        anchor.position = anchorInitialPoint.position;
    }
    void Update()
    {
        AnchorGroundCheck();
        CheckIsRollsOnPan();

        Capture();
        HitRolls();
        TollRolls();

        OnTossRolls();
    }
    void Capture()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.Z) || Input.GetMouseButtonDown(0))  // 캡쳐 캔슬
                return;
            panAnim.Play("Pan_Capture");
        }
    }
    void HitRolls()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            if (panManager.CountRollNumber() == 0)
                return;
            panAnim.Play("Pan_HitRoll");
            panManager.ClearRoll();
            EffectsClearRoll();
        }
    }
    void TollRolls()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (IsRollsOnPan == false)
            {
                return;
            }
            tossVelocity = tossForce;
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
    }
    void CaptureBoxOff()
    {
        captureBox.gameObject.SetActive(false);
    }
    void OnTossRolls()
    {
        tossVelocity += -tossGravity * tossGravityScale * Time.deltaTime;
        if (isAnchorGrounded == true && tossVelocity < 0)
        {
            tossVelocity = 0;
        }
        anchor.transform.Translate(new Vector3(0, tossVelocity, 0) * Time.deltaTime);

    }
    void AnchorGroundCheck()
    {
        if (Vector2.Distance(anchor.position, anchorInitialPoint.position) < .1f)
        {
            isAnchorGrounded = true;
            anchor.position = anchorInitialPoint.position;
            return;
        }
        isAnchorGrounded = false;
    }
    void CheckIsRollsOnPan()
    {
        // 롤이 팬위에 붙어 있는가
        if (panManager.CountRollNumber() > 0 && isAnchorGrounded)
        {
            IsRollsOnPan = true;
            return;
        }
        IsRollsOnPan = false;
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