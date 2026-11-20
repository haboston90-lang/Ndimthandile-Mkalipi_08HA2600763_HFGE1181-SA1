using UnityEngine;

public class Food : MonoBehaviour
{
    public float size = 0.5f;
    public float satiationValue = 1f;
    private bool isBeingEaten = false;
    private PlayerController player;
    private AudioManager audioManager;
    private GameManager gameManager;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
        transform.localScale = new Vector3(size, size, 1f);
    }

    public void SetSize(float newSize)
    {
        size = newSize;
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
        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f && eater != null)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(startPos, eater.position, t);
            yield return null;
        }
        if (audioManager != null) audioManager.PlayEat();
        if (player != null) player.AddSatiation(satiationValue);
        if (gameManager != null) gameManager.SpawnEatParticles(transform.position);
        Destroy(gameObject);
    }
}