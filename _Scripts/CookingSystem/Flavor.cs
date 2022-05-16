using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flavor
{
    public enum flavorType
    {
        none,
        fire,
        ice
    }

    public FlavorSo flavorSo;

    public Sprite flavorSprite;
    public GameObject[] flavorPrefab;
}
