using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMovements : MonoBehaviour
{
    [SerializeField] Transform limit_L;
    [SerializeField] Transform limit_R;
    [SerializeField] Transform limit_T;
    [SerializeField] Transform limit_B;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] LayerMask slotLayer;


    private void Update()
    {
        //SetLimits();
    }

    private void FixedUpdate()
    {
        //SetLimits();
    }
    void SetLimits()
    {
        float _xPosition = Mathf.Clamp(transform.position.x, limit_L.position.x - xOffset, limit_R.position.x + xOffset);
        //float _yPosition = transform.position.y;
        float _yPosition = Mathf.Clamp(transform.position.y, limit_B.position.y + yOffset, limit_T.position.y);
        transform.position = new Vector2(_xPosition, _yPosition);
    }    

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .1f);
        Gizmos.DrawCube(transform.position - new Vector3(0, .5f, 0), new Vector2(1, .1f));
    }

}
