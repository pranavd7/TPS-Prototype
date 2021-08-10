using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemyToSpawn = 5;
    [SerializeField] float firstWaveInterval = 3;
    [SerializeField] float spawnInterval = 1f;
    [SerializeField] float waveCooldownTime = 2f;
    public static int waveNumber = 0;

    AiAgent enemyInScene;
    bool spawningWave;
     float spawnCooldownTime;

    Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            spawnPoints[i] = transform.GetChild(i);
        if (GameManager.gm.isCompleted)
            StartCoroutine(SpawnEnemyWave(++waveNumber));
        else
            StartCoroutine(SpawnEnemyWave(waveNumber));
    }

    // Update is called once per frame
    void Update()
    {
        //enemyInScene = null;
        enemyInScene = FindObjectOfType<AiAgent>();
        //Debug.Log(enemyInScene);

        if (enemyInScene == null && !spawningWave)
        {
            if (GameManager.gm.isCompleted)
            {
                waveNumber++;
                StartCoroutine(SpawnEnemyWave(waveNumber));
            }
            else
            {
                GameManager.gm.wave1finished = true;
            }
        }
    }

    void SpawnEnemy()
    {
        int i = Random.Range(0, spawnPoints.Length);
        Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
    }

    IEnumerator SpawnEnemyWave(int waveNumber)
    {
        spawningWave = true;
        int enemiesToSpawn;
        if (waveNumber == 0)
        {
            enemiesToSpawn = maxEnemyToSpawn;
            spawnCooldownTime = firstWaveInterval;
        }
        else
        {
            enemiesToSpawn = 3 * waveNumber;
            spawnCooldownTime = spawnInterval;
        }

        yield return new WaitForSeconds(waveCooldownTime); //We wait here to pause between wave spawning

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnCooldownTime); //We wait here to give a bit of time between each enemy spawn
        }
        spawningWave = false;
    }
}
