using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실질적인 Roll, Flavor 생성
/// </summary>
public class CookingSystem : MonoBehaviour
{
    public static CookingSystem instance;

    Inventory inventory;
    RecipeRoll myRecipeRoll;
    RecipeFlavor myRecipeFlavor;
    public RollSO outputRoll;
    public FlavorSo outputFlavor;

    public RollSO defaultRollSo;
    public FlavorSo defaultFlavorSo;

    Roll.rollType _rollNameOnPan;
    Flavor.flavorType _flavorNameOnPan;

    public Transform panPoint;
    public LayerMask rollLayer;
    public LayerMask flavorLayer;

    [Header("Frying Pan")]
    public GameObject roll_Slot;
    public GameObject flavor_Slot;
    Animator _rollSlot_animator;
    SpriteRenderer _rollSprite;
    SpriteRenderer _flavorSprite;
    Color _color;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        inventory = GetComponent<Inventory>();
        myRecipeRoll = GetComponent<RecipeRoll>();
        myRecipeFlavor = GetComponent<RecipeFlavor>();
        panPoint = FindObjectOfType<PlayerPanAttack>().GetComponent<PlayerPanAttack>().transform;
        rollLayer = FindObjectOfType<PlayerPanAttack>().GetComponent<PlayerPanAttack>().rollLayers;

        _rollSlot_animator = roll_Slot.GetComponent<Animator>();

        _rollSprite = roll_Slot.GetComponent<SpriteRenderer>();
        _color = _rollSprite.color;
        _color.a = 0;
        _rollSprite.color = _color;

        _flavorSprite = flavor_Slot.GetComponent<SpriteRenderer>();
        _color = _flavorSprite.color;
        _color.a = 0;
        _rollSprite.color = _color;

        outputFlavor = defaultFlavorSo;
        outputFlavor.flavorType = Flavor.flavorType.none;
    }
    private void Update()
    {
        Collider2D[] checkRoll = Physics2D.OverlapCircleAll(panPoint.position, .3f, rollLayer);
        if(checkRoll != null)
        {
            int _numberOfRollOnPan = 0;
            foreach (var item in checkRoll)
            {
                _numberOfRollOnPan++;
            }
        }
    }

    public void CreateRollOutput()
    {
        // pan에 올라와 있는 body가 어떤 body type으로 일치하는지
        _rollNameOnPan = inventory.InputSlots[0].GetRoll().rollSo.rollType;
        
        if(_rollNameOnPan == Roll.rollType.none)
        {
            ResetOutputs();
        }

        // body 갯수, 타입
        // recipe에서 검색해서 output roll 찾아내기
        for (int i = 0; i < myRecipeRoll.recipeRoll.Length; i++)
        {
            if(myRecipeRoll.recipeRoll[i].rollType == _rollNameOnPan)
            {
                outputRoll = myRecipeRoll.recipeRoll[i];
                outputRoll.rollType = myRecipeRoll.recipeRoll[i].rollType;

                // 후라이팬의 Roll Slot의 Sprite를 output sprite로 교체
                // roll아웃풋이 none이 아니라면 outputSO에서 sprite를 꺼내오고 ROll Slot의 알파값은 1로
                //_rollSprite.sprite = outputRoll.rollSprite[inventory.numberOfRolls - 1];
                _color.a = 1;
                _rollSprite.color = _color;

                // 애니메이터 오버라이드
                _rollSlot_animator.runtimeAnimatorController = outputRoll.roll_OverrideController[inventory.numberOfRolls - 1];

                // 롤이 커질수록 롤의 위치를 _offsetY만큼 올려준다. 매번 0,0 으로 초기화 시켜서 무한히 더해지지 않게 함
                //float _offsetY = .45f * (inventory.numberOfRolls - 1);
                //roll_Slot.transform.localPosition = new Vector2(0, 0);
                //roll_Slot.transform.localPosition = new Vector2(0,  _offsetY);

                return;
            }
        }
    }
    public void CreateFlavorOutput()
    {
        // pan에 올라와 있는 flavor가 어떤 flavor type으로 일치하는지
        _flavorNameOnPan = inventory.InputSlots[0].GetFlavor().flavorSo.flavorType;

        // flavor 갯수, 타입 
        // recipe에서 검색해서 output flavor 찾아내기
        //for (int i = 0; i < myRecipeFlavor.recipeFlavor.Length; i++)
        //{
        //    if (myRecipeFlavor.recipeFlavor[i].flavorType == _flavorNameOnPan)
        //    {
        //        outputFlavor = myRecipeFlavor.recipeFlavor[i];
        //        outputFlavor.flavorType = myRecipeFlavor.recipeFlavor[i].flavorType;

        //        // Flavor Particle 생성 (그 전에 이전에 생성되었던 파티클이 있으면 제거)
        //        ResetFlavorParticle();
        //        GameObject _flavorParticle = Instantiate(outputFlavor.flavorParticle, roll_Slot.transform.position, Quaternion.identity);
        //        _flavorParticle.transform.parent = roll_Slot.transform;
        //        _flavorParticle.GetComponent<ParticleController>().numberOfRolls = inventory.numberOfRolls;
        //        _flavorParticle.transform.localEulerAngles = new Vector3(-90, 0, 0);
        //        return;
        //    }
        //}
    }
    public void ResetOutputs()
    {
        _color.a = 0;
        _rollSprite.color = _color;

        // Flavor의 파티클 리셋
        ResetFlavorParticle();
    }

    private void ResetFlavorParticle()
    {
        ParticleController[] _particles = roll_Slot.GetComponentsInChildren<ParticleController>();
        if(_particles != null)
        {
            foreach (var item in _particles)
            {
                item.GetComponent<ParticleController>().DestroyParticle();
            }
        }
    }
}
