using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiles : MonoBehaviour
{
    public Tilemap tileMap;
    public GameObject debrisParticleEffect;


    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
    }
    public void RemoveTile(Vector2 _point)
    {
        Vector3Int _cellPosition = tileMap.WorldToCell(_point);
        if (tileMap.GetTile(_cellPosition) != null)
        {
            tileMap.SetTile(_cellPosition, null);
            GenerateDebris(_cellPosition);
        }
        
    }
    private void GenerateDebris(Vector3 _DebrisPoint)
    {
        Instantiate(debrisParticleEffect, _DebrisPoint, Quaternion.identity);
    }
}
