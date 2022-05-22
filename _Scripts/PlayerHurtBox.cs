using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Health Controller에서 Die Effect를 날릴 떄 어느 방향으로 날릴지 Angle_Y를 참조해서 결정
/// Player Health Controller에서 플레이어가 죽었다면 Angle_Y값을 사용해서 파티클 생성
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
        // 플레이어가 너무 깊게 들어가서 죽으면 attack box는 플레이어의 뒤에 있을 수도 있음. 그래서 본체의 위치로 방향 확인. 
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
