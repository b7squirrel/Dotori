using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] Transform inPosition; // spike가 숨겨지는 위치
    [SerializeField] float coolTime;
    [SerializeField] float outSpeed;
    [SerializeField] float inSpeed;
    float coolTimeCounter;
    Vector3 outPosition;
    bool outState;

    private void Start()
    {
        outPosition = transform.position;
        inPosition.parent = null;
    }

    private void Update()
    {
        if (coolTimeCounter > 0)
        {
            coolTimeCounter -= Time.deltaTime;
        }
        else
        {
            coolTimeCounter = coolTime;
            outState = !outState;
        }
        if (outState)
        {
            SpikeOut();
        }
        else
        {
            SpikeIn();
        }
    }

    void SpikeIn()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, inPosition.position, inSpeed * Time.deltaTime);
    }

    void SpikeOut()
    {
        transform.position = 
            Vector3.MoveTowards(transform.position, outPosition, outSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            //if (PlayerController.instance.IsGrounded)
            //    return;
            PlayerHealthController.instance.KillPlayer();
        }
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>().Die();
        }
    }
}
