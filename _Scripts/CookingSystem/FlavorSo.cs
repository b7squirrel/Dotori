using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Flavor", menuName = "FlavorSO")]
public class FlavorSo : ScriptableObject
{
    public Flavor.flavorType flavorType;
    [Header("Sprite Displayed on UI")]
    public Sprite flavorSprite_UI;
    
    [Header("Sprite on the Roll on the Pan")]
    public Sprite[] flavorSprite;

    [Header("Particle on the Roll on the Pan")]
    public GameObject flavorParticle;

    [Header("Prefab Generated When the Roll Hits Something")]
    public GameObject actionPrefab;
}
