using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 들어오고 나가는 오브젝트의 위치값에만 관여한다
/// </summary>
public class PanManager : MonoBehaviour
{
    public static PanManager instance;
    
    [SerializeField] PanSlot spareSlot;
    [SerializeField] Transform _flavorEffectRot;  //파티클이 이상하게 붙어서 x축으로 -90도 회전시킴
    [SerializeField] Transform hitRollPoint;  // HitRoll을 할 때 자꾸 롤이 그라운드 판정이 나면서 사라짐. 그래서 좀 띄워서 발사해봄
    [SerializeField] PlayerTargetController playerTargetController;
    [SerializeField] FlavorSo defaultFlavorSo;
    [SerializeField] float flavorTime;
    PanSlot[] _panSlots;
    GameObject[] flavourParticle = new GameObject[3];
    BoxCollider2D _boxCol;
    FlavorSo currentFlavourSo;
    float flavorCounter;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _panSlots = GetComponentsInChildren<PanSlot>();
        _boxCol = GetComponent<BoxCollider2D>();
        currentFlavourSo = defaultFlavorSo;
    }
    private void Update()
    {
        ManagetFlavour();
    }

    public void AcquireRoll(Transform _prefab)
    {
        Debug.Log(IsAvailableToCapture());
        // 팬이 가득 차 있다면 맨 위에 위치한 Roll을 떨어트린다. 
        if (IsAvailableToCapture() == false)
        {
            DropRoll();
        }
        // 기존 롤들을 한 칸씩 올림
        for (int i = _panSlots.Length - 1; i > -1; i--)
        {
            if (_panSlots[i].isEmpty == false)
            {
                _panSlots[i].MoveRoll(_panSlots[i + 1]);
            }
        }
        // 그리고 가장 아래칸에 캡쳐한 롤을 집어넣음
        _panSlots[0].AddRoll(_prefab);
        // Flavored 상태라면 롤에 플레이버를 입혀줌
        if (flavorCounter > 0)
            AcquireFlavor(currentFlavourSo);
    }

    //슬롯을 돌면서 매개변수로 받은 flavorSO에서 해당 이펙트를 추출하고 각 슬롯을 따라가게 한다. 
    //슬롯의 롤들에게 isFlavored = true 값을 전달한다. 
    public void AcquireFlavor(FlavorSo _flavorSo)
    {
        ResetFlavour();
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (!_panSlots[i].IsEmpty())
            {
                flavourParticle[i] = Instantiate(_flavorSo.flavorParticle, _panSlots[i].transform.position, _flavorEffectRot.rotation);
                flavourParticle[i].GetComponent<ParticleController>().SetSlotToFollow(_panSlots[i]);

                _panSlots[i].GetRoll().GetComponent<EnemyRolling>().isFlavored = true;
                _panSlots[i].GetRoll().GetComponent<EnemyRolling>().m_flavorSO = _flavorSo;
;            }
        }
        currentFlavourSo = _flavorSo;
        flavorCounter = flavorTime;
    }
    void ResetFlavour()
    {
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (!_panSlots[i].IsEmpty())
            {
                if (flavourParticle[i] != null)
                {
                    // 파티클 없애고, Flavour 상태 초기화, FlavourSo 초기화
                    flavourParticle[i].GetComponent<ParticleController>().DestroyParticle();
                    _panSlots[i].GetRoll().GetComponent<EnemyRolling>().isFlavored = false;
                    _panSlots[i].GetRoll().GetComponent<EnemyRolling>().m_flavorSO = defaultFlavorSo;
                }
            }
        }
        currentFlavourSo = defaultFlavorSo;
        currentFlavourSo.flavorType = defaultFlavorSo.flavorType;
        flavorCounter = 0f;
    }

    void ManagetFlavour()
    {
        if (flavorCounter > 0)
        {
            flavorCounter -= Time.deltaTime;
        }
        else
        {
            ResetFlavour();
        }
    }
    public void FlipRoll()
    {
        if (_panSlots[0].IsEmpty())
            return;
        _panSlots[0].FlipRoll();
    }

    public void SwitchRolls()
    {
        if (CountRollNumber() == 1)
        {
            return;
        }
        for (int i = 0; i < CountRollNumber() - 1; i++)
        {
            _panSlots[i].MoveRoll(spareSlot);
            _panSlots[i + 1].MoveRoll(_panSlots[i]);
            spareSlot.MoveRoll(_panSlots[i + 1]);
        }
    }

    public void ClearRoll()
    {
        int _numberOfRolls = CountRollNumber();
        GameObject _roll = _panSlots[0].GetRoll().gameObject;
        _roll.transform.position = hitRollPoint.position;

        if (_roll.GetComponent<EnemyRolling>().isFlavored)
        {
            _roll.tag = "RollFlavored";
        }
        else
        {
            _roll.tag = "Rolling";
        }

        float _direction = PlayerController.instance.GetPlayerDirection();
        float _hSpeed = _roll.GetComponent<EnemyRolling>().horizontalSpeed;
        float _vSpeed = _roll.GetComponent<EnemyRolling>().verticalSpeed;
        PhysicsMaterial2D _physicsMat = _roll.GetComponent<EnemyRolling>().physicsMat;
        Rigidbody2D _theRB = _roll.AddComponent<Rigidbody2D>();
        CapsuleCollider2D _capColl = _roll.AddComponent<CapsuleCollider2D>();
        _theRB.sharedMaterial = _physicsMat;
        _capColl.size = new Vector2(1f, 1f);
        _capColl.direction = CapsuleDirection2D.Horizontal;
        _theRB.gravityScale = _roll.GetComponent<EnemyRolling>().gravity;
        Vector2 _mouseDirection = playerTargetController.GetMouseDirection();
        _theRB.velocity = new Vector2(_mouseDirection.x * _hSpeed, _mouseDirection.y * _hSpeed);

        // 0번 슬롯의 롤을 비워주고 롤 갯수를 하나 줄임
        _panSlots[0].RemoveRoll();
        for (int i = 0; i < _numberOfRolls - 1; i++){
            _panSlots[i + 1].MoveRoll(_panSlots[i]);
        }
    }

    void DropRoll()
    {
        GameObject _roll = _panSlots[_panSlots.Length - 1].GetRoll().gameObject;

        float _direction = PlayerController.instance.GetPlayerDirection();
        float _vSpeed = _roll.GetComponent<EnemyRolling>().verticalDropSpeed;
        PhysicsMaterial2D _physicsMat = _roll.GetComponent<EnemyRolling>().physicsMat;
        Rigidbody2D _theRB = _roll.AddComponent<Rigidbody2D>();
        CapsuleCollider2D _capColl = _roll.AddComponent<CapsuleCollider2D>();
        _theRB.sharedMaterial = _physicsMat;
        _capColl.size = new Vector2(1f, 1f);
        _capColl.direction = CapsuleDirection2D.Horizontal;
        _theRB.gravityScale = _roll.GetComponent<EnemyRolling>().dropGravity;
        _theRB.velocity = new Vector2(0, _vSpeed);

        _panSlots[_panSlots.Length - 1].RemoveRoll();
        Debug.Log("Dropped");
    }

    public int CountRollNumber()
    {
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (_panSlots[i].IsEmpty())
            {
                return i;
            }
        }
        return _panSlots.Length;
    }

    public bool IsAvailableToCapture()
    {
        if (CountRollNumber() < _panSlots.Length)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (_boxCol == null)
            return;
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(_boxCol.bounds.center, _boxCol.bounds.size);
    }
}
