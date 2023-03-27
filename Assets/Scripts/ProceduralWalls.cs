using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalls : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> posts;
    [SerializeField] private List<MeshRenderer> walls;
    
    public void SetColors(Color woodColor, Color wallColor)
    {
        //set wood color
        foreach(MeshRenderer post in posts) post.material.color = woodColor;
        
        //set wall color
        foreach(MeshRenderer wall in walls) wall.material.color = wallColor;
    }
}
