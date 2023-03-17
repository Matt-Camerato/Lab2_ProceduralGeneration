using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHut : MonoBehaviour
{
    [SerializeField] private Transform baseTransform;
    
    private float width;
    private float depth;

    private float height;

    private void Start()
    {
        //setup base platform
        width = Random.Range(4.5f, 7.5f);
        depth = Random.Range(4.5f, 7.5f);
        baseTransform.localScale = new Vector3(width, 0.1f, depth);
    }
}
