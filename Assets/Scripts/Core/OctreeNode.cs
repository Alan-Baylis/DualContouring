using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qef;

public enum NodeType {
	NONE,
	INTERNAL, 
	PSEUDO, 
	LEAF
};

public class OctreeNode {

	//-------Dati predefiniti per la computazione del Dual Contouring--------//
	public static int SOLID = 1;
	public static int AIR = 0;
	
	public static int MAX_CROSSINGS = 6;
	
	public static float QEF_ERROR = 1e-6f;
	public static int QEF_SWEEPS = 4;
	
	public static Vector3 [] CHILD_MIN_OFFSETS = {
		new Vector3( 0, 0, 0 ),
		new Vector3( 0, 0, 1 ),
		new Vector3( 0, 1, 0 ),
		new Vector3( 0, 1, 1 ),
		new Vector3( 1, 0, 0 ),
		new Vector3( 1, 0, 1 ),
		new Vector3( 1, 1, 0 ),
		new Vector3( 1, 1, 1 ),
	};
	
	public static int[][] edgevmap = new int[12][]{
		new int[2]{0,4},new int[2]{1,5},new int[2]{2,6},new int[2]{3,7}, // x-axis
		new int[2]{0,2},new int[2]{1,3},new int[2]{4,6},new int[2]{5,7}, // y-axis
		new int[2]{0,1},new int[2]{2,3},new int[2]{4,5},new int[2]{6,7} // z-axis
	};
	
	//----DATI DEL DUAL CONTOURING ORIGINALE----//
	public static int[] edgemask = { 5, 3, 6 } ;
	
	public static int[][] vertMap =
	{
		new int[3]{0,0,0},
		new int[3]{0,0,1},
		new int[3]{0,1,0},
		new int[3]{0,1,1},
		new int[3]{1,0,0},
		new int[3]{1,0,1},
		new int[3]{1,1,0},
		new int[3]{1,1,1}
	};
	
	public static int[][] faceMap = {
		new int[]{4, 8, 5, 9}, 
		new int[]{6, 10, 7, 11},
		new int[]{0, 8, 1, 10},
		new int[]{2, 9, 3, 11},
		new int[]{0, 4, 2, 6},
		new int[]{1, 5, 3, 7}
	} ;
	
	public static int[][] cellProcFaceMask = {
		new int[]{0,4,0},
		new int[]{1,5,0},
		new int[]{2,6,0},
		new int[]{3,7,0},
		new int[]{0,2,1},
		new int[]{4,6,1},
		new int[]{1,3,1},
		new int[]{5,7,1},
		new int[]{0,1,2},
		new int[]{2,3,2},
		new int[]{4,5,2},
		new int[]{6,7,2}
	} ;
	public static int[][] cellProcEdgeMask = {
		new int[]{0,1,2,3,0},
		new int[]{4,5,6,7,0},
		new int[]{0,4,1,5,1},
		new int[]{2,6,3,7,1},
		new int[]{0,2,4,6,2},
		new int[]{1,3,5,7,2}
	} ;
	public static int[][][] faceProcFaceMask = {
		new int[][]{
			new int[]{4,0,0},
			new int[]{5,1,0},
			new int[]{6,2,0},
			new int[]{7,3,0}
		},
		new int[][]{
			new int[]{2,0,1},
			new int[]{6,4,1},
			new int[]{3,1,1},
			new int[]{7,5,1}
		},
		new int[][]{
			new int[]{1,0,2},
			new int[]{3,2,2},
			new int[]{5,4,2},
			new int[]{7,6,2}
		}
	} ;
	public static int [][][] faceProcEdgeMask = {
		new int[][]{
			new int[]{1,4,0,5,1,1},
			new int[]{1,6,2,7,3,1},
			new int[]{0,4,6,0,2,2},
			new int[]{0,5,7,1,3,2}
		},
		new int[][]{
			new int[]{0,2,3,0,1,0},
			new int[]{0,6,7,4,5,0},
			new int[]{1,2,0,6,4,2},
			new int[]{1,3,1,7,5,2}
		},
		new int[][]{
			new int[]{1,1,0,3,2,0},
			new int[]{1,5,4,7,6,0},
			new int[]{0,1,5,0,4,1},
			new int[]{0,3,7,2,6,1}
		}
	};
	public static int [][][] edgeProcEdgeMask = {
		new int[][]{
			new int[]{3,2,1,0,0},
			new int[]{7,6,5,4,0}
		},
		new int[][]{
			new int[]{5,1,4,0,1},
			new int[]{7,3,6,2,1}
		},
		new int[][]{
			new int[]{6,4,2,0,2},
			new int[]{7,5,3,1,2}
		},
	};
	public static int [][] processEdgeMask = {
		new int[]{3,2,1,0},
		new int[]{7,5,6,4},
		new int[]{11,10,9,8}
	} ;
	
	
	//----------------------------------------------//
	
	public NodeType type;
	public Vector3 min;
	public int size;
	public OctreeNode[] children = new OctreeNode[8];
	public NodeInfo nodeInfo;
	
	//costruttore vuoto
	public OctreeNode(){
		this.type = NodeType.NONE;
		this.min = new Vector3 (0, 0, 0);
		this.size = 0;
		this.nodeInfo.index = -1;
		this.nodeInfo.corners = 0;
		
		for (int i=0; i<8; i++) {
			children[i] = null;
		}
	}
	
	//costruttore con tipo del nodo
	public OctreeNode(NodeType t){
		this.type = t;
		this.min = new Vector3 (0, 0, 0);
		this.size = 0;
		for (int i=0; i<8; i++) {
			children[i] = null;
		}
	}

	public OctreeNode(NodeType t, Vector3 min, int size){
		this.type = t;
		this.min = min;
		this.size = size;
	}
	
	//-----------------FUNZIONI---------------//
	//controlla se il nodo è una foglia (controlla soltanto un nodo)
	public bool IsLeaf(){
		for (int i=0; i<8; i++) {
			if (this.children[i] != null)
				return false;
		}
		return true;
	}

	public void DrawOctree(Color color, bool recursive){
		if (this == null)
			return;

		Vector3[] points = new Vector3[8];
		for (int i=0; i<8; i++) {
			points[i] = this.min+(CHILD_MIN_OFFSETS[i]*this.size);
		}

		Debug.DrawLine (points[0], points[1], color, 999);
		Debug.DrawLine (points[1], points[5], color, 999);
		Debug.DrawLine (points[5], points[4], color, 999);
		Debug.DrawLine (points[4], points[0], color, 999);

		Debug.DrawLine (points[2], points[3], color, 999);
		Debug.DrawLine (points[3], points[7], color, 999);
		Debug.DrawLine (points[7], points[6], color, 999);
		Debug.DrawLine (points[6], points[2], color, 999);

		Debug.DrawLine (points[2], points[0], color, 999);
		Debug.DrawLine (points[6], points[4], color, 999);
		Debug.DrawLine (points[7], points[5], color, 999);
		Debug.DrawLine (points[3], points[1], color, 999);

		if (recursive) {
			for (int i=0; i<8; i++) {
				if (children [i] != null)
					children [i].DrawOctree (color, recursive);
			}
		}
	}

	//metodo da richiamare per iniziare la costruzione dell'octree
	public OctreeNode BuildOctree(Vector3 min, int size, int leafsize){
		OctreeNode root = new OctreeNode();
		root.min = min;
		root.size = size;
		root.type = NodeType.INTERNAL;

		BuildOctreeNodes(root, leafsize);
		//root = SimplifyOctree(root, threshold);

		return root;
	}

	//costruisce l'octree da cima a fondo
	public OctreeNode BuildOctreeNodes(OctreeNode node, int leafsize){
		if (node == null)
			return null;
		
		if (node.size == leafsize)
			return BuildLeaf (node, leafsize);
		
		int childSize = node.size / 2;
		bool hasChildren = false;
		
		for (int i=0; i<8; i++) {
			OctreeNode child = new OctreeNode();
			child.size = childSize;
			child.min = node.min + (CHILD_MIN_OFFSETS[i]*childSize);
			child.type = NodeType.INTERNAL;
			
			node.children[i] = BuildOctreeNodes(child, leafsize);
			hasChildren |= (node.children[i] != null);
		}
		
		if (!hasChildren) {
			node = null;
			return null;
		}
	
		return node;
	}
	
	//costruisce il nodo terminale (foglia) calcolando le NodeInfo per la generazione della mesh
	public OctreeNode BuildLeaf(OctreeNode leaf, int size){
		if (leaf == null || leaf.size != size)
			return null;

		int corners = 0;
		for (int i=0; i<8; i++) {
			Vector3 cornerPos = leaf.min+(CHILD_MIN_OFFSETS[i]*size);
			float density = Density.DensityFunc(cornerPos);
			int material = density<0.0f ? SOLID : AIR;
			corners |= (material << i);
		}
		
		if (corners == 0 || corners == 255) {
			leaf = null;
			return null;
		}
		
		int edgeCount = 0;
		Vector3 averageNormal = new Vector3();
		QefSolver qef = new QefSolver ();
		
		for (int i=0; i<12 && edgeCount<MAX_CROSSINGS; i++) {
			int c1 = edgevmap[i][0];
			int c2 = edgevmap[i][1];
			
			int m1 = (corners >> c1) & 1;
			int m2 = (corners >> c2) & 1;
			
			if ((m1 == AIR && m2 == AIR) || (m1 == SOLID && m2 == SOLID))
				continue;
			
			Vector3 p1 = leaf.min+(CHILD_MIN_OFFSETS[c1]*size);
			Vector3 p2 = leaf.min+(CHILD_MIN_OFFSETS[c2]*size);
			Vector3 p = ApproximateZeroCrossingPosition(p1, p2, 8);
			Vector3 n = CalculateSurfaceNormal(p);
			qef.Add(p.x, p.y, p.z, n.x, n.y, n.z);			
			
			averageNormal += n;
			edgeCount++;
		}
		
		Vector3 qefPosition = new Vector3();
		qef.Solve (qefPosition, QEF_ERROR, QEF_SWEEPS, QEF_ERROR);
		
		NodeInfo info = new NodeInfo ();
		info.index = -1;
		info.corners = 0;
		info.position = qefPosition;
		info.qef = qef.GetData ();
		
		Vector3 min = leaf.min;
		Vector3 max = new Vector3(leaf.min.x+leaf.size, leaf.min.y+leaf.size, leaf.min.z+leaf.size);
		if (info.position.x < min.x || info.position.x > max.x ||
		    info.position.y < min.y || info.position.y > max.y ||
		    info.position.z < min.z || info.position.z > max.z) 
		{
			info.position = qef.GetMassPoint();
		}
		
		info.avgNormal = (averageNormal / (float)edgeCount).normalized;
		info.corners = corners;
		
		leaf.type = NodeType.LEAF;
		leaf.nodeInfo = info;
		return leaf;
	}

	//semplifica l'octree
	public OctreeNode SimplifyOctree(OctreeNode node, float threshold){
		if (node == null){
			return null;
		}

		if (node.type != NodeType.INTERNAL){
			// can't simplify!
			return node;
		}

		QefSolver qef = new QefSolver();
		int[] signs = new int[8]{ -1 , -1, -1, -1, -1, -1, -1, -1,};
		int midsign = -1;
		int edgeCount = 0;
		bool isCollapsible = true;
		for (int i = 0; i < 8; i++){
			node.children[i] = SimplifyOctree(node.children[i], threshold);

			if (node.children[i] != null){
				OctreeNode child = node.children[i];
				if (child.type == NodeType.INTERNAL){
					isCollapsible = false;
				}
				else{
					qef.Add(child.nodeInfo.qef);
					midsign = (child.nodeInfo.corners >> (7 - i)) & 1;
					signs[i] = (child.nodeInfo.corners >> i) & 1;
					edgeCount++;
				}
			}
		}
		if (!isCollapsible){
			// at least one child is an internal node, can't collapse
			return node;
		}

		Vector3 qefPosition = new Vector3();
		float error = qef.Solve(qefPosition, QEF_ERROR, QEF_SWEEPS, QEF_ERROR);
		Vector3 position = new Vector3(qefPosition.x, qefPosition.y, qefPosition.z);
		// at this point the masspoint will actually be a sum, so divide to make it the average
		if (error > threshold){
			// this collapse breaches the threshold
			return node;
		}

		if (position.x < node.min.x || position.x > (node.min.x + node.size) ||
		    position.y < node.min.y || position.y > (node.min.y + node.size) ||
		    position.z < node.min.z || position.z > (node.min.z + node.size)){

			position = qef.GetMassPoint();
		}

		// change the node from an internal node to a 'pseudo leaf' node
		NodeInfo info = new NodeInfo();
		for (int i = 0; i < 8; i++){
			if (signs[i] == -1){
				// Undetermined, use centre sign instead
				info.corners |= (midsign << i);
			}
			else
			{
				info.corners |= (signs[i] << i);
			}
		}
		info.avgNormal = new Vector3(0, 0, 0);
		for (int i = 0; i < 8; i++){
			if (node.children[i] != null)
			{
				if (node.children[i].type == NodeType.PSEUDO ||
				    node.children[i].type == NodeType.LEAF)
				{
					info.avgNormal += node.children[i].nodeInfo.avgNormal;
				}
			}
		}
		info.avgNormal = info.avgNormal.normalized;
		info.position = position;
		info.qef = qef.GetData();

		for (int i = 0; i < 8; i++){
			DestroyOctree(node.children[i]);
			node.children[i] = null;
		}
		node.type = NodeType.PSEUDO;
		node.nodeInfo.Set(info);
		return node;
	}

	//elimina l'albero
	public void DestroyOctree(OctreeNode node){
		if (node == null){
			return;
		}

		for (int i = 0; i < 8; i++){
			DestroyOctree(node.children[i]);
		}
		node = null;
	}

	//Calcola la normale del punto passato
	public Vector3 CalculateSurfaceNormal(Vector3 p){
		float H = 0.001f;
		float dx = Density.DensityFunc(p+new Vector3(H, 0, 0)) - Density.DensityFunc(p-new Vector3(H, 0, 0));
		float dy = Density.DensityFunc(p+new Vector3(0, H, 0)) - Density.DensityFunc(p-new Vector3(0, H, 0));
		float dz = Density.DensityFunc(p+new Vector3(0, 0, H)) - Density.DensityFunc(p-new Vector3(0, 0, H));
		Vector3 res = new Vector3 (dx, dy, dz);
		return res.normalized;
	}
	
	//approssima il punto di incontro dei vettori
	public Vector3 ApproximateZeroCrossingPosition(Vector3 p1, Vector3 p2, int steps){
		float minVal = float.MaxValue;
		float t = 0, curT = 0;
		float increment = 1.0f / (float)steps;
		
		while (curT <= 1.0f) {
			Vector3 p = p1 + ((p2-p1)*curT);
			float density = Mathf.Abs(Density.DensityFunc(p));
			if (density <minVal) {
				minVal = density;
				t = curT;
			}
			curT += increment;
		}
		return p1 + ((p2 - p1) * t);
	}

	//calcola i dati da utilizzare per il rendering della mesh
	public void GenerateMeshFromOctree(OctreeNode node, MeshData data){
		if (node == null)
			return;

		GenerateVertexIndices (node, data);
		ContourCellProc (node, data);
	}
	
	//Salva i dati dei vertici in MeshData
	public void GenerateVertexIndices(OctreeNode node, MeshData data){
		
		if (node == null)
			return;

		if (node.type != NodeType.LEAF) {
			for (int i=0; i<8; i++){
				GenerateVertexIndices(node.children[i], data);
			}
		}
		
		if (node.type != NodeType.INTERNAL) {

			/*if (node.nodeInfo.isEmpty()){
				Debug.LogError("Errore nella creazione del vertice");
				return;
			}*/
			node.nodeInfo.index = data.vertices.Count;
			data.AddVertex(node.nodeInfo.position);
			data.AddNormal(node.nodeInfo.avgNormal);
		}
	}

	//Ottiene i dati per i triangoli
	public void ContourCellProc(OctreeNode node, MeshData data){
		if (node == null)
			return;
		
		if (node.type == NodeType.INTERNAL) {
			for (int i=0; i<8; i++){
				ContourCellProc(node.children[i], data);
			}
		}
		
		for (int i=0; i<12; i++) {
			OctreeNode[] faceNodes = new OctreeNode[2];
			int[] c = {cellProcFaceMask[i][0], cellProcFaceMask[i][1]};
			
			faceNodes[0] = node.children[c[0]];
			faceNodes[1] = node.children[c[1]];
			
			ContourFaceProc(faceNodes, cellProcFaceMask[i][2], data);
		}
		
		for (int i=0; i<6; i++){
			OctreeNode[] edgeNodes = new OctreeNode[4];
			
			int[] c = new int[4]
			{
				cellProcEdgeMask[i][0],
				cellProcEdgeMask[i][1],
				cellProcEdgeMask[i][2],
				cellProcEdgeMask[i][3],
			};
			
			for (int j=0; j<4; j++){
				edgeNodes[j] = node.children[c[j]];
			}
			
			ContourEdgeProc(edgeNodes, cellProcEdgeMask[i][4], data);
		}
	}

	public void ContourFaceProc(OctreeNode[] node, int dir, MeshData data){
		if (node[0]==null || node[1]==null)
			return;

		if (node[0].type == NodeType.INTERNAL ||
		    node[1].type == NodeType.INTERNAL)
		{
			for (int i = 0; i < 4; i++)
			{
				OctreeNode[] faceNodes = new OctreeNode[2];
				int[] c =
				{
					faceProcFaceMask[dir][i][0],
					faceProcFaceMask[dir][i][1],
				};
				for (int j = 0; j < 2; j++)
				{
					if (node[j].type != NodeType.INTERNAL)
					{
						faceNodes[j] = node[j];
					}
					else
					{
						faceNodes[j] = node[j].children[c[j]];
					}
				}
				ContourFaceProc(faceNodes, faceProcFaceMask[dir][i][2], data);
			}
			int[][] orders =
			{
				new int[]{ 0, 0, 1, 1 },
				new int[]{ 0, 1, 0, 1 },
			};
			for (int i = 0; i < 4; i++)
			{
				OctreeNode[] edgeNodes = new OctreeNode[4];
				int[] c =
				{
					faceProcEdgeMask[dir][i][1],
					faceProcEdgeMask[dir][i][2],
					faceProcEdgeMask[dir][i][3],
					faceProcEdgeMask[dir][i][4],
				};
				int[] order = orders[faceProcEdgeMask[dir][i][0]];
				for (int j = 0; j < 4; j++)
				{
					if (node[order[j]].type == NodeType.LEAF ||
					    node[order[j]].type == NodeType.PSEUDO)
					{
						edgeNodes[j] = node[order[j]];
					}
					else
					{
						edgeNodes[j] = node[order[j]].children[c[j]];
					}
				}
				ContourEdgeProc(edgeNodes, faceProcEdgeMask[dir][i][5], data);
			}
		}
	}

	//ricorsione sugli "spigoli"
	public void ContourEdgeProc(OctreeNode[] node, int dir, MeshData data){
		if (node[0]==null || node[1]==null || node[2]==null || node[3]==null)
			return;

		if (node[0].type != NodeType.INTERNAL &&
		    node[1].type != NodeType.INTERNAL &&
		    node[2].type != NodeType.INTERNAL &&
		    node[3].type != NodeType.INTERNAL)
		{
			ContourProcessEdge(node, dir, data);
		}
		else
		{
			for (int i = 0; i < 2; i++)
			{
				OctreeNode[] edgeNodes = new OctreeNode[4];
				int[] c =
				{
					edgeProcEdgeMask[dir][i][0],
					edgeProcEdgeMask[dir][i][1],
					edgeProcEdgeMask[dir][i][2],
					edgeProcEdgeMask[dir][i][3],
				};

				for (int j = 0; j < 4; j++){
					if (node[j].type == NodeType.LEAF || node[j].type == NodeType.PSEUDO){
						edgeNodes[j] = node[j];
					}
					else{
						edgeNodes[j] = node[j].children[c[j]];
					}
				}
				ContourEdgeProc(edgeNodes, edgeProcEdgeMask[dir][i][4], data);
			}
		}
	}

	//creazione triangoli sui lati
	public void ContourProcessEdge(OctreeNode[] node, int dir, MeshData data){
		int minSize = 1000000; // arbitrary big number
		int minIndex = 0;
		int[] indices = { -1, -1, -1, -1 };
		bool flip = false;
		bool[] signChange = { false, false, false, false };
		for (int i = 0; i < 4; i++)
		{
			int edge = processEdgeMask[dir][i];
			int c1 = edgevmap[edge][0];
			int c2 = edgevmap[edge][1];
			int m1 = (node[i].nodeInfo.corners >> c1) & 1;
			int m2 = (node[i].nodeInfo.corners >> c2) & 1;
			if (node[i].size < minSize)
			{
				minSize = node[i].size;
				minIndex = i;
				flip = m1 != AIR;
			}
			indices[i] = node[i].nodeInfo.index;
			signChange[i] =(m1 == AIR && m2 != AIR) ||(m1 != AIR && m2 == AIR);
		}
		if (signChange[minIndex])
		{
			if (!flip)
			{
				data.AddTriangle(indices[0]);
				data.AddTriangle(indices[1]);
				data.AddTriangle(indices[3]);
				data.AddTriangle(indices[0]);
				data.AddTriangle(indices[3]);
				data.AddTriangle(indices[2]);
			}
			else
			{
				data.AddTriangle(indices[0]);
				data.AddTriangle(indices[3]);
				data.AddTriangle(indices[1]);
				data.AddTriangle(indices[0]);
				data.AddTriangle(indices[2]);
				data.AddTriangle(indices[3]);
			}
		}
	}

	public void GetInternalNodes(Dictionary<Vector3, OctreeNode> seamNodes, Vector3 pos){
		if (this == null)
			return;

		if (this.type != NodeType.LEAF) {
			for (int i=0; i<8; i++){
				if (children[i] != null)
					this.children[i].GetInternalNodes(seamNodes, pos);
			}
		}

		if (this.type == NodeType.LEAF) {
			if (this.min.x == pos.x || this.min.y == pos.y || this.min.z == pos.z)
				seamNodes.Add(this.min,this);
		}
	}

	public void GetSeamNodes(Dictionary<Vector3, OctreeNode> seamNodes, Vector3 min, int offset){
		if (this == null)
			return;

		if (this.type != NodeType.LEAF) {
			for (int i=0; i<8; i++){
				if (children[i] != null)
					this.children[i].GetSeamNodes(seamNodes, min, offset);
			}
			return;
		}

		switch (offset) {
		case 1:
			if (this.min.z == min.z)
				seamNodes.Add (this.min, this);
			break;
		case 2:
			if (this.min.y == min.y)
				seamNodes.Add (this.min, this);
			break;
		case 3:
			if (this.min.y == min.y && this.min.z == min.z)
				seamNodes.Add (this.min, this);
			break;
		case 4:
			if (this.min.x == min.x)
				seamNodes.Add (this.min, this);
			break;
		case 5:
			if (this.min.x == min.x && this.min.z == min.z)
				seamNodes.Add (this.min, this);
			break;
		case 6:
			if (this.min.x == min.x && this.min.y == min.y)
				seamNodes.Add (this.min, this);
			break;
		case 7:
			if (this.min.x == min.x && this.min.y == min.y && this.min.z == min.z)
				seamNodes.Add (this.min, this);
			break;

		}
	}

	public OctreeNode BuildOctreeUpwards(Dictionary<Vector3, OctreeNode> seamNodes, Vector3 chunkMin){

		Dictionary<Vector3, OctreeNode> parentNodes = new Dictionary<Vector3, OctreeNode> ();

		foreach (KeyValuePair<Vector3, OctreeNode> n in seamNodes) {
			if (n.Value.size == Chunk.size*2){
				return n.Value;
			}

			int parentSize = 2*n.Value.size;
			Vector3 deltaPos = n.Value.min - chunkMin;
			Vector3 offset = new Vector3(deltaPos.x%parentSize, deltaPos.y%parentSize, deltaPos.z%parentSize);
			Vector3 minoffs = offset/n.Value.size;
			Vector3 parentPos = n.Value.min - offset;

			OctreeNode parentNode;
			if (!parentNodes.TryGetValue(parentPos, out parentNode)){
				parentNode = new OctreeNode(NodeType.INTERNAL, parentPos, parentSize);
				parentNode.AddChild(n.Value, minoffs);
				parentNodes.Add(parentPos, parentNode);
			}
			else{
				parentNode.AddChild(n.Value, minoffs);
			}
		}
		return BuildOctreeUpwards (parentNodes, chunkMin);
	}

	public void AddChild ( OctreeNode node, Vector3 offset){
		for (int i=0; i<8; i++) {
			if (offset == CHILD_MIN_OFFSETS[i]){
				this.children[i] = node;
				return;
			}
		}
	}
}
