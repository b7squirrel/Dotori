using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] float initialSpawnTime;
    [SerializeField] bool isPlayerIn;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;
    
    [Range(1f, 20f)]
    [SerializeField] float sizeX;
    [Range(1f, 20f)]
    [SerializeField] float sizeY;
    Vector2 size;
    float spawnCoolTime;
    float spawnTimeCounter;
    int currentIndex;

    private void Start()
    {
        spawnTimeCounter = initialSpawnTime;
        size = new Vector2(sizeX, sizeY);
    }
    private void Update()
    {
        CheckPlayerEnemies();
        Spawn();
    }
    void CheckPlayerEnemies()
    {
        Vector2 pointA = new Vector2(transform.position.x - sizeX/2, transform.position.y - sizeY/2);
        Vector2 pointB = new Vector2(transform.position.x + sizeX / 2, transform.position.y + sizeY / 2);
        isPlayerIn = Physics2D.OverlapArea(pointA , pointB, playerLayer);
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

        if (currentIndex > enemies.Length - 1)
        {
            currentIndex = 0;
        }

        Instantiate(enemies[currentIndex], transform.position, Quaternion.identity);
        currentIndex++;
        isPlayerIn = false;
        spawnCoolTime = initialSpawnTime + 3f;
        spawnTimeCounter = spawnCoolTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .2f);
        Gizmos.DrawCube(transform.position, size);
    }
}
