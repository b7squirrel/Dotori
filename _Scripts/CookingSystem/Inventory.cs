using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI관리
/// Roll, Flavor 갯수 관리
/// </summary>
public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Slot[] InputSlots;
    public RollSO defaultRollSo;
    public FlavorSo defaultFlavorSo;
    
    public int numberOfRolls = 0;
    public bool isFlavored = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InputSlots = GetComponentsInChildren<Slot>();
        ResetInventory();
        isFlavored = false;
    }

    public void ResetInventory()
    {
        foreach (Slot _slot in InputSlots)
        {
            _slot.InitSlot();
        }
        numberOfRolls = 0;
        isFlavored = false;
    }

    public void AcquireRoll(RollSO _rollSo)
    {
        if (InputSlots[0].GetRoll().rollSo.rollType != Roll.rollType.none)
        {
            for (int i = 0; i < numberOfRolls; i++)
            {
                if (InputSlots[i].GetRoll().rollSo.rollType != _rollSo.rollType)  // Roll이 있는 슬롯만 돌면서 비교하다가 다른 roll이 나오면
                {
                    for (int j = 0; j < InputSlots.Length; j++)  // 모든 슬롯을 default로 초기화 하고
                    {
                        InputSlots[j].AddRoll(defaultRollSo);
                    }
                    numberOfRolls = 0;
                    return;
                }
            }
        }
        // 슬롯을 돌면서 비교했는데 모두 캡쳐한 roll과 같은 타입이라면
        foreach (Slot _slot in InputSlots)
        {
            if(_slot.GetRoll().rollSo.rollType == Roll.rollType.none)
            {
                _slot.AddRoll(_rollSo);
                numberOfRolls++;

                if (isFlavored)
                {
                    AcquireFlavor(CookingSystem.instance.outputFlavor);
                }
                return;
            }
        }
    }

    public void AcquireFlavor(FlavorSo _flavorSo)
    {
        if(numberOfRolls > 0)  // Roll이 있을 때만 Flavor를 받음
        {
            for (int i = 0; i < numberOfRolls; i++)
            {
                InputSlots[i].AddFlavor(_flavorSo);
            }
            isFlavored = true;
        }
    }
}
