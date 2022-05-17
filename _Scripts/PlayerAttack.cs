using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] GameObject attackBox;
    [SerializeField] float attackCoolTime;
    float attackTimer;

    [SerializeField] float parryDuration;
    public float ParryTimer { get; set; }

    Animator anim;

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
            if (PanManager.instance.CountRollNumber() > 0)
                return;
            if (attackTimer <= 0f)
            {
                anim.Play("Pan_Attack");
                AudioManager.instance.Play("whoosh_01");
                attackTimer = attackCoolTime;
            }
        }

        if (ParryTimer > 0)
        {
            ParryTimer -= Time.deltaTime;
        }
    }
    // animation event
    void AttackBoxOn()
    {
        attackBox.SetActive(true);
        ParryTimer = parryDuration;
    }
    void AttackBoxOff()
    {
        attackBox.SetActive(false);
    }
}
