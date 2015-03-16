using UnityEngine;
using System.Collections;
using System.Threading;

public class ChunkThread : ThreadedJob {

	public Chunk chunk;

	// Use this for initialization
	protected override void ThreadFunction()
	{
		chunk.root = chunk.root.BuildOctree (chunk.pos, Chunk.size, chunk.leafsize);
	}

	protected override void OnFinished()
	{
		chunk.manager.RemoveThread (this);
		chunk.RenderChunk ();
	}
}
