using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorBG : MonoBehaviour
{
    [SerializeField] GameObject[] torchParts;
    [SerializeField] Flavor.flavorType flavorType;
    FlavorSo flavorSo;
    public bool IsCaptured { get; set; }

    private void Update()
    {
        if (IsCaptured)
        {
            GetFlavored();
        }
    }

    /// <summary>
    /// 주어진 flaovrType으로 flavorSo를 가져온다. 파티클이나 조명은 비활성화 시킴
    /// </summary>
    void GetFlavored()
    {
        flavorSo = RecipeFlavor.instance.GetFlavourSo(flavorType);
        PanManager.instance.AcquireFlavor(flavorSo);
        foreach (var item in torchParts)
        {
            item.SetActive(false);
        }
    }
}
