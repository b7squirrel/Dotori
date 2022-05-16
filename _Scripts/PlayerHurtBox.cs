using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtBox : MonoBehaviour
{
    private Animator anim;
    public int whichSideToBeHit; // dieEffect의 날아가는 방향을 정하기 위함
    private bool isHitAfterParrying;
    private bool assumingBeingHit;
    private bool isAttackBoxParried; // projectile의 isAttackBoxParried를 가져오기 위한 플래그

    private float waitingTime;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("ProjectileEnemy"))
        {
            float distance = transform.position.x - collision.transform.position.x;

            if (distance < 0)
            {
                whichSideToBeHit = 0;
            }
            else if (distance > 0) 
            {
                whichSideToBeHit = 180;
            }

            PlayerHealthController.instance.isDead = true;
        }
    }
}
