using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] SlotPhysics slotPhysicsSet;
    [Header("Parry")]
    [SerializeField] float parryCoolTime;
    float parryCoolingCounter;
    PlayerController playerController;
    PanManager panManager;
    PlayerAttackBox playerAttackBox;
    PlayerCaptureBox playerCaptureBox;
    Animator panAnim;

    void Start()
    {
        panAnim = GetComponentInChildren<PlayerCapture>().GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        panManager = GetComponentInChildren<PanManager>();
        playerAttackBox = GetComponentInChildren<PlayerAttackBox>();
        playerCaptureBox = GetComponentInChildren<PlayerCaptureBox>();
        playerAttackBox.gameObject.SetActive(false);
    }

    void Update()
    {
        //Attack();
    }

    void Attack()
    {
        if (parryCoolingCounter > 0f)
        {
            parryCoolingCounter -= Time.deltaTime;
        }

        //어택 동작이 부득이하게 도중에 취소되어 attackboxOff가 실행되지 않았을 경우를 대비
        if (IsPlayingPanAnimation("Pan_Attack") == false)
        {
            playerAttackBox.gameObject.SetActive(false);
        }

        //공격이 발동되면 패링카운터까지 초기화 되어 패링도 가능하게 된다.
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            if (slotPhysicsSet.IsRollsOnPan)
                return;
            //if (IsPlayingPanAnimation("Pan_Capture"))
            //    return;
            if (IsPlayingPanAnimation("Pan_Attack"))
                return;
            if (IsPlayingPanAnimation("Pan_HitRoll"))
                return;
            if (parryCoolingCounter <= 0f)
            {
                panAnim.Play("Pan_Attack");
                AudioManager.instance.Play("whoosh_01");
                parryCoolingCounter = parryCoolTime;
            }
        }
    }
    bool IsPlayingPanAnimation(string _animation)
    {
        if (panAnim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// animation events
    /// parry할 때 움직임을 제어하기 위해 IsParrying 플레이어 컨트롤러에 전달. parry box 켜기
    /// </summary>
    void EnterAttack()
    {
        playerController.IsAttacking = true;
        playerAttackBox.gameObject.SetActive(true);
        playerController.SetParryStepTarget();
        if (playerCaptureBox == null)
            return;
        playerCaptureBox.gameObject.SetActive(false);
        
    }
    void ExitAttack()
    {
        playerController.IsAttacking = false;
        playerAttackBox.gameObject.SetActive(false);
    }
}
