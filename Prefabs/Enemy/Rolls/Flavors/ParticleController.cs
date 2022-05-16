using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일단 numberOfRoll를 1로 해두었다. 나중에 Merge를 구현하면 그 때 값을 받아서 그에 맞는 파티클을 재생하도록 하자.
/// </summary>
public class ParticleController : MonoBehaviour
{
    private ParticleSystem ps;
    public float sizeMultiplier;
    public float lifeTimeMultiplier;
    public float shapeRadius;
    public float shapeAngle;
    public bool emission;
    
    public bool isFollowing;
    Transform _rollToFollow;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        var psMain = ps.main;
        var psShape = ps.shape;
        var psEmission = ps.emission;
        psEmission.enabled = true;
        
        psMain.startLifetimeMultiplier = 0;
        psMain.startSizeMultiplier = .1f;

        psShape.radius = .01f;
        psShape.angle = 15;
    }

    private void Update()
    {
        if (isFollowing)
        {
            FollowSlot();
        }
    }

    public void SetSlotToFollow(PanSlot _slot)
    {
        _rollToFollow = _slot.GetComponent<PanSlot>().GetRoll();
        isFollowing = true;
    }

    /// <summary>
    /// 롤이 땅이나 적에게 부딪쳐서 파괴되어 버렸을 때는 더 이상 Follow를 수행하지 못하도록 스스로 파괴
    /// </summary>
    void FollowSlot()
    {
        if (_rollToFollow != null)
        {
            transform.position = _rollToFollow.position;
        }
        else
        {
            DestroyParticle();
        }
    }

    public void DestroyParticle()
    {
        isFollowing = false;
        Destroy(gameObject);
    }
}
