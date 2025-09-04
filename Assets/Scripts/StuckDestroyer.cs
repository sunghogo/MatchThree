using UnityEngine;

public class StuckDestroyer : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float touchDuration = 0.1f;
    [SerializeField] float timer = 0f;
    [SerializeField] bool isTouchingTile = false;
    [SerializeField] Collider2D currentTile = null;

    void ResetTimer()
    {
        isTouchingTile = false;
        currentTile = null;
        timer = 0f;
    }

    void FixedUpdate()
    {
        if (isTouchingTile && currentTile != null)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= touchDuration)
            {
                Destroy(currentTile.gameObject);
                ResetTimer();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            isTouchingTile = true;
            currentTile = other;
            timer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentTile)
        {
            ResetTimer();
        }
    }
}
