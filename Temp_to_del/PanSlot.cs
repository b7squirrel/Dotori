using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanSlot : MonoBehaviour
{
    public bool isEmpty = true;
    float _height;
    int _capacity;

    public bool IsEmpty()
    {
        if (isEmpty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetToOccupied()
    {
        isEmpty = false;
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
        isEmpty = false;
    }

    public Transform GetRoll()
    {
        return GetComponentInChildren<EnemyRolling>().transform;
    }

    public void MoveRoll(PanSlot _targetSlot)
    {
        if (!isEmpty)
        {
            GetRoll().position = _targetSlot.transform.position;
            GetRoll().parent = _targetSlot.transform;
            isEmpty = true;
            _targetSlot.SetToOccupied();
        }
    }

    public void RemoveRoll()
    {
        GetRoll().parent = null;
        isEmpty = true;
    }

    public void FlipRoll()
    {
        GetRoll().localEulerAngles += new Vector3(0, 0, 90f);
    }

    public void FlipSprite()
    {
        if (!isEmpty)
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
