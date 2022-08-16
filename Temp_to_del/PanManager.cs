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
        // 팬이 가득 차 있다면 맨 위에 위치한 Roll을 떨어트린다. 
        if (IsAvailableToCapture() == false)
        {
            DropRoll();
        }
        // 기존 롤들을 한 칸씩 올림
        for (int i = _panSlots.Length - 1; i > -1; i--)
        {
            if (_panSlots[i].IsEmpty == false)
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
            if (!_panSlots[i].IsEmpty)
            {
                flavourParticle[i] = Instantiate(_flavorSo.flavorParticle, _panSlots[i].transform.position, _flavorEffectRot.rotation);
                flavourParticle[i].GetComponent<ParticleController>().SetSlotToFollow(_panSlots[i]);

                _panSlots[i].GetRoll().GetComponent<EnemyRolling>().SetFlavor(true, _flavorSo);
;            }
        }
        currentFlavourSo = _flavorSo;
        flavorCounter = flavorTime;
    }
    void ResetFlavour()
    {
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (!_panSlots[i].IsEmpty)
            {
                if (flavourParticle[i] != null)
                {
                    // 파티클 없애고, Flavour 상태 초기화, FlavourSo 초기화
                    flavourParticle[i].GetComponent<ParticleController>().DestroyParticle();
                    _panSlots[i].GetRoll().GetComponent<EnemyRolling>().SetFlavor(false, defaultFlavorSo);
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
        if (_panSlots[0].IsEmpty)
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

    /// <summary>
    /// enemy를 roll로 만들어 슬롯에 넣으면 rigidBody나 collider가 없는 상태이다.
    /// Hit Roll을 할 때 rigidBody와 collider를 붙여서 내보낸다
    /// </summary>
    public void ClearRoll()
    {
        int _numberOfRolls = CountRollNumber();
        GameObject _roll = _panSlots[0].GetRoll().gameObject;
        _roll.transform.position = hitRollPoint.position;

        if (_roll.GetComponent<EnemyRolling>().IsFlavored)
        {
            _roll.tag = "RollFlavored";
        }
        else
        {
            _roll.tag = "Rolling";
        }
        AddPhysics(_roll);
        float _hSpeed = _roll.GetComponent<EnemyRolling>().horizontalSpeed;
        float _vSpeed = _roll.GetComponent<EnemyRolling>().verticalSpeed;
        //Vector2 _mouseDirection = playerTargetController.GetMouseDirection();
        //_roll.GetComponent<Rigidbody2D>().velocity = 
        //    new Vector2(_mouseDirection.x * _hSpeed, _mouseDirection.y * _hSpeed);
        _roll.GetComponent<Rigidbody2D>().velocity =
            new Vector2(_hSpeed * PlayerController.instance.GetPlayerDirection(), _hSpeed);

        // 0번 슬롯의 롤을 비워주고 롤 갯수를 하나 줄임
        _panSlots[0].RemoveRoll();
        for (int i = 0; i < _numberOfRolls - 1; i++){
            _panSlots[i + 1].MoveRoll(_panSlots[i]);
        }
    }

    /// <summary>
    /// enemy를 roll로 만들어 슬롯에 넣으면 rigidBody나 collider가 없는 상태이다.
    /// Hit Roll을 할 때 rigidBody와 collider를 붙여서 내보낸다
    /// </summary>
    void DropRoll()
    {
        GameObject _roll = _panSlots[_panSlots.Length - 1].GetRoll().gameObject;

        if (_roll.GetComponent<EnemyRolling>().IsFlavored)
        {
            _roll.tag = "RollFlavored";
        }
        else
        {
            _roll.tag = "Rolling";
        }
        AddPhysics(_roll);
        float _vSpeed = _roll.GetComponent<EnemyRolling>().verticalDropSpeed;
        _roll.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1f, 1f), _vSpeed);

        _panSlots[_panSlots.Length - 1].RemoveRoll();
    }
    void AddPhysics(GameObject _roll)
    {
        float _direction = PlayerController.instance.GetPlayerDirection();
        
        PhysicsMaterial2D _physicsMat = _roll.GetComponent<EnemyRolling>().physicsMat;
        Rigidbody2D _theRB = _roll.AddComponent<Rigidbody2D>();
        CapsuleCollider2D _capColl = _roll.AddComponent<CapsuleCollider2D>();
        CapsuleCollider2D _capCollwTrigger = _roll.AddComponent<CapsuleCollider2D>();

        _theRB.sharedMaterial = _physicsMat;
        _theRB.mass = _roll.GetComponent<EnemyRolling>().mass;
        _theRB.gravityScale = _roll.GetComponent<EnemyRolling>().dropGravity;
        _theRB.angularDrag = 5;
        _capColl.size = new Vector2(1f, 1f);
        _capColl.direction = CapsuleDirection2D.Horizontal;
        _capCollwTrigger.size = new Vector2(1f, 1f);
        _capCollwTrigger.direction = CapsuleDirection2D.Horizontal;
        _capCollwTrigger.isTrigger = true;
    }

    /// <summary>
    /// 플레이어가 죽으면 Roll을 모두 떨어트림
    /// </summary>
    public void DropAllRolls()
    {
        int _numberOfRolls = CountRollNumber();  // CountRollNumber는 0, 1, 2 슬롯번호를 가리킨다
        float[] _vSpeeds = new float[_numberOfRolls];

        GameObject[] _rolls = new GameObject[_numberOfRolls];
        PhysicsMaterial2D[] _physicsMats = new PhysicsMaterial2D[_numberOfRolls];
        Rigidbody2D[] _theRBs = new Rigidbody2D[_numberOfRolls];
        CapsuleCollider2D[] _capColls = new CapsuleCollider2D[_numberOfRolls];
        CapsuleCollider2D[] _capCollwTriggers = new CapsuleCollider2D[_numberOfRolls];
        for (int i = 0; i < _rolls.Length; i++)
        {
            _rolls[i] = _panSlots[i].GetRoll().gameObject;
            if (_rolls[i].GetComponent<EnemyRolling>().IsFlavored)
            {
                _rolls[i].tag = "RollFlavored";
            }
            else
            {
                _rolls[i].tag = "Rolling";
            }
            _physicsMats[i] = _rolls[i].GetComponent<EnemyRolling>().physicsMat;
            _theRBs[i] = _rolls[i].AddComponent<Rigidbody2D>();
            _theRBs[i].mass = _rolls[i].GetComponent<EnemyRolling>().mass;
            _capColls[i] = _rolls[i].AddComponent<CapsuleCollider2D>();
            _capCollwTriggers[i] = _rolls[i].AddComponent<CapsuleCollider2D>();
            _vSpeeds[i] = _rolls[i].GetComponent<EnemyRolling>().verticalDropSpeed;

            _theRBs[i].sharedMaterial = _physicsMats[i];
            _capColls[i].size = new Vector2(1f, 1f);
            _capCollwTriggers[i].size = new Vector2(1f, 1f);
            _capCollwTriggers[i].direction = CapsuleDirection2D.Horizontal;
            _capCollwTriggers[i].isTrigger = true;
            _theRBs[i].gravityScale = _rolls[i].GetComponent<EnemyRolling>().dropGravity;
            _theRBs[i].velocity = new Vector2(Random.Range(-1f, 1f), _vSpeeds[i]);

            _panSlots[i].RemoveRoll();
        }
    }

    public int CountRollNumber()
    {
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (_panSlots[i].IsEmpty)
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
