using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance; //singleton

    [Header("Level Generation Settings")]
    [SerializeField] private int levelWidth = 5;
    [SerializeField] private int levelDepth = 5;

    //noise settings shouldn't change once level starts
    [Header("Noise Settings")]
    public float noiseScale = 1f;
    public int octaves = 0;
    [Range(0, 1)] public float persistance;
    public float lacunarity = 1f;
    public int seed = 0; //seed of level

    [Header("Terrain Settings")]
    public float heightScale = 1f;
    public AnimationCurve heightCurve;
    public TerrainType[] terrainTypes;

    [Header("References")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject safeZonePrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private TMP_InputField seedIF;

    private GameObject[,] tiles; //2D array of spawned tiles
    private GameObject goal; //goal cube

    private bool hasPath = false;

    private void Awake() => Instance = this;

    private void Start()
    {
        //check if this level has a path in it
        if(GetComponent<PathGenerator>() != null) hasPath = true;
    }

    public void GenerateLevel()
    {
        //delete any previous tiles
        if(tiles != null) foreach(GameObject tile in tiles) Destroy(tile);
        tiles = new GameObject[levelWidth, levelDepth]; //initialize new 2D array of tiles

        //if path objects exist, delete them
        if(hasPath && GetComponent<PathGenerator>().pathObjs != null)
        {
            List<GameObject> pathObjs = GetComponent<PathGenerator>().pathObjs;
            foreach(GameObject path in pathObjs) Destroy(path);
        }

        //get noise seed from input field if possible
        if(!string.IsNullOrEmpty(seedIF.text)) seed = int.Parse(seedIF.text);

        //get tile dimensions from prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        //generate tiles based on level width and depth
        for(int x = 0; x < levelWidth; x++)
        {
            for(int z = 0; z < levelDepth; z++)
            {
                Vector3 tilePos = new Vector3(x * tileWidth, 0, z * tileDepth);
                GameObject newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                tiles[x, z] = newTile; //save reference to spawned tile
            }
        }

        //place the goal on the last tile generated, opposite where the player starts
        Vector3 goalPos = new Vector3((levelWidth - 1) * tileWidth, 5, (levelDepth - 1) * tileDepth);
        SpawnGoal(goalPos);
    }

    public void SpawnGoal(Vector3 pos)
    {
        //delete any pre-existing goal
        if(goal != null) Destroy(goal);

        //spawn new goal cube
        goal = Instantiate(goalPrefab, pos, Quaternion.identity);
    }

    public void GeneratePath()
    {
        //get tile dimensions from prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        //generate path
        GetComponent<PathGenerator>().GeneratePath(levelWidth, levelDepth, tileWidth, tileDepth);
    }

    public void GenerateSafeZones()
    {
        foreach(GameObject tile in tiles)
        {
            //loop through tiles, only selecting ones that are relatively flat
            TileGenerator tg = tile.GetComponent<TileGenerator>();
            if(tg.AvgHeight < 0.5 || tg.AvgHeight > 0.55) continue;
            if(tg.AvgHeightScaled < 2f || tg.AvgHeightScaled > 2.2f) continue;

            //after checks, only 30% chance a safe zone will spawn
            if(Random.Range(0f, 1f) > 0.6f) continue;

            //spawn save zone
            tg.SpawnSafeZone();
        }
    }

    private void OnValidate()
    {
        //clamps for inspector values
        if(levelWidth < 1) levelWidth = 1;
        if(levelDepth < 1) levelDepth = 1;
        if(octaves < 0) octaves = 0;
        if(lacunarity < 1f) lacunarity = 1f;
        if(heightScale < 1f) heightScale = 1f;
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
