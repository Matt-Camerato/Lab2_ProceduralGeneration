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
            float offsetX = randomSeed.Next(-100000, 100000);
            float offsetZ = randomSeed.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        //generate float values for noise map
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x + tileOffset.x) / scale;
                float sampleZ = (z + tileOffset.y) / scale;

                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;
                float normalization = 0f;
                for(int i = 0; i < LevelGenerator.Instance.octaves; i++)
                {
                    //generate height using perlin noise for current octave
                    float perlinValue = Mathf.PerlinNoise(
                        sampleX * frequency + octaveOffsets[i].x,
                        sampleZ * frequency + octaveOffsets[i].y);
                    noiseHeight += amplitude * perlinValue;
                    normalization += amplitude;
                    
                    //change amplitude and frequency of next wave using persistance and lacunarity values
                    amplitude *= LevelGenerator.Instance.persistance;
                    frequency *= LevelGenerator.Instance.lacunarity;
                }
                //normalize noise value between 0 and 1
                noiseHeight /= normalization;
                noiseMap[z, x] = noiseHeight;
            }
        }
        //return noise map
        return noiseMap;
    }
}
