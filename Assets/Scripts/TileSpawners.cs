using UnityEngine;
using System.Collections.Generic;

public class TileSpawners : MonoBehaviour
{
    public static TileSpawners Instance { get; private set; }


    [Header("Refs")]
    [SerializeField] List<TileSpawner> tileSpawners = new List<TileSpawner>();

    [Header("Properties")]
    [SerializeField] bool isUniqueSpawns = true;
    [SerializeField] float uniqueSpawnDuration = 6f;
    [SerializeField] float spawnDuration = 0.2f;

    float uniqueTimer = 0f;
    float spawnTimer = 0f;

    public void SetUniqueSpawnType(TileSpawner spawner)
    {
        List<TileType> validTypes = new List<TileType>(TileTypeUtil.Types);
        if (isUniqueSpawns) validTypes.Remove(spawner.SpawnedType);
        if (isUniqueSpawns && spawner.LeftNeighbor != null) validTypes.Remove(spawner.LeftNeighbor.SpawnedType);
        if (isUniqueSpawns && spawner.RightNeighbor != null) validTypes.Remove(spawner.RightNeighbor.SpawnedType);
        TileType spawnedType = validTypes[Random.Range(0, validTypes.Count)];
        spawner.SetTileType(spawnedType);
    }

    public void SetUniqueSpawns()
    {
        foreach (var spawner in tileSpawners)
        {
            if (!GameManager.Instance.StartingScreen && spawner.CanSpawn) SetUniqueSpawnType(spawner);
        }
    }

    public void SpawnAll()
    {
        foreach (var spawner in tileSpawners)
        {
        if (!GameManager.Instance.StartingScreen && spawner.CanSpawn) spawner.SpawnRandomTile();
        }
    }

    void ProcessUniqueSpawnTimer()
    {
        if (!isUniqueSpawns) return;
        if (uniqueTimer >= uniqueSpawnDuration)
        {
            uniqueTimer = 0f;
            isUniqueSpawns = false;
        }
        uniqueTimer += Time.fixedDeltaTime;
    }

    void ProcessSpawns()
    {
        if (spawnTimer >= spawnDuration)
        {
            spawnTimer = 0f;
            SetUniqueSpawns();
            SpawnAll();
        }
        spawnTimer += Time.fixedDeltaTime;
    }

    void ResetUniqueSpawns() {
        isUniqueSpawns = true;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameManager.OnGameStart += ResetUniqueSpawns;
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= ResetUniqueSpawns;
    }

    void FixedUpdate()
    {
        ProcessUniqueSpawnTimer();
        ProcessSpawns();
    }
}
