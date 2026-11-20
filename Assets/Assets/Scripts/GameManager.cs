using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameOver = false;
    public float surviveTime = 600f; // 10 minutes
    public float elapsedTime = 0f;

    [Header("Bounds")]
    public float minX = -18f;
    public float maxX = 18f;
    public float minY = -9f;
    public float maxY = 9f;

    [Header("UI")]
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI satiationText;
    public GameObject pauseMenu;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public RectTransform enemyArrow; // Arrow UI image
    public PlayerController player;

    [Header("FX")]
    public GameObject eatParticlesPrefab;
    public GameObject hitParticlesPrefab;
    public AudioManager audioManager;

    void Start()
    {
        Time.timeScale = 1;
        isGameOver = false;
        if (pauseMenu) pauseMenu.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (audioManager != null) audioManager.PlayMusic();
    }

    void Update()
    {
        if (isGameOver) return;
        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
        UpdateEnemyArrow();

        if (elapsedTime >= surviveTime)
        {
            WinGame();
        }
    }

    public void UpdateHUD(float size, float satiation)
    {
        if (sizeText) sizeText.text = "Size: " + size.ToString("F1");
        if (satiationText) satiationText.text = "Satiation: " + satiation.ToString("F0");
    }

    void UpdateTimerUI()
    {
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateEnemyArrow()
    {
        if (enemyArrow == null || player == null) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) { enemyArrow.gameObject.SetActive(false); return; }
        enemyArrow.gameObject.SetActive(true);

        // Find nearest enemy
        GameObject nearest = enemies[0];
        float minDist = Vector2.Distance(player.transform.position, nearest.transform.position);
        foreach (GameObject e in enemies)
        {
            float d = Vector2.Distance(player.transform.position, e.transform.position);
            if (d < minDist) { minDist = d; nearest = e; }
        }

        Vector2 dir = (nearest.transform.position - player.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        enemyArrow.rotation = Quaternion.Euler(0, 0, angle - 90f);
        // Position arrow around player on screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position + (Vector3)dir * 2f);
        enemyArrow.position = screenPos;
    }

    public void SpawnEatParticles(Vector3 pos)
    {
        if (eatParticlesPrefab) { GameObject p = Instantiate(eatParticlesPrefab, pos, Quaternion.identity); Destroy(p, 1f); }
    }
    public void SpawnHitParticles(Vector3 pos)
    {
        if (hitParticlesPrefab) { GameObject p = Instantiate(hitParticlesPrefab, pos, Quaternion.identity); Destroy(p, 1f); }
    }

    public void GameOver()
    {
        isGameOver = true;
        if (audioManager != null) audioManager.StopMusic();
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void WinGame()
    {
        isGameOver = true;
        if (audioManager != null) audioManager.StopMusic();
        if (winPanel) winPanel.SetActive(true);
    }

    // UI Buttons
    public void PauseGame()
    {
        Time.timeScale = 0;
        if (pauseMenu) pauseMenu.SetActive(true);
        if (audioManager != null) audioManager.PlayClick();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pauseMenu) pauseMenu.SetActive(false);
        if (audioManager != null) audioManager.PlayClick();
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}