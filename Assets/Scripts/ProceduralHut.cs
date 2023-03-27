using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHut : MonoBehaviour
{
    
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
        //setup base platform
        //width = Random.Range(4.5f, 7.5f);
        //depth = Random.Range(4.5f, 7.5f);
        //baseTransform.localScale = new Vector3(width, 0.1f, depth);

        //35% chance to spawn fences
        bool spawnFences = (Random.value <= 0.35);
        if(spawnFences)
        {
            //generate wood material color and spawn fences
            woodColor = Color.HSVToRGB(45, 100, Random.Range(10, 40));
            SpawnFences();
        }

        //50% chance to spawn building
        bool spawnBuilding = (Random.value <= 0.5);
        if(spawnBuilding)
        {
            //generate wood, wall, and roof material colors and spawn building
            woodColor = new Color(0.26f, 0.196f, Random.Range(0f, 0.15f));
            wallColor = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
            roofColor = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
            SpawnBuilding();
        }
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
        numStories = Random.Range(2, 6);
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
            if(Random.value <= 0.35f) spawnRoof = true; //35% chance to spawn a roof without being top floor
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
