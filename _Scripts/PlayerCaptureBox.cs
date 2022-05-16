using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCaptureBox : MonoBehaviour
{
    [HideInInspector] public BoxCollider2D boxCol;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Capture Timer가 0보다 크다는 말은 캡쳐버튼을 눌러서 Capture Duration값으로 초기화 되었다는 의미
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ProjectileEnemy") && PlayerPanAttack.instance.CaptureTimer > 0)
        {
            EnemyProjectile _clone = collision.GetComponent<EnemyProjectile>();
            _clone.isCaptured = true;
            _clone.tag = "ProjectileCaptured";
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null)
            return;

        if (PlayerPanAttack.instance.CaptureTimer > 0)
        {
            Color color = new Color(1, 0, 0, .3f);
            Gizmos.color = color;
            Gizmos.DrawCube(boxCol.bounds.center, boxCol.bounds.size);
        }
    }
}
