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

    IEnumerator SwapRoutine(Tile tile1, Tile tile2, float duration = 1f)
    {
        Vector3 start1 = tile1.Rb.position;
        Vector3 start2 = tile2.Rb.position;

        float t = 0f;

        IsSwapping = true;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / duration;

            Vector3 pos1 = Vector3.Lerp(start1, start2, t);
            Vector3 pos2 = Vector3.Lerp(start2, start1, t);

            tile1.Rb.MovePosition(pos1);
            tile2.Rb.MovePosition(pos2);

            yield return new WaitForFixedUpdate();
        }
        IsSwapping = false;
        SwapNeighbors(tile1, tile2);
    }

    public void PickTile(Tile tile)
    {
        if (IsSwapping) return;

        switch (QueuedTiles.Count)
        {
            case 0:
                QueuedTiles.Add(tile);
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
                            swappedTiles.TogglePulsing();
                        }
                        SwapTiles();
                        QueuedTiles.Clear();
                    }
                    else tile.TogglePulsing();
                }
                else QueuedTiles.Remove(tile);
                break;
            default:
                QueuedTiles.Clear();
                break;
        }
    }

    void SwapTiles()
    {
        StartCoroutine(SwapRoutine(QueuedTiles[0], QueuedTiles[1], duration));
    }

    void SwapNeighbors(Tile tile1, Tile tile2)
    {
        List<Tile> temp = tile1.Neighbors;
        tile1.SetNeighbors(tile2.Neighbors);
        tile2.SetNeighbors(temp);
        // tile1.RemoveNeighbor(tile1);
        // tile2.RemoveNeighbor(tile2);
        foreach (Tile neighbor in tile1.Neighbors) neighbor.RemoveNeighbor(tile2);
        foreach (Tile neighbor in tile2.Neighbors) neighbor.RemoveNeighbor(tile1);
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
