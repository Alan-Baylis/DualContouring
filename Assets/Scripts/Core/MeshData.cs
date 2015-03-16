using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData {

	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
	public List<Vector2> uv = new List<Vector2>();

	public List<Vector3> colVertices = new List<Vector3>();
	public List<int> colTriangles = new List<int>();

	private bool collisions = true;

	public void AddTriangles(){
		//vextices 1, 2, 3
		this.triangles.Add (vertices.Count - 4);
		this.triangles.Add (vertices.Count - 3);
		this.triangles.Add (vertices.Count - 2);
		
		//vertices 1, 3, 4
		this.triangles.Add (vertices.Count - 4);
		this.triangles.Add (vertices.Count - 2);
		this.triangles.Add (vertices.Count - 1);
	}

	public void AddVertex(Vector3 vertex){
		vertices.Add (vertex);
		if (collisions)
			colVertices.Add (vertex);
	}

	public void AddTriangle(int triangle){
		this.triangles.Add (triangle);
		if (collisions)
			colTriangles.Add (triangle);
	}

    internal void AddNormal(Vector3 normal){
        this.normals.Add(normal);
    }
}
