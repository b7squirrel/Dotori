using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerHorizontal : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] Transform detectionBox;
    Vector2 directionVector;
    private void Start()
    {
        directionVector = (detectionBox.position - transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HurtBoxPlayer"))
        {
            Debug.Log(directionVector * force);
            collision.GetComponentInParent<Rigidbody2D>().AddForce(directionVector * force, ForceMode2D.Impulse);
        }
    }
}
