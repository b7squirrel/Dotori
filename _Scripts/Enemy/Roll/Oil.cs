using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField] int maxBounce;
    [SerializeField] float lifeTime;
    int numberOfBounceToGround;
    float lifeTimeCounter;
    private void Start()
    {
        numberOfBounceToGround = maxBounce;
        lifeTimeCounter = lifeTime;
    }
    private void Update()
    {
        if (numberOfBounceToGround == 0)
        {
            Destroy(gameObject);
        }
        if (lifeTimeCounter > 0)
        {
            lifeTimeCounter -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            numberOfBounceToGround--;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            numberOfBounceToGround--;
        }
    }
}
