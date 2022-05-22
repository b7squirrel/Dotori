using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerAttack의 parryTimer 참조해서 패링 성공여부 판단
/// </summary>
public class PlayerParryBox : MonoBehaviour
{
    BoxCollider2D boxCol;
    Color parryColor;
    PlayerController playerConteroller;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        parryColor = new Color(1, 0, 1, 0.5f);
        playerConteroller = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerConteroller.IsParrying == false)
            return;
        if (collision.CompareTag("ProjectileEnemy"))
        {
            var clone = collision.GetComponent<EnemyProjectile>();
            clone.GetComponent<EnemyProjectile>().ContactPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            clone.GetComponent<EnemyProjectile>().IsParried = true;
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
