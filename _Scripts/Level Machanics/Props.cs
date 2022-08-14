using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    [SerializeField] Transform[] broken;
    [SerializeField] float breakingForce;
    [SerializeField] LayerMask explosionLayer;
    [SerializeField] BoxCollider2D boxCol;

    public bool isDead { get; set; }

    private void Update()
    {
        if (isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        foreach (var item in broken)
        {
            item.gameObject.SetActive(true);
            item.parent = null;
            Vector2 _direction = item.position - transform.position;
            item.GetComponent<Rigidbody2D>().AddForce(_direction * breakingForce, ForceMode2D.Impulse);
            
        }
        Destroy(gameObject);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(boxCol.offset, boxCol.size);
    }
    
}
