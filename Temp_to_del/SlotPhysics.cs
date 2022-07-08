using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPhysics : MonoBehaviour
{
    [SerializeField] Transform[] slots = new Transform[3];

    void Start()
    {
        InitSlotPosition();
    }

    void InitSlotPosition()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].localPosition = new Vector2(0, i);
        }
    }
}
