using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Size")]
    public float size = 1f;
    public float satiationValue = 2f; // how much player gains when eating this

    [Header("Patrol")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    private Vector2 targetPos;
    private float timer;
    private Rigidbody2D rb;
    private PlayerController player;
    private bool isBeingEaten = false;

    [Header("References")]
    public GameManager gameManager;
    public AudioManager audioManager;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
        PickNewTarget();
        UpdateScale();
    }

    void Update()
    {
        if (isBeingEaten) return;
        if (player == null) return;

        // Self-destruct if smaller than 25% of player
        if (size < player.GetSize() * 0.25f)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer > changeDirectionTime || Vector2.Distance(transform.position, targetPos) < 0.5f)
        {
            PickNewTarget();
            timer = 0;
        }

        if (animator != null) animator.SetBool("isMoving", true);
    }

    void FixedUpdate()
    {
        if (isBeingEaten) return;
        Vector2 dir = ((Vector2)targetPos - rb.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    void PickNewTarget()
    {
        // Uses GameManager bounds so enemies never leave map
        if (gameManager != null)
        {
            float x = Random.Range(gameManager.minX, gameManager.maxX);
            float y = Random.Range(gameManager.minY, gameManager.maxY);
            targetPos = new Vector2(x, y);
        }
        else
        {
            targetPos = new Vector2(Random.Range(-15, 15), Random.Range(-8, 8));
        }
    }

    public void SetSize(float newSize)
    {
        size = newSize;
        UpdateScale();
    }

    void UpdateScale()
    {
        transform.localScale = new Vector3(size, size, 1f);
    }

    public void BeingEaten(Transform eater)
    {
        if (isBeingEaten) return;
        isBeingEaten = true;
        StartCoroutine(MoveToCentreAndDie(eater));
    }

    System.Collections.IEnumerator MoveToCentreAndDie(Transform eater)
    {
        if (audioManager != null) audioManager.PlayEat();
        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f && eater != null)
        {
            t += Time.deltaTime * 4f;
            transform.position = Vector3.Lerp(startPos, eater.position, t);
            yield return null;
        }
        if (player != null) player.AddSatiation(satiationValue);
        if (gameManager != null) gameManager.SpawnEatParticles(transform.position);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy colliding with player is handled in PlayerController
        // This handles enemy hitting player when player is smaller - for particles/sound
        if (other.CompareTag("Player") && size > player.GetSize())
        {
            if (audioManager != null) audioManager.PlayHit();
            if (gameManager != null) gameManager.SpawnHitParticles(transform.position);
        }
    }
}