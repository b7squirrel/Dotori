using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFalling : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] float timer;
    Rigidbody2D theRB;

    [Header("Debug")]
    [SerializeField] float timeCounter;
    [SerializeField] bool isTriggered;

    private void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        timeCounter = timer;
    }

    private void Update()
    {
        CountDown();
    }
    void CountDown()
    {
        if (isTriggered == false)
            return;
        if (timeCounter < 0)
        {
            Fall();
            isTriggered = false;
            return;
        }
        timeCounter -= Time.deltaTime;
    }
    void Fall()
    {
        theRB.bodyType = RigidbodyType2D.Dynamic;
        theRB.gravityScale = 5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
        }
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
