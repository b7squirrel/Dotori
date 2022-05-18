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
            Debug.Log("Hitting Attack BOx");
            parriedBufferTimeCounter = parriedBufferTime;
            _enemyHealth.SetParriedState(true);
            isHittingAttackBox = false;
            isHittingPlayer = false;
        }

        if (isHittingPlayer == false)
            return;

        parriedBufferTimeCounter -= Time.deltaTime;

        if (parriedBufferTimeCounter <= 0)
        {
            PlayerHealthController.instance.isDead = true;
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
        }
        if (collision.CompareTag("HurtBoxPlayer"))  // �и��� ���� ���ÿ� hurt box�� ������ �����ϵ���
        {
            isHittingPlayer = true;
        }
    }
}
