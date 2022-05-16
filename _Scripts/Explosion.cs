using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public GameObject explosionEffect;
    public BoxCollider2D boxCol;

    [Header("Debris")]
    public GameObject debris;
    public GameObject debrisDirt;
    public GameObject debrisParticleEffect;

    public int numberOfRolls;
    private Vector2 explosionSize;

    public GameObject debugDot;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        DestroyArea(numberOfRolls);
    }

    public void DestroyArea(int _size)
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Vector2Int _boxColSize = new Vector2Int(Mathf.RoundToInt(Mathf.Pow(_size, 2) * 2),
            Mathf.RoundToInt(Mathf.Pow(_size, 2) * 2 - _size));
        explosionSize = new Vector2(_boxColSize.x, _boxColSize.y);

        for (int i = -_boxColSize.x / 2; i <= _boxColSize.x / 2; i++)
        {
            for (int j = -_boxColSize.y / 2; j <= _boxColSize.y / 2; j++)
            {
                Vector3 _cellPosition =
                    new Vector3(transform.position.x + i, transform.position.y + j, 0);

                Instantiate(debugDot, _cellPosition, Quaternion.identity);

                Collider2D _hitground = Physics2D.OverlapCircle(_cellPosition, .02f, groundLayer);
                if (_hitground != null)
                {
                    _hitground.GetComponent<Tiles>().RemoveTile(_cellPosition);
                    GenerateDebris(transform.position, _cellPosition);
                }
            }
        }
        // Explosion 오브젝트의 tag를 attackBoxPlayer로 해서 TakeDamage에서 감지한다.
        // TakeDamage의 Die를 직접 건드리면 안된다. 왜인지 모르겠다. 
        boxCol.size = explosionSize;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .2f);
        Gizmos.DrawCube(transform.position, explosionSize);
    }
    private void GenerateDebris(Vector2 _expPoint, Vector2 _DebrisPoint)
    {
        Instantiate(debrisParticleEffect, _DebrisPoint, Quaternion.identity);
    }
}
