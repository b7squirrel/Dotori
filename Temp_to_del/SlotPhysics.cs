using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPhysics : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] Transform[] slots = new Transform[3];

    [Header("Toss Rolls")]
    [SerializeField] Transform anchor;
    [SerializeField] float tossForce;
    [SerializeField] float tossGravity;
    [SerializeField] float tossGravityScale;
    
    [Header("Toss Debug")]
    [SerializeField] float tossVelocity;
    [SerializeField] Transform anchorInitialPoint;

    [field: SerializeField]
    public bool IsAnchorGrounded;

    [field: SerializeField]
    public bool IsRollsOnPan { get; private set; }

    void Start()
    {
        InitSlotPosition();
        anchor.position = anchorInitialPoint.position;
    }

    private void Update()
    {
        AnchorGroundCheck();
        CheckIsRollsOnPan();
        OnTossRolls();
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
        if (tossVelocity > 0) // 던지면 grounded가 해제되어야 함. 올라가는 동안은 ground되지 않음
        {
            IsAnchorGrounded = false;
            return;
        }
        if (Vector2.Distance(anchor.position, anchorInitialPoint.position) < .1f)
        {
            IsAnchorGrounded = true;
            anchor.position = anchorInitialPoint.position;
        }
    }
    void CheckIsRollsOnPan()
    {
        // 롤이 팬위에 붙어 있는가
        if (PanManager.instance.CountRollNumber() > 0 && IsAnchorGrounded)
        {
            IsRollsOnPan = true;
            return;
        }
        IsRollsOnPan = false;
    }
    void OnTossRolls()
    {
        tossVelocity += -tossGravity * tossGravityScale * Time.deltaTime;
        if (IsAnchorGrounded == true && tossVelocity < 0)
        {
            tossVelocity = 0;
        }
        anchor.transform.Translate(new Vector3(0, tossVelocity, 0) * Time.deltaTime);
    }
    public void TossRolls()
    {
        tossVelocity = tossForce;
    }
}
