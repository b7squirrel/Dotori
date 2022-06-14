using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBlocker : MonoBehaviour
{
    [SerializeField] Collider2D collisionCol;
    [SerializeField] Collider2D characterBlockerCol;

    void Start()
    {
        Physics2D.IgnoreCollision(collisionCol, characterBlockerCol);
    }
}
