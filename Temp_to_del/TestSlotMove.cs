using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSlotMove : MonoBehaviour
{
    [SerializeField] GameObject[] slots = new GameObject[4];
    [SerializeField] float moveSpeed;
    Vector2[] currentPosition = new Vector2[4];
    Vector2[] pastPosition = new Vector2[4];
    Vector2[] direction = new Vector2[4];

    void Start()
    {
        for (int i = 1; i < slots.Length; i++)
        {
            slots[i].transform.localPosition = new Vector3(0, 1, 0);
        }
    }

    void Update()
    {
        GetDirecitons();
        ApplyMovements();
    }


    void GetDirecitons()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            currentPosition[i] = slots[i].transform.position;
            direction[i] = (currentPosition[i] - pastPosition[i]).normalized;
            pastPosition[i] = currentPosition[i];
        }
    }
    void ApplyMovements()
    {
        for (int i = 1; i < slots.Length; i++)
        {
            // 부모의 움직임과 반대로 움직이고 감소된 움직임.
            Vector2 _targetPosition = new Vector2 (0,1)-direction[i -1] * 1/(i*2);
            float yPosition = Mathf.Clamp(_targetPosition.y, 1, 2f);
            float xPosition = Mathf.Clamp(_targetPosition.x, -1, 1f);
            _targetPosition = new Vector2(xPosition, yPosition);
            slots[i].transform.localPosition = Vector2.Lerp(slots[i].transform.localPosition, _targetPosition, moveSpeed * Time.deltaTime);
            
            //slots[i].transform.localPosition = new Vector2(slots[i].transform.position.x, yPosition);
        }
    }
}
