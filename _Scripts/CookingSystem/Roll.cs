using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll
{
    public enum rollType
    {
        none,
        goulF,
        warlockF,
        goblinBolt,
        cornFly,
        runner,
        projectileF
    }
    public RollSO rollSo;

    public Sprite rollSprite;
    public GameObject[] rollPrefab;
}
