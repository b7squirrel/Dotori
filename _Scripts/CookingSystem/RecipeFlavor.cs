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
    /// 프로젝타일 혹은 마법사들이 죽기 전에 호출
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
