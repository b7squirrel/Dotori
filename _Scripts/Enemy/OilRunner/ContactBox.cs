using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactBox : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHealth;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            if (collision.gameObject.layer == 18) // 18. playerDodging
                return;
            PlayerHealthController.instance.KillPlayer();
            enemyHealth.Die();
        }
    }
}
