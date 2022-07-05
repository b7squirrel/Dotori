using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
{
    [SerializeField] GameObject[] slotProxies = new GameObject[4];  // 슬롯이 이 gameObject를 따라가게 된다.
    [SerializeField] float verticalMax;
    [SerializeField] float horizontalMax;
    [SerializeField] Vector2 moveSpeed;
    Vector2 currentAnchorPosition;
    Vector2 pastAnchorPosition;

    Vector2[] currentPosition = new Vector2[4];
    Vector2[] pastPosition = new Vector2[4];
    Vector2[] direction = new Vector2[4];
    Vector2[] slotDeltaPosition = new Vector2[4];
    Vector2 slotMovement;
    Vector2 anchorDeltaDistance;

    void Start()
    {
        InitiateSlots();
    }

    void Update()
    {
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
            //if (Mathf.Abs(slotProxies[i + 1].transform.position.y) < .1f) 
            //{
            //    slotProxies[i + 1].transform.position =
            //        new Vector2(slotProxies[i + 1].transform.position.x, -moveSpeed.y);
            //}
        }
    }
    Vector2 GetSlotMovement()
    {
        slotMovement = moveSpeed;
        float yPosition = Mathf.Clamp(slotMovement.y, 1f, verticalMax);
        float xPosition = Mathf.Clamp(slotMovement.x, -horizontalMax, horizontalMax);
        slotMovement = new Vector2(xPosition, yPosition);
        return slotMovement;
    }
    void ApplySlotMovement()
    {
        for (int i = 0; i < slotProxies.Length - 1; i++)
        {
            slotProxies[i + 1].transform.position =
                Vector2.Lerp(slotProxies[i + 1].transform.position, GetSlotMovement() + (Vector2)slotProxies[i].transform.position, 10f * Time.deltaTime);
        }
    }
}
