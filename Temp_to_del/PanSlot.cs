using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanSlot : MonoBehaviour
{
    public bool IsEmpty { get; private set; } = true;
    [SerializeField] float moveSpeed;
    PanSlot targetSlot;
    GameObject movingRoll;
    bool isMoving;
    float _height;
    int _capacity;

    private void Update()
    {
        UpdateRollMovement();
    }

    public void SetToOccupied()
    {
        IsEmpty = false;
    }

    void UpdateSlot(RollSO _rollSO)
    {
        _height = _rollSO.height;
    }

    public void AddRoll(Transform _prefab)
    {
        _prefab.position = transform.position;
        _prefab.rotation = transform.rotation;
        _prefab.parent = transform;
        IsEmpty = false;
    }

    public Transform GetRoll()
    {
        return GetComponentInChildren<EnemyRolling>().transform;
    }

    public void MoveRoll(PanSlot _targetSlot)
    {
        if (!IsEmpty)
        {
            isMoving = true;
            targetSlot = _targetSlot;
            movingRoll = GetRoll().gameObject;
            IsEmpty = true;
            movingRoll.transform.parent = targetSlot.transform;
            _targetSlot.SetToOccupied();
            //GetRoll().position = _targetSlot.transform.position;
            //GetRoll().parent = _targetSlot.transform;
            //IsEmpty = true;
            //_targetSlot.SetToOccupied();
        }
    }

    void UpdateRollMovement()
    {
        if (isMoving == false)
            return;
        movingRoll.transform.position = 
            Vector2.MoveTowards(movingRoll.transform.position, targetSlot.transform.position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(movingRoll.transform.position, targetSlot.transform.position) < .1f)
        {
            isMoving = false;
        }
    }

    public void RemoveRoll()
    {
        GetRoll().parent = null;
        IsEmpty = true;
    }

    public void FlipRoll()
    {
        GetRoll().localEulerAngles += new Vector3(0, 0, 90f);
    }

    public void FlipSprite()
    {
        if (!IsEmpty)
        {
            if (PlayerController.instance.GetPlayerDirection() > 0)
            {
                //float _rotateZ = transform.eulerAngles.z;
                //transform.eulerAngles = new Vector3(0, 0, _rotateZ);
                EnemyRolling _roll = GetComponentInChildren<EnemyRolling>();
                float _rotationZ = _roll.transform.eulerAngles.z;
                _roll.transform.eulerAngles = new Vector3(0, 0, _rotationZ);
            }
            else
            {
                //float _rotateZ = transform.eulerAngles.z;
                //transform.eulerAngles = new Vector3(0, 180f, _rotateZ);
                EnemyRolling _roll = GetComponentInChildren<EnemyRolling>();
                float _rotationZ = _roll.transform.eulerAngles.z;
                _roll.transform.eulerAngles = new Vector3(0, 180f, _rotationZ);
            }
        }
    }
}
