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
    [SerializeField] private float noiseScale;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public int seed; //seed of level

    [Header("References")]
    [SerializeField] private GameObject tilePrefab;

    private GameObject[,] tiles; //2D array of spawned tiles

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateLevel()
    {
        //generate tiles based on level width and depth
        for(int x = 0; x < levelWidth; x++)
        {
            for(int z = 0; z < levelDepth; z++)
            {
                Vector3 tilePos = new Vector3(x, 0, z);
                GameObject newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                tiles[x, z] = newTile; //save reference to spawned tile
            }
        }
    }
}
