using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Roll roll = new Roll();
    public RollSO defaultRollSo;

    public Flavor flavor = new Flavor();
    public FlavorSo defaultFlavorSo;

    public Image imageRoll;
    public Image imageFlavor;

    private void Start()
    {
        InitSlot();
    }

    // 슬롯의 하위에 있는 이미지를 인스펙터에서 끌어왔으므로 그 이미지를 변경하려면 roll.rollsprite를 넣어준다
    // 중간에 roll.rollsprite에 디폴트 스프라이트를 넣지 않고 바로 imageRoll.sprite에 디폴트 스프라이트를 넣어도 되지 않을까
    // 그렇지만 일단 roll.rollSo에 정보를 모두 입력해 주어야 나중에 빼서 쓸 때 오류가 나지 않을 것 같다.
    public void AddRoll(RollSO _rollSo)
    {
        roll.rollSo = _rollSo;
        roll.rollSo.rollType = _rollSo.rollType;
        roll.rollSprite = _rollSo.rollSprite_UI;
        imageRoll.sprite = roll.rollSprite;
    }
    public void AddFlavor(FlavorSo _flavorSo)
    {
        this.flavor.flavorSo = _flavorSo;
        flavor.flavorSo.flavorType = _flavorSo.flavorType;
        flavor.flavorSprite = _flavorSo.flavorSprite_UI;
        imageFlavor.sprite = flavor.flavorSprite;
    }
    public Roll GetRoll()
    {
        return roll;
    }

    public Flavor GetFlavor()
    {
        return flavor;
    }
    public void InitSlot()
    {
        AddRoll(defaultRollSo);
        AddFlavor(defaultFlavorSo);
    }
}
