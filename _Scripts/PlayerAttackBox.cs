using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerAttack의 parryTimer 참조해서 패링 성공여부 판단
/// </summary>
public class PlayerAttackBox : MonoBehaviour
{
    BoxCollider2D boxCol;
    Color parryColor;
    PlayerAttack playerAttack;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        parryColor = new Color(1, 0, 1, 0.5f);
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerAttack.ParryTimer <= 0f)
            return;
        if (collision.CompareTag("ProjectileEnemy"))
        {
            var clone = collision.GetComponent<EnemyProjectile>();
            clone.GetComponent<EnemyProjectile>().contactPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            clone.GetComponent<EnemyProjectile>().isParried = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null)
            return;
        Gizmos.color = parryColor;
        Gizmos.DrawCube(boxCol.bounds.center, boxCol.bounds.size);
    }
}
