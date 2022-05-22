using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Parry")]
    [SerializeField] float parryCoolTime;
    float parryCoolingCounter;
    PlayerController playerController;
    PlayerParryBox playerParryBox;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerParryBox = GetComponentInChildren<PlayerParryBox>();
        playerParryBox.gameObject.SetActive(false);
    }

    void Update()
    {
        if(parryCoolingCounter > 0f)
        {
            parryCoolingCounter -= Time.deltaTime;
        }

        //어택 동작이 부득이하게 도중에 취소되어 attackboxOff가 실행되지 않았을 경우를 대비
        if (IsPlayingAnimation("Player_Parry") == false)
        {
            playerParryBox.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (PanManager.instance.CountRollNumber() > 0)
                return;
            if (IsPlayingAnimation("Player_Parry"))
                return;
            if (parryCoolingCounter <= 0f)
            {
                anim.Play("Player_Parry");
                AudioManager.instance.Play("whoosh_01");
                parryCoolingCounter = parryCoolTime;
            }
        }
    }
    bool IsPlayingAnimation(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
        {
            return true;
        }
        return false;
    }
    //animation events
    /// <summary>
    /// parry할 때 움직임을 제어하기 위해 IsParrying 플레이어 컨트롤러에 전달. parry box 켜기
    /// </summary>
    void EnterParry()
    {
        playerController.IsParrying = true;
        playerParryBox.gameObject.SetActive(true);
        playerController.SetParryStepTarget();
    }
    void ExitParry()
    {
        playerController.IsParrying = false;
        playerParryBox.gameObject.SetActive(false);

    }
}
