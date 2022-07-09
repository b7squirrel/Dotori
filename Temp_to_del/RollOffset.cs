using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 slot들이 Proxy Slot들을 따라다니도록 함.
/// </summary>
public class RollOffset : MonoBehaviour
{
    [SerializeField] PanSlot[] slots;
    [SerializeField] Transform[] slotProxies = new Transform[3];

    private void Update()
    {
        FollowProxies();
        Flip();
    }
    
    void FollowProxies()
    {
        for (int i = 0; i < slotProxies.Length; i++)
        {
            slots[i].transform.position = slotProxies[i].position;
        }
    }

    void Flip()
    {
        foreach (var _slot in slots)
        {
            _slot.FlipSprite();
        }
    }
}