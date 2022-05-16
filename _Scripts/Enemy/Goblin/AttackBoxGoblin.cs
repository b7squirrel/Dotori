using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoxGoblin : MonoBehaviour
{
    private GoblinBolt goblinBolt;

    private void Awake()
    {
        goblinBolt = GetComponentInParent<GoblinBolt>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer") && !goblinBolt.isParried)
        {
            PlayerHealthController.instance.isDead = true;
        }
    }
}
