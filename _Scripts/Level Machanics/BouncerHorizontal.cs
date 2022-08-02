using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerHorizontal : MonoBehaviour
{
    [SerializeField] float horizontalSpeed, verticalSpeed;
    [SerializeField] Transform detectionBox;
    Animator anim;
    Vector2 bouncerForceVector;
    private void Start()
    {
        anim = GetComponent<Animator>();
        bouncerForceVector = (detectionBox.position - transform.position).normalized;
        bouncerForceVector = new Vector2(bouncerForceVector.x * horizontalSpeed, verticalSpeed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            anim.Play("Bouncer_On");
            collision.gameObject.GetComponentInParent<PlayerController>().OnBouncer(bouncerForceVector);
        }
    }
}
