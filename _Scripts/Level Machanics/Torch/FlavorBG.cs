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
    /// �־��� flaovrType���� flavorSo�� �����´�. ��ƼŬ�̳� ������ ��Ȱ��ȭ ��Ŵ
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
