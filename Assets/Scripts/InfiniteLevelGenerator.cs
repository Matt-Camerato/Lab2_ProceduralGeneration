using UnityEngine;

public class InfiniteLevelGenerator : MonoBehaviour
{
    public static InfiniteLevelGenerator Instance; //singleton

    [Header("Level Generation Settings")]
    [SerializeField] private int renderDistance = 5;

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
    [SerializeField] private Transform playerTransform;

    private GameObject[,] tiles; //2D array of spawned tiles
    private Vector3 currentTilePos;
    private Vector3 previousTilePos;

    private void Awake() => Instance = this;

    private void Start()
    {
        currentTilePos = Vector3.zero;
        previousTilePos = Vector3.zero;

        //generate first set of tiles around player
        GenerateTiles(currentTilePos);
    }

    private void Update()
    {
        //check if the player has reached the edge of the current tile
        if (Mathf.Abs(playerTransform.position.x - currentTilePos.x) >= tilePrefab.transform.localScale.x
            || Mathf.Abs(playerTransform.position.z - currentTilePos.z) >= tilePrefab.transform.localScale.z)
        {
            //update the current and previous tile positions
            previousTilePos = currentTilePos;
            currentTilePos = new Vector3Int(
                Mathf.FloorToInt(playerTransform.position.x / tilePrefab.transform.localScale.x) * Mathf.FloorToInt(tilePrefab.transform.localScale.x),
                0,
                Mathf.FloorToInt(playerTransform.position.z / tilePrefab.transform.localScale.z) * Mathf.FloorToInt(tilePrefab.transform.localScale.z));

            //generate new tiles in the direction the player is moving
            GenerateTiles(currentTilePos);
        }
    }

    private void GenerateTiles(Vector3 pos)
    {
        //delete any previous tiles
        if (tiles != null)
        {
            foreach (GameObject tile in tiles) Destroy(tile);
        }

        //get tile dimensions from prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        //get player position
        Vector3 playerPos = playerTransform.position;

         //calculate the range of tiles to spawn around the player
        int startTileX = Mathf.FloorToInt((playerPos.x - renderDistance * tileWidth) / tileWidth);
        int endTileX = Mathf.FloorToInt((playerPos.x + renderDistance * tileWidth) / tileWidth);
        int startTileZ = Mathf.FloorToInt((playerPos.z - renderDistance * tileDepth) / tileDepth);
        int endTileZ = Mathf.FloorToInt((playerPos.z + renderDistance * tileDepth) / tileDepth);

        //generate tiles within the render distance around the player
        tiles = new GameObject[endTileX - startTileX + 1, endTileZ - startTileZ + 1];
        for (int x = startTileX; x <= endTileX; x++)
        {
            for (int z = startTileZ; z <= endTileZ; z++)
            {
                Vector3 tilePos = new Vector3(x * tileWidth, 0, z * tileDepth);
                GameObject newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                tiles[x - startTileX, z - startTileZ] = newTile; //save reference to spawned tile
            }
        }
    }

    private void OnValidate()
    {
        //clamps for inspector values
        if(octaves < 0) octaves = 0;
        if(lacunarity < 1f) lacunarity = 1f;
        if(heightScale < 1f) heightScale = 1f;
        if(noiseScale < 0.001f) noiseScale = 0.001f;
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
