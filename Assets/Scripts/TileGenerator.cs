using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TileGenerator : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        //calculate offsets based on tile position
        float offsetX = -transform.position.x;
        float offsetZ = -transform.position.z;

        //generate height map using noise generator and tile offsets
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(
            tileDepth, tileWidth, new Vector2(offsetX, offsetZ));

        //generate vertices using height map and update mesh filter and collider
        Vector3[] vertices = GenerateVertices(heightMap);
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();


        //generate texture and apply it using height map

    }

    //generate texture based on given heightmap
    private Texture2D GenerateTexture(float[,] heightMap)
    {

    }

    private Vector3[] GenerateVertices(float[,] heightMap)
    {

    }


}
