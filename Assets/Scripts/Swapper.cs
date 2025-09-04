using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Swapper : MonoBehaviour
{
    public static Swapper Instance { get; private set; }

    [Header("States")]
    [SerializeField] public bool IsSwapping { get; private set; } = false;

    [Header("Properties")]
    [SerializeField] private float duration = 0.25f;


    [field: Header("Queued Tiles for Swapping")]
    [field: SerializeField] public List<Tile> QueuedTiles { get; private set; } = new List<Tile>();

    public IEnumerator SwapRoutine(Tile tile1, Tile tile2, float duration = 1f)
    {
        // Save world positions of the tiles (these stay as the "grid positions")
        Vector3 gridPos1 = tile1.transform.position;
        Vector3 gridPos2 = tile2.transform.position;

        // Get sprite transforms (child objects)
        Transform sprite1 = tile1.Sprite.transform;
        Transform sprite2 = tile2.Sprite.transform;

        // Cache their starting positions
        Vector3 startSpritePos1 = sprite1.position;
        Vector3 startSpritePos2 = sprite2.position;

        float t = 0f;
        IsSwapping = true;

        // Lerp the sprites visually between the two slots
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            sprite1.position = Vector3.Lerp(startSpritePos1, gridPos2, t);
            sprite2.position = Vector3.Lerp(startSpritePos2, gridPos1, t);

            yield return new WaitForFixedUpdate(); 
        }

        // Snap sprites back to center of their new slots
        sprite1.position = gridPos2;
        sprite2.position = gridPos1;

        // Now swap the *tile objects* in the grid (so colliders move once)
        Vector3 temp = tile1.transform.position;
        tile1.transform.position = gridPos2;
        tile2.transform.position = temp;

        // Reset sprite positions relative to their new parent
        sprite1.localPosition = Vector3.zero;
        sprite2.localPosition = Vector3.zero;

        // Delay for neighbor readjustment
        t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        IsSwapping = false;
        tile1.TurnOnPhysics();
        tile2.TurnOnPhysics();
        QueuedTiles.Clear();

        // Check matches only once the swap is finalized
        tile1.ProcessMatch();
        tile2.ProcessMatch();
    }

    public void PickTile(Tile tile)
    {
        if (IsSwapping) return;

        switch (QueuedTiles.Count)
        {
            case 0:
                QueuedTiles.Add(tile);
                tile.ToggleOnPulsing();
                break;
            case 1:
                if (!QueuedTiles.Contains(tile))
                {
                    if (AreNeighbors(QueuedTiles[0], tile))
                    {
                        QueuedTiles.Add(tile);
                        foreach (var swappedTiles in QueuedTiles)
                        {
                            swappedTiles.TurnOffPhysics();
                            swappedTiles.TurnOffPulsing();
                        }
                        SwapTiles();
                    }
                    else tile.TurnOffPulsing();
                }
                else
                {
                    tile.TurnOffPulsing();
                    QueuedTiles.Remove(tile);
                }
                break;
            default:
                QueuedTiles.Clear();
                break;
        }
    }

    public void RemoveTile(Tile tile)
    {
        QueuedTiles.Remove(tile);
    }

    void SwapTiles()
    {
        Tile tile1 = QueuedTiles[0];
        Tile tile2 = QueuedTiles[1];

        if (tile1 == null || tile2 == null)
        {
            if (tile1 != null) tile1.TurnOffPulsing();
            if (tile2 != null) tile2.TurnOffPulsing();
            QueuedTiles.Clear();
            return;
        }

        GameManager.Instance.DecrementMoves();
        StartCoroutine(SwapRoutine(QueuedTiles[0], QueuedTiles[1], duration));
    }

    bool AreNeighbors(Tile tile1, Tile tile2)
    {
        return tile1.Neighbors.Contains(tile2) && tile2.Neighbors.Contains(tile1);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
