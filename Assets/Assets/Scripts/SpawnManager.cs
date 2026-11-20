using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject foodPrefab;
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float foodSpawnInterval = 1.5f;
    public float enemySpawnInterval = 3f;
    public int maxFood = 20;
    public int maxEnemies = 8;

    [Header("Bounds - match your outer walls")]
    public float minX = -18f;
    public float maxX = 18f;
    public float minY = -9f;
    public float maxY = 9f;

    private PlayerController player;
    private float foodTimer;
    private float enemyTimer;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // Pre-spawn some food
        for (int i = 0; i < 10; i++) SpawnFood();
        for (int i = 0; i < 3; i++) SpawnEnemy();
    }

    void Update()
    {
        if (player == null) return;
        foodTimer += Time.deltaTime;
        enemyTimer += Time.deltaTime;

        if (foodTimer >= foodSpawnInterval)
        {
            foodTimer = 0;
            if (GameObject.FindGameObjectsWithTag("Food").Length < maxFood)
                SpawnFood();
        }
        if (enemyTimer >= enemySpawnInterval)
        {
            enemyTimer = 0;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
                SpawnEnemy();
        }
    }

    Vector2 GetRandomPos()
    {
        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void SpawnFood()
    {
        if (foodPrefab == null || player == null) return;
        GameObject obj = Instantiate(foodPrefab, GetRandomPos(), Quaternion.identity);
        Food food = obj.GetComponent<Food>();
        // Food always smaller than player, relative size
        float foodSize = Random.Range(0.3f, player.GetSize() * 0.8f);
        food.SetSize(Mathf.Clamp(foodSize, 0.2f, 2f));
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null) return;
        GameObject obj = Instantiate(enemyPrefab, GetRandomPos(), Quaternion.identity);
        Enemy enemy = obj.GetComponent<Enemy>();
        // Max 1.5x player size, min 0.5x
        float enemySize = Random.Range(player.GetSize() * 0.5f, player.GetSize() * 1.5f);
        enemy.SetSize(enemySize);
        // Link managers
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) { enemy.gameManager = gm; enemy.audioManager = FindObjectOfType<AudioManager>(); }
    }
}