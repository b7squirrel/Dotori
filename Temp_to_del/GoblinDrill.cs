using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDrill : MonoBehaviour
{
    public Transform drillPoint;
    Vector2 _drillPoint;
    Vector2 _defaultDrillSize;
    public Vector2 drillSize;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public Transform rayOrigin;
    Vector2 _rayOrigin;
    public float rayLength;
    Vector2 _rayDirection;

    public GameObject debrisParticleEffect;

    [Header("Debug")]
    public GameObject debugDot;
    public bool debuggingOn;

    private void Start()
    {
        _defaultDrillSize = drillSize;
    }

    private void Update()
    {
        _drillPoint = drillPoint.position;
        _rayOrigin = rayOrigin.position;
        //UpdateDrillSize();
        DetectingGround();
        //SearchingTiles();
        //SearchingTilesTemp();
    }

    void SearchingTilesTemp()
    {
        _drillPoint = drillPoint.position;

        for (int i = (int)-drillSize.x; i <= drillSize.x; i++)
        {
            for (int j = (int)-drillSize.y; j <= (int)drillSize.y; j++)
            {
                Vector3 _cellPosition =
                    new Vector3(_drillPoint.x + i, _drillPoint.y + j, 0);

                Instantiate(debugDot, _cellPosition, Quaternion.identity);

                Collider2D _hitground = Physics2D.OverlapCircle(_cellPosition, .5f, groundLayer);
                if (_hitground != null)
                {
                    _hitground.GetComponent<Tiles>().RemoveTile(_cellPosition);
                }
            }
        }
    }

    /// <summary>
    /// 드릴이 90도 단위로 회전하므로 더 길쭉한 축 부분을 사이즈만큼 탐색한다
    /// </summary>
    void SearchingTiles()
    {
        _drillPoint = drillPoint.position;

        Vector3 _cellPosition =
                    new Vector3(_drillPoint.x, _drillPoint.y, 0);

        Collider2D _hitground = Physics2D.OverlapCircle(_cellPosition, .5f, groundLayer);
        if (_hitground != null)
        {
            _hitground.GetComponent<Tiles>().RemoveTile(_cellPosition);
        }

        Collider2D _hitenemy = Physics2D.OverlapCircle(_cellPosition, .5f, enemyLayer);
        if (_hitenemy != null)
        {
            _hitenemy.GetComponent<EnemyHealth>().Die();
        }

        DebugDestructionPoint(_cellPosition);
    }

    void DetectingGround()
    {
        Collider2D _hitground = Physics2D.OverlapCircle(_drillPoint, .1f, groundLayer);
        if (_hitground != null)
        {
            _hitground.GetComponent<Tiles>().RemoveTile(_drillPoint);
            Instantiate(debugDot, _drillPoint, Quaternion.identity);
        }
        _hitground = Physics2D.OverlapCircle(_rayOrigin, .1f, groundLayer);
        if (_hitground != null)
        {
            _hitground.GetComponent<Tiles>().RemoveTile(_rayOrigin);
            Instantiate(debugDot, _rayOrigin, Quaternion.identity);
        }
    }

    void UpdateDrillSize()
    {
        float _zRot = transform.localEulerAngles.z;
        if (_zRot == 0 || _zRot == 180f || _zRot == 360f)
        {
            drillSize = new Vector2(_defaultDrillSize.x, _defaultDrillSize.y);
        }
        else if (_zRot == 90f || _zRot == 270f)
        {
            drillSize = new Vector2(_defaultDrillSize.y, _defaultDrillSize.x);
        }
    }

    Vector2 GetGreaterOne(float x, float y)
    {
        if (x > y)
        {
            return new Vector2(1, 0);
        }
        else
        {
            return new Vector2(0, 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(_drillPoint, drillSize);
    }

    void DebugDestructionPoint(Vector3 dotPoint)
    {
        if (debuggingOn)
        {
            Instantiate(debugDot, dotPoint, Quaternion.identity);
        }
    }
}
