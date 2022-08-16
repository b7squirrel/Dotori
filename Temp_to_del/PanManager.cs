using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ ������Ʈ�� ��ġ������ �����Ѵ�
/// </summary>
public class PanManager : MonoBehaviour
{
    public static PanManager instance;
    
    [SerializeField] PanSlot spareSlot;
    [SerializeField] Transform _flavorEffectRot;  //��ƼŬ�� �̻��ϰ� �پ x������ -90�� ȸ����Ŵ
    [SerializeField] Transform hitRollPoint;  // HitRoll�� �� �� �ڲ� ���� �׶��� ������ ���鼭 �����. �׷��� �� ����� �߻��غ�
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
        // ���� ���� �� �ִٸ� �� ���� ��ġ�� Roll�� ����Ʈ����. 
        if (IsAvailableToCapture() == false)
        {
            DropRoll();
        }
        // ���� �ѵ��� �� ĭ�� �ø�
        for (int i = _panSlots.Length - 1; i > -1; i--)
        {
            if (_panSlots[i].IsEmpty == false)
            {
                _panSlots[i].MoveRoll(_panSlots[i + 1]);
            }
        }
        // �׸��� ���� �Ʒ�ĭ�� ĸ���� ���� �������
        _panSlots[0].AddRoll(_prefab);
        // Flavored ���¶�� �ѿ� �÷��̹��� ������
        if (flavorCounter > 0)
            AcquireFlavor(currentFlavourSo);
    }

    //������ ���鼭 �Ű������� ���� flavorSO���� �ش� ����Ʈ�� �����ϰ� �� ������ ���󰡰� �Ѵ�. 
    //������ �ѵ鿡�� isFlavored = true ���� �����Ѵ�. 
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
                    // ��ƼŬ ���ְ�, Flavour ���� �ʱ�ȭ, FlavourSo �ʱ�ȭ
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
    /// enemy�� roll�� ����� ���Կ� ������ rigidBody�� collider�� ���� �����̴�.
    /// Hit Roll�� �� �� rigidBody�� collider�� �ٿ��� ��������
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

        // 0�� ������ ���� ����ְ� �� ������ �ϳ� ����
        _panSlots[0].RemoveRoll();
        for (int i = 0; i < _numberOfRolls - 1; i++){
            _panSlots[i + 1].MoveRoll(_panSlots[i]);
        }
    }

    /// <summary>
    /// enemy�� roll�� ����� ���Կ� ������ rigidBody�� collider�� ���� �����̴�.
    /// Hit Roll�� �� �� rigidBody�� collider�� �ٿ��� ��������
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
    /// �÷��̾ ������ Roll�� ��� ����Ʈ��
    /// </summary>
    public void DropAllRolls()
    {
        int _numberOfRolls = CountRollNumber();  // CountRollNumber�� 0, 1, 2 ���Թ�ȣ�� ����Ų��
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
