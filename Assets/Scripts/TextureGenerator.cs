using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(int width, int depth, Color[] colorMap)
    {
        //create a new texture and set its pixel colors using color map
        Texture2D texture = new Texture2D(width, depth);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int z = 0; z < tileDepth; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                float height = heightMap[z, x]; //height value between 0 and 1
                foreach(LevelGenerator.TerrainType terrainType in LevelGenerator.Instance.terrainTypes)
                {
                    if(height > terrainType.height) continue; //loop until terrain type is found
                    colorMap[z * tileDepth + x] = terrainType.color; //set appropriate color based on terrain type
                    break;
                }
            }
        }
        return TextureFromColorMap(tileWidth, tileDepth, colorMap);
    }
}
