using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHut : MonoBehaviour
{
    public System.Random randomSeed;

    [Header("Settings")]
    [SerializeField] private float storyHeight = 30.0f;
    
    [Header("References")]
    [SerializeField] private Transform baseTransform;
    [SerializeField] private GameObject fencesPrefab, fourWallsPrefab, threeWallsPrefab, roofPrefab;
    
    private float width;
    private float depth;
    
    private Color woodColor, wallColor, roofColor;

    private enum StoryType { Top, Center, Bottom };
    
    private int numStories;

    private void Start()
    {
        //generate random seed and material colors
        woodColor = GenerateRandomColor(randomSeed, 0.26f, 0.26f, 0.196f, 0.196f, 0f, 0.15f);
        wallColor = GenerateRandomColor(randomSeed, 0.3f, 1f, 0.3f, 1f, 0.3f, 1f);
        roofColor = GenerateRandomColor(randomSeed, 0.3f, 1f, 0.3f, 1f, 0.3f, 1f);

        //35% chance to spawn fences (based on random seed)
        if((float)randomSeed.NextDouble() <= 0.35f) SpawnFences();

        //50% chance to spawn building
        if((float)randomSeed.NextDouble() <= 0.5f) SpawnBuilding();
    }

    //generates a pseudo-random color based on a given random seed
    private Color GenerateRandomColor(System.Random randomSeed, float minR, float maxR, float minG, float maxG, float minB, float maxB)
    {
        float r = (float)randomSeed.NextDouble() * (maxR - minR) + minR;
        float g = (float)randomSeed.NextDouble() * (maxG - minG) + minG;
        float b = (float)randomSeed.NextDouble() * (maxB - minB) + minB;
        return new Color(r, g, b);
    }

    private void SpawnFences()
    {
        //spawn fences prefab and set wood color
        GameObject fencesObj = Instantiate(fencesPrefab, baseTransform);
        MeshRenderer[] fencesMR = fencesObj.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer fence in fencesMR) fence.material.color = woodColor;
    }

    //spawn building with random amount of stories (between 2 and 5)
    private void SpawnBuilding()
    {
        numStories = Mathf.RoundToInt((float)randomSeed.NextDouble() * 4 + 2);
        for(int i = 0; i < numStories; i++)
        {
            //calculate center pos of current story
            Vector3 startPos = Vector3.up * (storyHeight / 2);
            Vector3 pos = (i * Vector3.up * storyHeight) + startPos + baseTransform.position;

            //determine what type of story this is
            StoryType type = StoryType.Bottom;
            if(i > 0) type = StoryType.Center;
            if(i == numStories - 1) type = StoryType.Top;

            //spawn walls and possibly spawn roof
            SpawnWalls(pos, type);
            bool spawnRoof = (i == numStories - 1); //spawn a roof if this is the top floor
            if((float)randomSeed.NextDouble() <= 0.35f) spawnRoof = true; //35% chance to spawn a roof without being top floor
            if(spawnRoof)
            {
                Vector3 roofPos = pos + startPos;
                SpawnRoof(roofPos);
            }
        }
    }

    private void SpawnWalls(Vector3 pos, StoryType storyType)
    {
        //spawn walls at pos with proper material colors 
        GameObject wallsPrefab = fourWallsPrefab;
        if(storyType == StoryType.Bottom) wallsPrefab = threeWallsPrefab;
        GameObject walls = Instantiate(wallsPrefab, baseTransform);
        walls.GetComponent<Transform>().position = pos; //set wall position
        walls.GetComponent<ProceduralWalls>().SetColors(woodColor, wallColor); //set material colors
    }

    private void SpawnRoof(Vector3 pos)
    {
        //spawn roof at pos with proper material colors
        GameObject roof = Instantiate(roofPrefab, baseTransform);
        roof.GetComponent<Transform>().position = pos; //set roof position
        roof.GetComponent<ProceduralRoof>().SetColors(woodColor, roofColor); //set material colors
    }
}
