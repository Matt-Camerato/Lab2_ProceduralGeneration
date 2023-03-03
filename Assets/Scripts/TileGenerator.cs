using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TileGenerator : MonoBehaviour
{
    public float offsetX, offsetZ;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

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
        Texture2D texture = GenerateTexture(heightMap);
        meshRenderer.material.mainTexture = texture;

        UpdateVertices(heightMap);
    }

    //generate mesh vertices with given heightmap
    private void UpdateVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] vertices = meshFilter.mesh.vertices;

        //iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int z = 0; z < tileDepth; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                float height = heightMap[z, x];
                Vector3 vertex = vertices[vertexIndex];
                //height = AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(height);
                vertices[vertexIndex] = new Vector3(vertex.x, height, vertex.z);
                vertexIndex++;
            }
        }
        //update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    //generate texture with given heightmap
    private Texture2D GenerateTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int z = 0; z < tileDepth; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = z * tileDepth + x;
                float height = heightMap[z, x];
                //assign shade of grey proportional to the height value
                colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
                
                //TerrainType terrainType = ChooseTerrainType(height);
                //colorMap[colorIndex] = terrainType.color;
            }
        }

        // create a new texture and set its pixel colors
        Texture2D texture = new Texture2D(tileWidth, tileDepth);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
}
