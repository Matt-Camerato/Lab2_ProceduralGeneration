using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance; //singleton

    [Header("Level Generation Settings")]
    [SerializeField] private int levelWidth = 5;
    [SerializeField] private int levelDepth = 5;
    //NOTE: 5x5 tiles generate around player (render distance)

    //noise settings shouldn't change once level starts
    [Header("Noise Settings")]
    public float noiseScale = 1f;
    public int octaves = 0;
    [Range(0, 1)] public float persistance;
    public float lacunarity = 1f;
    public int seed; //seed of level

    [Header("Terrain Settings")]
    public float heightScale = 1f;
    public AnimationCurve heightCurve;
    public TerrainType[] terrainTypes;

    [Header("References")]
    [SerializeField] private GameObject tilePrefab;

    private GameObject[,] tiles; //2D array of spawned tiles

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateLevel()
    {
        //reset tiles array if not empty
        if(tiles != null)
        {
            foreach(GameObject tile in tiles) Destroy(tile);
        }

        //initialize 2D array of tiles
        tiles = new GameObject[levelWidth, levelDepth];

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
