using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapDepth, Vector2 tileOffset)
    {
        //initialize float array for noise map
        float[,] noiseMap = new float[mapDepth, mapWidth];

        //minimum noise scale is 0.001f
        float scale = Mathf.Max(LevelGenerator.Instance.noiseScale, 0.001f);

        //use level seed to calculate octave offsets for noise map generation
        System.Random randomSeed = new System.Random(LevelGenerator.Instance.seed);
        Vector2[] octaveOffsets = new Vector2[LevelGenerator.Instance.octaves];
        for(int i = 0; i < LevelGenerator.Instance.octaves; i++)
        {
            float offsetX = randomSeed.Next(-100000, 100000) + tileOffset.x;
            float offsetZ = randomSeed.Next(-100000, 100000) + tileOffset.y;
        }

        //fields used to find min and max values of noise map
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //generate float values for noise map
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < LevelGenerator.Instance.octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleZ = z / scale * frequency;

                    //generate noise height using perlin noise for given wave settings
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    
                    amplitude *= LevelGenerator.Instance.persistance;
                    frequency *= LevelGenerator.Instance.lacunarity;
                }
                //update min and max values of noise map
                if(noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if(noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[z, x] = noiseHeight;
            }
        }

        //normalize noise map with min and max values
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[z, x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[z, x]);
            }
        }

        //return noise map
        return noiseMap;
    }
}
