using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Plane: MonoBehaviour
{
	public int w, h;
	public float scale;

	private Mesh _mesh;
	private MeshFilter _meshFilter;
	private MeshCollider _meshCollider;

    private void Start()
	{
		_meshFilter = GetComponent<MeshFilter>();
		_meshCollider = GetComponent<MeshCollider>();
	}

	private void UpdateMesh()
	{
		_mesh = new Mesh();
		
		_mesh.Clear();
		_mesh.vertices = CalculateVertices();
		_mesh.triangles = CalculateTriangles();
		_mesh.RecalculateNormals();
		
		_meshFilter.mesh = _mesh;
		_meshCollider.sharedMesh = _mesh;
	}

	private int[] CalculateTriangles()
	{
		int[] triangles = new int[(w - 1) * (h - 1) * 6];
		for (int i = 0; i < w - 1; i++)
		{
			for (int j = 0; j < h - 1; j++)
			{
				int index = (i + j * (w - 1)) * 6;
				triangles[index] = i + j * w;
				triangles[index + 1] = i + (j + 1) * w;
				triangles[index + 2] = i + j * w + 1;
				triangles[index + 3] = i + j * w + 1;
				triangles[index + 4] = i + (j + 1) * w;
				triangles[index + 5] = i + (j + 1) * w + 1;
			}
		}
		return triangles;
	}

	private Vector3[] CalculateVertices()
	{
		Vector3[] vertices = new Vector3[w * h];
		float t = Time.time;
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				vertices[i + j * w] = new Vector3(i * scale, CalculateY(t, i, j), j * scale);
			}
		}
		return vertices;
	}

	private float CalculateY(float t, float x, float z)
	{
		float tnow = 2*Mathf.PI*t/10;
		float dist = 3;
		float dist1 = 0;
		float argx = dist*Mathf.Sin (tnow) + (x - w / 2) * scale;
		float argz = dist*Mathf.Cos (tnow) + (z - h / 2) * scale;
		float argx1 = dist1*Mathf.Sin (tnow+Mathf.PI) + (x - w / 2) * scale;
		float argz1 = dist1*Mathf.Cos (tnow+Mathf.PI) + (z - h / 2) * scale;
		//return -3*Mathf.Exp (-Mathf.Sin (argx)*Mathf.Sin (argx) - Mathf.Cos (argz)*Mathf.Cos (argz));
		return -3*Mathf.Exp (-argx*argx -argz*argz)-6*Mathf.Exp (-argx1*argx1 -argz1*argz1);
	}

	//No need to calculate these ourselves unless we want abnormal normals, this can probably be deleted
	private Vector3[] CalculateNormals()
	{
		Vector3[] normals = new Vector3[w * h];
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				normals[i + j * w] = Vector3.up;
			}
		}
		return normals;
	}

	private void Update()
	{
		UpdateMesh();
	}
}
