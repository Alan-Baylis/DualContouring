using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeamChunk : MonoBehaviour{

	public Vector3 pos;
	public OctreeNode root = new OctreeNode();
	public Planet planet;
	public int leafsize;

	public Chunk parent;
	private MeshFilter filter;
	// Use this for initialization
	void Start () {
		filter = gameObject.GetComponent<MeshFilter> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			BuildSeam();
			RenderSeam();
		}
	}

	public void BuildSeam(){
		Dictionary<Vector3, OctreeNode> seamNodes = new Dictionary<Vector3, OctreeNode> ();
		Vector3 maxPos = new Vector3 (this.pos.x + Chunk.size-leafsize, this.pos.y + Chunk.size-leafsize, this.pos.z + Chunk.size-leafsize);

		parent.root.GetInternalNodes (seamNodes, maxPos);
		//Debug.Log ("Totale interni: "+seamNodes.Count);
		
		for (int i=1; i<8; i++) {
			Chunk c = planet.GetChunk(maxPos+(OctreeNode.CHILD_MIN_OFFSETS[i]*leafsize));
			
			if (c!=null && !c.root.IsLeaf()){
				c.root.GetSeamNodes(seamNodes, c.root.min, i);
			}
		}
		/*Debug.Log ("Totale: "+seamNodes.Count);
		foreach (KeyValuePair<Vector3, OctreeNode> n in seamNodes) {
			n.Value.DrawOctree(Color.green);
		}*/
		BuildSeamChunk (seamNodes);
	}
	
	public void BuildSeamChunk(Dictionary<Vector3, OctreeNode> seamNodes){
		if (seamNodes.Count == 0)
			return;
		root = root.BuildOctreeUpwards (seamNodes, pos);
	}

	public void RenderSeam(){
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
}
