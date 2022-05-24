using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Health Controller���� Die Effect�� ���� �� ��� �������� ������ Angle_Y�� �����ؼ� ����
/// Player Health Controller���� �÷��̾ �׾��ٸ� Angle_Y���� ����ؼ� ��ƼŬ ����
/// </summary>
public class PlayerHurtBox : MonoBehaviour
{
    public int Angle_Y { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ProjectileEnemy"))
        {
            Vector2 _direction = collision.transform.position - PlayerController.instance.transform.position;
            if (_direction.x > 0)
            {
                Angle_Y = 0;
            }
            else
            {
                Angle_Y = 180;
            }
        }
        // �÷��̾ �ʹ� ��� ���� ������ attack box�� �÷��̾��� �ڿ� ���� ���� ����. �׷��� ��ü�� ��ġ�� ���� Ȯ��. 
        if (collision.CompareTag("AttackBoxEnemy"))
        {
            Transform _enemyTransform = collision.GetComponent<EnemyAttackBox>().BodyPoint;
            Vector2 _direction = _enemyTransform.position - PlayerController.instance.transform.position;
            if (_direction.x > 0)
            {
                Angle_Y = 0;
            }
            else
            {
                Angle_Y = 180;
            }
        }
    }
}
