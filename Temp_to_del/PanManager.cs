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
    BoxCollider2D _boxCol;
    FlavorSo m_flavorSo;
    float flavorCounter;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _panSlots = GetComponentsInChildren<PanSlot>();
        _boxCol = GetComponent<BoxCollider2D>();
        m_flavorSo = defaultFlavorSo;
    }
    private void Update()
    {
        if (m_flavorSo.flavorType == Flavor.flavorType.none)
        {
            m_flavorSo.flavorType = Flavor.flavorType.none;
            return;
        }
        flavorCounter -= Time.deltaTime;
    }

    // �� ������ ������ ĸ�İ� ������� �����Ƿ� AcquireRoll�� ȣ��Ǿ��� ���� ������ �� ������ ����
    public void AcquireRoll(Transform _prefab)
    {
        if (IsAvailableToCapture() == false)
            return;
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
            AcquireFlavor(m_flavorSo);
    }

    //������ ���鼭 �Ű������� ���� flavorSO���� �ش� ����Ʈ�� �����ϰ� �� ������ ���󰡰� �Ѵ�. 
    //������ �ѵ鿡�� isFlavored = true ���� �����Ѵ�. 
    public void AcquireFlavor(FlavorSo _flavorSo)
    {
        for (int i = 0; i < _panSlots.Length; i++)
        {
            if (!_panSlots[i].IsEmpty())
            {
                var _clone = Instantiate(_flavorSo.flavorParticle, _panSlots[i].transform.position, _flavorEffectRot.rotation);
                _clone.GetComponent<ParticleController>().SetSlotToFollow(_panSlots[i]);

                _panSlots[i].GetRoll().GetComponent<EnemyRolling>().isFlavored = true;
                _panSlots[i].GetRoll().GetComponent<EnemyRolling>().m_flavorSO = _flavorSo;
            }
        }
        m_flavorSo = _flavorSo;
        m_flavorSo.flavorType = _flavorSo.flavorType;
        flavorCounter = flavorTime;
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

    public void ClearRoll(){
        int _numberOfRolls = CountRollNumber();
        GameObject _roll = _panSlots[0].GetRoll().gameObject;
        _roll.transform.position = hitRollPoint.position;

        if (_roll.GetComponent<EnemyRolling>().isFlavored)
        {
            _roll.tag = "RollFlavored";
            _roll.GetComponent<EnemyRolling>().isFlavored = false;
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

        _panSlots[0].RemoveRoll();

        // 0�� ������ ���� ����ְ� �� ������ �ϳ� ����
        for (int i = 0; i < _numberOfRolls - 1; i++){
            _panSlots[i + 1].MoveRoll(_panSlots[i]);
        }
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
