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

    public FlavorSo GetFlavourSo(FlavorSo _flavorSo)
    {
        FlavorSo _temp = _flavorSo; // ���� �Ҵ��ؾ� �ϹǷ� �ƹ� ���̳� �Ҵ�����.
        foreach (var _recipe in recipeFlavor)
        {
            if (_flavorSo == _recipe)
            {
                _temp = _recipe;
            }
        }
        return _temp;
    }
}
