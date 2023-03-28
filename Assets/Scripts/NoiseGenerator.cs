using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapDepth, Vector2 tileOffset)
    {
        //initialize float array for noise map
        float[,] noiseMap = new float[mapDepth, mapWidth];

        float scale;
        System.Random randomSeed;
        Vector2[] octaveOffsets;

        //get noise scale and use level seed to calculate octave offsets for noise map generation
        if(LevelGenerator.Instance != null) //if first four levels with regular level generator
        {
            scale = LevelGenerator.Instance.noiseScale;
            randomSeed = new System.Random(LevelGenerator.Instance.seed);
            octaveOffsets = new Vector2[LevelGenerator.Instance.octaves];
            for(int i = 0; i < LevelGenerator.Instance.octaves; i++)
            {
                float offsetX = randomSeed.Next(-100000, 100000);
                float offsetZ = randomSeed.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetZ);
            }
        }
        else //else use level 5's infinite level generator
        {
            scale = InfiniteLevelGenerator.Instance.noiseScale;
            randomSeed = new System.Random(InfiniteLevelGenerator.Instance.seed);
            octaveOffsets = new Vector2[InfiniteLevelGenerator.Instance.octaves];
            for(int i = 0; i < InfiniteLevelGenerator.Instance.octaves; i++)
            {
                float offsetX = randomSeed.Next(-100000, 100000);
                float offsetZ = randomSeed.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetZ);
            }
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
                
                int numOctaves = (LevelGenerator.Instance != null)?
                    LevelGenerator.Instance.octaves : InfiniteLevelGenerator.Instance.octaves;
                for(int i = 0; i < numOctaves; i++)
                {
                    //generate height using perlin noise for current octave
                    float perlinValue = Mathf.PerlinNoise(
                        sampleX * frequency + octaveOffsets[i].x,
                        sampleZ * frequency + octaveOffsets[i].y);
                    noiseHeight += amplitude * perlinValue;
                    normalization += amplitude;
                    
                    //change amplitude and frequency of next wave using persistance and lacunarity values
                    amplitude *= (LevelGenerator.Instance != null)? LevelGenerator.Instance.persistance : InfiniteLevelGenerator.Instance.persistance;
                    frequency *= (LevelGenerator.Instance != null)? LevelGenerator.Instance.lacunarity : InfiniteLevelGenerator.Instance.lacunarity;
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
