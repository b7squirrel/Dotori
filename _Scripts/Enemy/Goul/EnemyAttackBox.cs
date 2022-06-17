using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    [SerializeField] EnemyHealth _enemyHealth; // 자신의 EnemyHealth 끌어놓기
    [SerializeField] float parriedBufferTime;
    float parriedBufferTimeCounter;
    bool isHittingPlayer;
    bool isHittingAttackBox;
    [SerializeField] GameObject parriedEffect;
    Vector2 parriedEffectPoint;

    [field: SerializeField]
    public Transform BodyPoint { get; private set; } // Player Hurt Box에서 접근해서 Enmey본체의 x position을 참조

    private void Start()
    {
        parriedBufferTimeCounter = parriedBufferTime;
    }

    /// <summary>
    /// 플레이어의 Hurt Box에 닿았을 때, 
    /// parriedBufferTImeCounter가 아직 0이 아니면서 플레이어의 attackBox 판정이 나오면
    /// 자신을 parried State로 설정함
    /// </summary>
    private void Update()
    {
        if (isHittingAttackBox)
        {
            parriedBufferTimeCounter = parriedBufferTime;
            _enemyHealth.SetParriedState(true);
            FeedbackOnParried();
            isHittingAttackBox = false;
            isHittingPlayer = false;
        }

        if (isHittingPlayer == false)
            return;

        parriedBufferTimeCounter -= Time.deltaTime;

        if (parriedBufferTimeCounter <= 0)
        {
            PlayerHealthController.instance.KillPlayer();
            isHittingAttackBox = false;
            isHittingPlayer = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("AttackBoxPlayer"))
        {
            isHittingAttackBox = true;
            parriedEffectPoint = collision.ClosestPoint(transform.position);
        }
        if (collision.CompareTag("HurtBoxPlayer"))  // 패리와 거의 동시에 hurt box에 닿으면 무시하도록
        {
            isHittingPlayer = true;
        }
    }

    void FeedbackOnParried()
    {
        AudioManager.instance.Play("pan_hit_05");
        Instantiate(parriedEffect, parriedEffectPoint, Quaternion.identity);
    }
}
