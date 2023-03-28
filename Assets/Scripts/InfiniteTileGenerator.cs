using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class InfiniteTileGenerator : MonoBehaviour
{
    public float offsetX, offsetZ;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    
    [SerializeField] private GameObject safeZonePrefab;
    [SerializeField] private GameObject treePrefab;

    public float AvgHeight;
    public float AvgHeightScaled;
    private GameObject safeZone = null;
    private List<GameObject> trees = null;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        GenerateTile();
    }

    private void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        //calculate offsets based on tile position
        offsetX = -transform.position.x;
        offsetZ = -transform.position.z;

        //generate height map using noise generator and tile offsets
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(
            tileDepth, tileWidth, new Vector2(offsetX, offsetZ));

        //generate texture and apply it using height map
        Texture2D texture = TextureGenerator.TextureFromHeightMap(heightMap);
        meshRenderer.material.mainTexture = texture;

        //update vertices with height map
        UpdateVertices(heightMap);

        bool hasSafeZone = SpawnSafeZone();

        if(hasSafeZone) return;

        SpawnTrees(tileWidth, tileDepth);
    }

    //generate mesh vertices with given heightmap
    private void UpdateVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] vertices = meshFilter.mesh.vertices;

        //iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        float totalHeight = 0;
        float totalHeightScaled = 0;
        for (int z = 0; z < tileDepth; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                float height = heightMap[z, x];
                totalHeight += height;
                Vector3 vertex = vertices[vertexIndex];

                //get height curve and scale from correct level generator
                AnimationCurve heightCurve = (LevelGenerator.Instance != null)?
                    LevelGenerator.Instance.heightCurve : InfiniteLevelGenerator.Instance.heightCurve;
                float heightScale = (LevelGenerator.Instance != null)?
                    LevelGenerator.Instance.heightScale : InfiniteLevelGenerator.Instance.heightScale;

                //adjust height using height curve and scale
                height = heightCurve.Evaluate(height);
                height *= heightScale;
                totalHeightScaled += height;
                vertices[vertexIndex] = new Vector3(vertex.x, height, vertex.z);
                vertexIndex++;
            }
        }

        //calculate tile's average height (and average height scaled)
        AvgHeight = totalHeight / vertexIndex;
        AvgHeightScaled = totalHeightScaled / vertexIndex;

        //update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public bool SpawnSafeZone()
    {
        //spawn safe zones on flat grassy areas
        if(AvgHeight < 0.5 || AvgHeight > 0.55) return false;
        if(AvgHeightScaled < 2f || AvgHeightScaled > 2.65f) return false;

        //generate random seed
        int seed = InfiniteLevelGenerator.Instance.seed + Mathf.FloorToInt(offsetX) * Mathf.FloorToInt(offsetZ);
        System.Random randomSeed = new System.Random(seed);

        //50% chance to spawn on a possible tile
        if((float)randomSeed.NextDouble() > 0.5f) return false;

        Vector3 spawnPos = new Vector3(transform.position.x, AvgHeightScaled, transform.position.z);
        safeZone = Instantiate(safeZonePrefab, spawnPos, Quaternion.identity);
        safeZone.GetComponent<ProceduralHut>().randomSeed = randomSeed;
        return true;
    }

    private void SpawnTrees(float tileWidth, float tileDepth)
    {
        //spawn trees on grass tiles
        if(AvgHeight < 0.45 || AvgHeight > 0.55) return;
        if(AvgHeightScaled < 2f || AvgHeightScaled > 2.7f) return;

        //generate random seed
        int seed = InfiniteLevelGenerator.Instance.seed + Mathf.FloorToInt(offsetX) * Mathf.FloorToInt(offsetZ);
        System.Random randomSeed = new System.Random(seed);

        //generate pseudo-random # of trees, then place them in pseudo-random locations on tile
        trees = new List<GameObject>();
        int numTrees = randomSeed.Next(0, 3);
        for(int i = 0; i < numTrees; i++)
        {
            float xOffset = (float)randomSeed.NextDouble() * tileWidth - (tileWidth / 2);
            float zOffset = (float)randomSeed.NextDouble() * tileDepth - (tileDepth / 2);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 2f, zOffset);
            GameObject tree = Instantiate(treePrefab, spawnPos, Quaternion.identity);
            tree.transform.localScale = Vector3.one * ((float)randomSeed.NextDouble() * 0.3f + 0.7f);
            trees.Add(tree);
        }
    }

    private void OnDestroy()
    {
        if(safeZone != null) Destroy(safeZone);
        if(trees != null) foreach(GameObject tree in trees) Destroy(tree);
    }
}
