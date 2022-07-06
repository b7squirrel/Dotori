using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
{
    [SerializeField] GameObject[] slotProxies = new GameObject[4];  // 슬롯이 이 gameObject를 따라가게 된다.
    [SerializeField] float verticalMax;
    [SerializeField] float horizontalMax;
    [SerializeField] Vector2 moveSpeed;

    [SerializeField] Animator animPan;

    Vector2[] currentPosition = new Vector2[4];
    Vector2[] pastPosition = new Vector2[4];
    Vector2[] direction = new Vector2[4];
    Vector2[] slotDeltaPosition = new Vector2[4];

    
    void Start()
    {
        InitiateSlots();
    }

    void Update()
    {
        SetVerticalMax();
        GetDirecitons();
        GetDeltaPositions();
        ApplyMovements();
    }

    void InitiateSlots()
    {
        for (int i = 0; i < slotProxies.Length - 1; i++)
        {
            slotProxies[i + 1].transform.parent = slotProxies[i].transform;
            
        }
        for (int i = 0; i < slotProxies.Length - 1; i++)
        {
            slotProxies[i + 1].transform.position = (Vector2)slotProxies[i].transform.position + new Vector2(0, 1f);
        }
    }

    /// <summary>
    /// 아래 슬롯과 반대 움직임 벡터를 가짐
    /// </summary>
    void GetDirecitons()
    {
        for (int i = 0; i < slotProxies.Length; i++)
        {
            currentPosition[i] = slotProxies[i].transform.position;
            direction[i] = (currentPosition[i] - pastPosition[i]).normalized;
            pastPosition[i] = currentPosition[i];
        }
    }
    void GetDeltaPositions()
    {
        for (int i = 1; i < slotProxies.Length; i++)
        {
            slotDeltaPosition[i] = -direction[i - 1] * moveSpeed;
            float yPosition = Mathf.Clamp(slotDeltaPosition[i].y, 1f, verticalMax);
            float xPosition = Mathf.Clamp(slotDeltaPosition[i].x, -horizontalMax, horizontalMax);
            slotDeltaPosition[i] = new Vector2(xPosition, yPosition);
        }
    }
    void ApplyMovements()
    {
        for (int i = 0; i < slotProxies.Length - 1; i++)
        {
            slotProxies[i + 1].transform.position =
                Vector2.Lerp(slotProxies[i + 1].transform.position, 
                slotDeltaPosition[i] + (Vector2)slotProxies[i].transform.position, 10f * Time.deltaTime);
        }
    }

    void SetVerticalMax()
    {
        if (animPan.GetCurrentAnimatorStateInfo(0).IsName("Pan_Pan")
            || animPan.GetCurrentAnimatorStateInfo(0).IsName("Pan_Capture")
            || animPan.GetCurrentAnimatorStateInfo(0).IsName("Pan_HitRoll"))
        {
            verticalMax = 1.5f;
        }
        else
        {
            verticalMax = 3f;
        }
    }
}
