using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    [field: Header("Refs")]
    [field: SerializeField] public Rigidbody2D Rb { get; protected set; }
    [field: SerializeField] public Transform Sprite { get; protected set; }


    [field: Header("State")]
    [field: SerializeField] bool isPulsing = false;
    [field: SerializeField] public bool IsFalling { get; protected set; } = true;
    [field: SerializeField] public bool IsHidden { get; protected set; } = false;

    [field: Header("Properties")]
    [field: SerializeField] public TileType TileType { get; protected set; }
    [field: SerializeField] public float HideDuration { get; protected set; } = 0.1f;


    [field: Header("Pulse")]
    [field: SerializeField] public float PulseSpeed { get; protected set; } = 2f;
    [field: SerializeField] public float PulseSize { get; protected set; } = 0.2f;

    [field: Header("Neighboring Tiles")]
    [field: SerializeField] public List<Tile> Neighbors { get; protected set; } = new List<Tile>();

    float timer;
    float hideTimer;

    Vector3 baseScale;

    public void SetNeighbors(List<Tile> neighbors)
    {
        Neighbors = neighbors;
    }

    public void RemoveNeighbor(Tile tile)
    {
        Neighbors.Remove(tile);
    }

    public void AddNeighbor(Tile tile)
    {
        Neighbors.Add(tile);
    }

    public void ClearNeighbors()
    {
        Neighbors.Clear();
    }

    public void ToggleOnPulsing()
    {
        isPulsing = true;
    }

    public void TurnOffPulsing()
    {
        Sprite.localScale = baseScale;
        timer = 0f;
        isPulsing = false;
    }

    public void TurnOffPhysics()
    {
        Rb.bodyType = RigidbodyType2D.Kinematic;
        Rb.linearVelocity = Vector2.zero;
    }

    public void TurnOnPhysics()
    {
        Rb.bodyType = RigidbodyType2D.Dynamic;
        Rb.linearVelocity = Vector2.down * Time.fixedDeltaTime * 0.5f;
    }

    public void DestroyTile()
    {
        GameManager.Instance.IncrementScore();
        Destroy(gameObject);
    }

    public void DestroyNoPoints()
    {
        Destroy(gameObject);
    }

    public bool ProcessMatch()
    {
        TileType targetType = TileType;

        Queue<Tile> queue = new Queue<Tile>();
        HashSet<Tile> connectedTiles = new HashSet<Tile>();

        queue.Enqueue(this);
        connectedTiles.Add(this);

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            foreach (Tile neighbor in current.Neighbors)
            {
                if (neighbor != null && neighbor.TileType == targetType && !connectedTiles.Contains(neighbor) && !neighbor.IsFalling)
                {
                    connectedTiles.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (connectedTiles.Count >= 3)
        {
            foreach (Tile t in connectedTiles)
            {
                t.DestroyTile();
                GameManager.Instance.Matched();
            }
            return true;
        }

        return false;
    }

    void AddNeighboringTiles(Collider2D other)
    {
        if (!other.CompareTag("Tile")) return;
        Tile tile = other.gameObject.GetComponent<Tile>();
        if (!Neighbors.Contains(tile)) Neighbors.Add(tile);
    }

    void RemoveNeighboringTiles(Collider2D other)
    {
        if (!other.CompareTag("Tile")) return;
        Tile tile = other.gameObject.GetComponent<Tile>();
        if (Neighbors.Contains(tile)) Neighbors.Remove(tile);
    }

    void PickForSwapping()
    {
        Swapper.Instance.PickTile(this);
    }

    void Pulse(float time)
    {
        float pulse = Mathf.Sin(time * PulseSpeed) * 0.5f + 0.5f;
        float scale = 1f + pulse * PulseSize;
        Sprite.localScale = baseScale * scale;
    }

    void ProcessPulsing()
    {
        if (isPulsing) Pulse(timer);
        else return;

        if (timer >= Mathf.PI)
        {
            timer = 0;
        }
        timer += Time.fixedDeltaTime;
    }

    void ProcessRevealing()
    {
        if (!IsHidden) return;

        if (hideTimer >= HideDuration)
        {
            hideTimer = 0;
            Show();
        }
        hideTimer += Time.fixedDeltaTime;
    }

    void Hide()
    {
        Sprite.GetComponent<SpriteRenderer>().enabled = false;
        IsHidden = true;
    }

    void Show()
    {
        Sprite.GetComponent<SpriteRenderer>().enabled = true;
        IsHidden = false;
    }

    void OnTriggerStay2D(Collider2D other) {
        AddNeighboringTiles(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        RemoveNeighboringTiles(other);
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.Moves > 0 && !Swapper.Instance.IsSwapping) PickForSwapping();
    }

    void Awake()
    {
        if (Rb == null) Rb = GetComponent<Rigidbody2D>();
        if (Sprite == null) Sprite = GetComponentInChildren<SpriteRenderer>().transform;
        GameManager.OnMatch += TurnOnPhysics;
        GameManager.OnGameStart += DestroyNoPoints;
    }

    void OnDestroy()
    {
        if (Swapper.Instance.QueuedTiles.Contains(this)) Swapper.Instance.RemoveTile(this);
        GameManager.OnMatch -= TurnOnPhysics;
        GameManager.OnGameStart -= DestroyNoPoints;
    }

    void Start()
    {
        baseScale = Sprite.localScale;
        TurnOnPhysics();
        // Hide();
    }

    void FixedUpdate()
    {
        // ProcessRevealing();
        if (Rb.linearVelocityY > 0)
        {
            TurnOffPhysics();
            IsFalling = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            ProcessMatch();
        }
        ProcessPulsing();
    }
}
