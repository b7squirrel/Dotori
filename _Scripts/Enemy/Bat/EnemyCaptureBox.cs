using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCaptureBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            PlayerHealthController.instance.CapturePlayer(this.transform);
        }
    }
}
