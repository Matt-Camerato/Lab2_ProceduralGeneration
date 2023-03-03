using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Terrain : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    //get reference to mesh renderer
    private void Start() => meshRenderer = GetComponent<MeshRenderer>();

    public void ShowNoiseMap(float[,] noiseMap)
    {
        //get width and height of noise map
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        //create texture that will be applied to the mesh renderer
        Texture2D texture = new Texture2D(width, height);
        
        //color the texture based on noise values
        Color[] colorMap = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colorMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();
        
        //apply texture to mesh renderer and set its scale based on map dimensions
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
