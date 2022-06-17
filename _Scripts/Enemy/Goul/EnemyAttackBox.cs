using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    [SerializeField] EnemyHealth _enemyHealth; // �ڽ��� EnemyHealth �������
    [SerializeField] float parriedBufferTime;
    float parriedBufferTimeCounter;
    bool isHittingPlayer;
    bool isHittingAttackBox;
    [SerializeField] GameObject parriedEffect;
    Vector2 parriedEffectPoint;

    [field: SerializeField]
    public Transform BodyPoint { get; private set; } // Player Hurt Box���� �����ؼ� Enmey��ü�� x position�� ����

    private void Start()
    {
        parriedBufferTimeCounter = parriedBufferTime;
    }

    /// <summary>
    /// �÷��̾��� Hurt Box�� ����� ��, 
    /// parriedBufferTImeCounter�� ���� 0�� �ƴϸ鼭 �÷��̾��� attackBox ������ ������
    /// �ڽ��� parried State�� ������
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
        if (collision.CompareTag("HurtBoxPlayer"))  // �и��� ���� ���ÿ� hurt box�� ������ �����ϵ���
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
