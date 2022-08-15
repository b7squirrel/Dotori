using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCaptureBox : MonoBehaviour
{
    BoxCollider2D boxCol;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ProjectileEnemy"))
        {
            EnemyProjectile _clone = collision.GetComponent<EnemyProjectile>();
            _clone.IsCaptured = true;
            _clone.tag = "ProjectileCaptured";
        }
        if (collision.CompareTag("FlavorBG"))
        {
            FlavorBG _flavorBG = collision.GetComponent<FlavorBG>();
            _flavorBG.IsCaptured = true;
        }

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>().PlayerCaptureBoxIn();
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null)
            return;
        Color color = new Color(1, 0, 0, .3f);
        Gizmos.color = color;
        Gizmos.DrawCube(boxCol.bounds.center, boxCol.bounds.size);
    }
}
