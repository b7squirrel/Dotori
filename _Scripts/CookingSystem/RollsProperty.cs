using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollsProperty : MonoBehaviour
{
    public SpriteRenderer FlavorSprite;
    public CircleCollider2D col;

    public RollSO rollSO;
    public FlavorSo flavorSO;

    private void Awake()
    {
        col = GetComponentInChildren<CircleCollider2D>();
        FlavorSprite = GetComponentInChildren<SpriteRenderer>();
    }
}
