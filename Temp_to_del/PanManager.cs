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
        Debug.Log(IsAvailableToCapture());
        // ���� ���� �� �ִٸ� �� ���� ��ġ�� Roll�� ����Ʈ����. 
        if (IsAvailableToCapture() == false)
        {
            DropRoll();
        }
        // ���� �ѵ��� �� ĭ�� �ø�
        for (int i = _panSlots.Length - 1; i > -1; i--)
        {
            if (_panSlots[i].isEmpty == false)
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
                    // ��ƼŬ ���ְ�, Flavour ���� �ʱ�ȭ, FlavourSo �ʱ�ȭ
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

        // 0�� ������ ���� ����ְ� �� ������ �ϳ� ����
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
