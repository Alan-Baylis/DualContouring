using UnityEngine;
using System.Collections;
using Qef;

public struct NodeInfo{
	public int index;
	public int corners;
	public Vector3 position;
	public Vector3 avgNormal;
	public QefData qef;
	
	public NodeInfo(int index, int corners, Vector3 position, Vector3 averageNormal, QefData qef)
	{
		this.index = -1;
		this.corners = 0;
		this.position = new Vector3(0, 0, 0);
		this.avgNormal = new Vector3(0, 0, 0);
		this.qef = new QefData();
	}

	public void Set(NodeInfo info){
		this.index = info.index;
		this.corners = info.corners;
		this.position = info.position;
		this.avgNormal = info.avgNormal;
		this.qef = info.qef;
	}
}
