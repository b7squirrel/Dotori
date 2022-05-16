using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float pushForce;
    public Transform panPosition;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("RollsOnPan"))
        {
            Rigidbody2D _theRB = collision.GetComponent<Rigidbody2D>();
            collision.transform.position = Vector2.Lerp(collision.transform.position, panPosition.position, 1f);
        }
    }
}
