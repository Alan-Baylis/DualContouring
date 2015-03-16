using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Chunk : MonoBehaviour {

	public OctreeNode root = new OctreeNode ();
    public static int size = 64;
	public int leafsize; 
	public Vector3 pos;
	public GameObject chunkPrefab;
	public Planet planet;
	public ThreadManager manager;

	private SeamChunk seamChunk;
	private ChunkThread t;
    
	private MeshFilter filter;

    void Start () {
		leafsize = size / 8;

        filter = gameObject.GetComponent<MeshFilter>();

		GameObject newChunkObject = Instantiate(chunkPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
		newChunkObject.transform.parent = this.transform;
		seamChunk = newChunkObject.GetComponent<SeamChunk> ();

		seamChunk.pos = this.pos;
		seamChunk.planet = this.planet;
		seamChunk.leafsize = this.leafsize;
		seamChunk.parent = this;

		t = new ChunkThread ();
		t.chunk = this;
	}
	
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			manager.AddThread(t);
		}

		if (t != null){
			if (t.Update()){
				t = null;
			}
		}
	}

	public void RenderChunk(){
		//root.DrawOctree (Color.magenta, true);
		if (root.IsLeaf ())
			return;

		MeshData data = new MeshData ();
		root.GenerateMeshFromOctree (root, data);

		filter.mesh.Clear ();
		filter.mesh.vertices = data.vertices.ToArray ();
		filter.mesh.triangles = data.triangles.ToArray ();
		//filter.mesh.RecalculateNormals ();
		filter.mesh.normals = data.normals.ToArray ();
	}


	/*public IEnumerator BuildChunk(){
		root = root.BuildOctree(pos,size,-1);
		yield return null;
	}*/
}
