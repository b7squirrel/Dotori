using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeFlavor : MonoBehaviour
{
    public static RecipeFlavor instance;

    public FlavorSo[] recipeFlavor;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// ������Ÿ�� Ȥ�� ��������� �ױ� ���� ȣ��
    /// </summary>
    public FlavorSo GetFlavourSo(Flavor.flavorType _flavorType)
    {
        foreach (var _recipe in recipeFlavor)
        {
            if (_flavorType == _recipe.flavorType)
            {
                return _recipe;
            }
        }
        return null;
    }
}
