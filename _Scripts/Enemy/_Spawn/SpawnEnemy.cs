using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] float initialSpawnTime;
    [SerializeField] int maxSpawnEnemy;
    [SerializeField] bool isPlayerIn;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;

    [Range(1f, 20f)]
    [SerializeField] float sizeX = 16f;
    [Range(1f, 20f)]
    [SerializeField] float sizeY = 9f;
    [Range(-10f, 10f)]
    [SerializeField] float offsetX = 0;
    [Range(-10f, 10f)]
    [SerializeField] float offsetY = 0;
    Vector2 size;
    Vector2 offset;
    float spawnCoolTime;
    float spawnTimeCounter;
    int currentIndex;
    int numberOfSpawnedEnemy;

    private void Start()
    {
        spawnTimeCounter = initialSpawnTime;
    }
    private void Update()
    {
        CheckPlayerEnemies();
        Spawn();
    }
    void CheckPlayerEnemies()
    {
        size = new Vector2(sizeX, sizeY);
        offset = new Vector2(offsetX, offsetY);

        Vector2 _pointA = (Vector2)transform.position + offset - size / 2;
        Vector2 _pointB = (Vector2)transform.position + offset + size / 2;
        isPlayerIn = Physics2D.OverlapArea(_pointA , _pointB, playerLayer);
    }

    void Spawn()
    {
        
        if (isPlayerIn == false)
            return;
        if (spawnTimeCounter > 0f)
        {
            spawnTimeCounter -= Time.deltaTime;
            return;
        }
        if (numberOfSpawnedEnemy >= maxSpawnEnemy)
            return;

        if (currentIndex > enemies.Length - 1)
        {
            currentIndex = 0;
        }

        Instantiate(enemies[currentIndex], transform.position, Quaternion.identity);
        numberOfSpawnedEnemy++;
        currentIndex++;
        isPlayerIn = false;
        spawnCoolTime = initialSpawnTime + 3f;
        spawnTimeCounter = spawnCoolTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .2f);
        Gizmos.DrawCube((Vector2)transform.position + offset, size);
    }
}
