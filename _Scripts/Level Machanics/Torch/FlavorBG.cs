using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorBG : MonoBehaviour
{
    [SerializeField] GameObject[] torchParts;
    [SerializeField] Flavor.flavorType myFlavorType;
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
        FlavorSo _flavorSo = RecipeFlavor.instance.GetFlavourSo(myFlavorType);
        PanManager.instance.AcquireFlavor(_flavorSo);
        
        foreach (var item in torchParts)
        {
            item.SetActive(false);
        }
    }
}
