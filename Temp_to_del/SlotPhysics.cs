using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPhysics : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] Transform[] slots = new Transform[3];
    [SerializeField] Transform slotPhysicsBase;

    [Header("Toss Rolls")]
    [SerializeField] Transform anchor;
    [SerializeField] float tossForce;
    [SerializeField] float tossGravity;
    [SerializeField] float tossGravityScale;
    
    [Header("Toss Debug")]
    [SerializeField] float tossVelocity;
    [SerializeField] Transform anchorInitialPoint;
    [SerializeField] GameObject dotDebugging;
    [SerializeField] Transform targetDebugging;
    GameObject dotOnAnchor;

    [field: SerializeField]
    public bool IsAnchorGrounded { get; private set; }

    [field: SerializeField]
    public bool IsRollsOnPan { get; private set; }

    void Start()
    {
        InitSlotPosition();
        anchor.position = anchorInitialPoint.position;
        transform.parent = null;
        //dotOnAnchor = Instantiate(dotDebugging, targetDebugging.position, Quaternion.identity);
        //dotOnAnchor.transform.parent = anchorInitialPoint;
    }

    private void Update()
    {
        AnchorGroundCheck();
        CheckIsRollsOnPan();
        OnTossRolls();
        Follow();
    }

    void InitSlotPosition()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].localPosition = new Vector2(0, i);
        }
    }

    void AnchorGroundCheck()
    {
        if (tossVelocity > 0) // ������ grounded�� �����Ǿ�� ��. �ö󰡴� ������ ground���� ����
        {
            IsAnchorGrounded = false;
            return;
        }
        if (Vector2.Distance(anchor.position, anchorInitialPoint.position) < .1f)
        {
            IsAnchorGrounded = true;
            //anchor.position = anchorInitialPoint.position;
        }
    }
    void CheckIsRollsOnPan()
    {
        // ���� ������ �پ� �ִ°�
        if (PanManager.instance.CountRollNumber() > 0 && IsAnchorGrounded)
        {
            IsRollsOnPan = true;
            return;
        }
        IsRollsOnPan = false;
    }
    /// <summary>
    /// ���� �Ʒ������� ������. �� ���� �������� �ӵ��� 0���� ����
    /// </summary>
    void OnTossRolls()
    {
        tossVelocity += -tossGravity * tossGravityScale * Time.deltaTime;
        if (IsAnchorGrounded == true && tossVelocity < 0)
        {
            tossVelocity = 0;
        }
        transform.Translate(new Vector3(0, tossVelocity, 0) * Time.deltaTime);
    }
    void Follow()
    {
        // slot physics set
        float _xPosition = slotPhysicsBase.position.x;
        float _yPosition = transform.position.y;
        if (IsAnchorGrounded)
        {
            _yPosition = slotPhysicsBase.position.y;
        }
        _yPosition = Mathf.Clamp(_yPosition, slotPhysicsBase.position.y, _yPosition);
        transform.position = new Vector2(_xPosition, _yPosition);

        // anchor initial point
        anchorInitialPoint.position = slotPhysicsBase.position;
    }
    public void TossRolls()
    {
        tossVelocity = tossForce;
    }
}
