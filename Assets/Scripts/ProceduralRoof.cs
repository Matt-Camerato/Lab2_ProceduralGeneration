using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoof : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> beams;
    [SerializeField] private MeshRenderer roofPlatform;
    
    public void SetColors(Color woodColor, Color roofColor)
    {
        //set wood color
        foreach(MeshRenderer beam in beams) beam.material.color = woodColor;
        
        //set roof color
        roofPlatform.material.color = roofColor; 
    }
}
