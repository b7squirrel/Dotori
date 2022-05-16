using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    BoxCollider2D boxCol;
    Color parryColor;
    bool isParrying;
    PlayerAttack playerAttack;
    int _attackPower;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        parryColor = new Color(1, 0, 1, 0.5f);

        playerAttack = GetComponentInParent<PlayerAttack>();
        SetAttackPower(1);
    }

    private void Update()
    {
        if (playerAttack.parryTimer > 0f)
        {
            playerAttack.parryTimer -= Time.deltaTime;
        }
        else
        {
            playerAttack.parryTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerAttack.parryTimer > 0f)
        {
            if(collision.CompareTag("ProjectileEnemy"))
            {
                var clone = collision.GetComponent<EnemyProjectile>();
                clone.GetComponent<EnemyProjectile>().contactPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
                clone.GetComponent<EnemyProjectile>().isParried = true;
            }
        }
        else
        {
            return;
        }
    }
    public int GetAttackPower()
    {
        return _attackPower;
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null)
            return;
        Gizmos.color = parryColor;
        Gizmos.DrawCube(boxCol.bounds.center, boxCol.bounds.size);
    }

    public void SetAttackPower(int powerValue)
    {
        _attackPower = powerValue;
    }

}
