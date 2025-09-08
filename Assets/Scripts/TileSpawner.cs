using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [field: Header("Refs")]
    [field: SerializeField] GameObject yellowPrefab;
    [field: SerializeField] GameObject purplePrefab;
    [field: SerializeField] GameObject bluePrefab;
    [field: SerializeField] GameObject redPrefab;
    [field: SerializeField] GameObject greenPrefab;
    [field: SerializeField] public TileSpawner LeftNeighbor { get; private set; }
    [field: SerializeField] public TileSpawner RightNeighbor { get; private set; }

    [field: Header("State")]
    [field: SerializeField] public bool CanSpawn { get; private set; } = true;


    [field: Header("Properties")]
    [field: SerializeField] public TileType SpawnedType { get; private set; }

    public void SpawnRandomTile()
    {
        GameObject prefab = GetPrefabForTileType(SpawnedType);
        if (prefab == null) return;
        Tile tile = Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<Tile>();
        GameManager.Instance.Tiles.Add(tile);
        CanSpawn = false;
    }

    public void SetTileType(TileType tileType)
    {
        SpawnedType = tileType;
    }

    public void ResetSpawns()
    {
        CanSpawn = true;
    }

    private GameObject GetPrefabForTileType(TileType type)
    {
        switch (type)
        {
            case TileType.Yellow: return yellowPrefab;
            case TileType.Purple: return purplePrefab;
            case TileType.Blue: return bluePrefab;
            case TileType.Red: return redPrefab;
            case TileType.Green: return greenPrefab;
            default: return null;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Tile")) CanSpawn = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tile")) CanSpawn = true;
    }

    void Awake() {
        GameManager.OnGameStart += ResetSpawns;
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= ResetSpawns;
    }
}
