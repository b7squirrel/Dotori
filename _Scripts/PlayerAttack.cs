using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;

    public GameObject attackBox;
    public float attackCoolTime;
    private float attackTimer;

    public float parryDuration;
    [HideInInspector] public float parryTimer;

    private bool isAttacking; // 어택 애니메이션이 재생되는 동안은 다른 어택 애니메이션이 재생되지 않도록 하는 플래그

    public Inventory inventory;

    void Start()
    {
        anim = GetComponent<Animator>();
        attackBox.SetActive(false);
    }

    void Update()
    {
        if(attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        //어택 동작이 부득이하게 도중에 취소되어 attackboxOff가 실행되지 않았을 경우를 대비
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Pan_Attack"))
        {
            AttackBoxOff();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(PanManager.instance.CountRollNumber() == 0)  // 롤이 팬 위에 없으면 일반 공격
            {
                if(attackTimer <= 0f)
                {
                    anim.Play("Pan_Attack");
                    AudioManager.instance.Play("whoosh_01");
                    attackTimer = attackCoolTime;
                }
            }
        }
    }
    // animation event
    void AttackBoxOn()
    {
        attackBox.SetActive(true);
        parryTimer = parryDuration;
    }
    void AttackBoxOff()
    {
        attackBox.SetActive(false);
    }
}
