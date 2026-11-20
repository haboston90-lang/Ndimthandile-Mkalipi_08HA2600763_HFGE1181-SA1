using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 touchTarget;
    private bool isTouchMoving = false;

    [Header("Size / Satiation")]
    public float size = 1f;
    public float satiation = 0f;
    public float satiationThreshold = 10f;
    public float sizeGrowth = 0.2f;

    [Header("References")]
    public GameManager gameManager;
    public AudioManager audioManager;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        size = 1f;
        UpdateScale();
    }

    void Update()
    {
        // Keyboard for Editor testing
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0)
        {
            moveInput = new Vector2(h, v).normalized;
            isTouchMoving = false;
        }
        // Touch - drag to move / follow finger
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchTarget = worldPos;
            isTouchMoving = true;
            moveInput = ((Vector2)worldPos - rb.position).normalized;
            // Stop if very close to finger
            if (Vector2.Distance(rb.position, touchTarget) < 0.3f)
                moveInput = Vector2.zero;
        }
        else
        {
            moveInput = Vector2.zero;
            isTouchMoving = false;
        }

        if (animator != null)
            animator.SetBool("isMoving", moveInput != Vector2.zero);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void AddSatiation(float amount)
    {
        if (gameManager != null && gameManager.isGameOver) return;
        satiation += amount;
        if (satiation >= satiationThreshold)
        {
            satiation = 0;
            size += sizeGrowth;
            UpdateScale();
            if (audioManager != null) audioManager.PlayGrow();
        }
        if (gameManager != null) gameManager.UpdateHUD(size, satiation);
    }

    public void UpdateScale()
    {
        transform.localScale = new Vector3(size, size, 1f);
    }

    public float GetSize() { return size; }

    public void Die()
    {
        if (gameManager != null) gameManager.GameOver();
        gameObject.SetActive(false);
    }

    // Eating is handled here via triggers
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Food food = other.GetComponent<Food>();
            if (food != null && food.size < size)
            {
                food.BeingEaten(transform);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.size < size)
                {
                    // Enemy is smaller - player eats it
                    enemy.BeingEaten(transform);
                }
                else if (enemy.size > size)
                {
                    // Enemy is larger - enemy eats player
                    // Start being pulled to enemy
                    StartCoroutine(PulledToEnemy(enemy.transform));
                }
            }
        }
    }

    System.Collections.IEnumerator PulledToEnemy(Transform enemyTransform)
    {
        if (audioManager != null) audioManager.PlayHit();
        if (gameManager != null) gameManager.SpawnHitParticles(transform.position);

        // Pull towards enemy centre over 0.5s
        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f && enemyTransform != null)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, enemyTransform.position, t);
            yield return null;
        }
        Die();
    }
}