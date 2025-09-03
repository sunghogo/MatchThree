using UnityEngine;
using System.Collections.Generic;

public enum TileType
{
    Yellow,
    Purple,
    Blue,
    Red,
    Green
}

public class Tile : MonoBehaviour
{
    [field: Header("Refs")]
    [field: SerializeField] public Rigidbody2D Rb { get; protected set; }
    [field: SerializeField] public Transform Sprite { get; protected set; }

    [Header("State")]
    [SerializeField] bool isPulsing = false;

    [field: Header("Properties")]
    [field: SerializeField] public TileType TileType { get; protected set; }

    [field: Header("Pulse")]
    [field: SerializeField] public float PulseSpeed { get; protected set; } = 2f;
    [field: SerializeField] public float PulseSize { get; protected set; } = 0.2f;

    [field: Header("Neighboring Tiles")]
    [field: SerializeField] public List<Tile> Neighbors { get; protected set; } = new List<Tile>();

    float timer;
    Vector3 baseScale;

    public void SetNeighbors(List<Tile> neighbors)
    {
        Neighbors = neighbors;
    }

    public void RemoveNeighbor(Tile tile)
    {
        Neighbors.Remove(tile);
    }

    public void TogglePulsing()
    {
        if (isPulsing) Sprite.localScale = baseScale;
        timer = 0f;
        isPulsing = !isPulsing;
    }

    public void TurnOffPhysics()
    {
        Rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void TurnOnPhysics()
    {
        Rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void AddNeighboringTiles(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Tile")) return;
        Tile other = collision.gameObject.GetComponent<Tile>();
        if (!Neighbors.Contains(other)) Neighbors.Add(other);
    }

    void PickForSwapping()
    {
        TogglePulsing();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        AddNeighboringTiles(collision);
    }

    void OnMouseDown()
    {
        if (!Swapper.Instance.IsSwapping) PickForSwapping();
    }

    void Awake()
    {
        if (Rb == null) Rb = GetComponent<Rigidbody2D>();
        if (Sprite == null) Sprite = GetComponentInChildren<SpriteRenderer>().transform;
    }

    void Start()
    {
        baseScale = Sprite.localScale;
    }

    void FixedUpdate()
    {
        ProcessPulsing();
    }
}
