using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        Texture2D texture = TextureGenerator.TextureFromHeightMap(heightMap);
        meshRenderer.material.mainTexture = texture;

        //update vertices with height map
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
                height = LevelGenerator.Instance.heightCurve.Evaluate(height);
                height *= LevelGenerator.Instance.heightScale;
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
}
