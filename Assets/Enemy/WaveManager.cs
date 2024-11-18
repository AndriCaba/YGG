using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public int totalWaves = 5;
    public int enemiesPerWave = 10;
    public float timeBetweenWaves = 5f;

    [Header("UI Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesRemainingText;

    [Header("References")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    private int currentWave = 0;
    private int enemiesRemaining;
    private bool isSpawning = false;

    private void Start()
    {
        StartNextWave();
    }

    private void Update()
    {
        // Update UI
        waveText.text = $"Wave: {currentWave}/{totalWaves}";
        enemiesRemainingText.text = $"Enemies Remaining: {enemiesRemaining}";

        // Check for next wave condition
        if (enemiesRemaining <= 0 && !isSpawning && currentWave < totalWaves)
        {
            Invoke(nameof(StartNextWave), timeBetweenWaves);
            isSpawning = true;
        }
    }

    private void StartNextWave()
    {
        currentWave++;
        if (currentWave > totalWaves)
        {
            Debug.Log("All waves completed!");
            return;
        }

        enemiesRemaining = enemiesPerWave + (currentWave * 2); // Optional scaling
        isSpawning = false;

        Debug.Log($"Starting Wave {currentWave}");
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesRemaining; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            Debug.Log($"Wave {currentWave} completed!");
        }
    }
}
