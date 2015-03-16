using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class ThreadManager {

	public static ThreadManager Instance { get; private set; }

	Timer timer;
	Vector3 priorityPos = new Vector3();
	List<ChunkThread> runningThreads;
	Queue<ChunkThread> queue { get; set; }
	object threadLock = new object();
	int maxThreads;
	int mutex;

	static ThreadManager(){
		Instance = new ThreadManager(2);
	}

	public ThreadManager(int threads){

		//lock
		Interlocked.Exchange (ref mutex, 1);

		//thread queue initialized
		queue = new Queue<ChunkThread> (16384);

		//running and idle threads lists initialized
		runningThreads = new List<ChunkThread>();
		maxThreads = threads;

		//unlock
		Interlocked.Exchange (ref mutex, 0);

		this.timer = new Timer ((obj) => {
			ProcessThread ();}, null, 8, 8);
	}

	public void ProcessThread(){
		int mval = (int) Interlocked.CompareExchange (ref mutex, 0, 1);
		if (mval != 0)
			return;

		lock (queue) {
			if (queue.Count>0 && runningThreads.Count<maxThreads){
				ChunkThread thread = queue.Dequeue();
				RunThread(thread);

				if (thread != null){
					thread.Start();
				}
			}
		}
	}

	public void AddThread(ChunkThread t){
		lock (queue) {
			queue.Enqueue(t);
		}
	}

	public void RemoveThread(ChunkThread t){
		lock (threadLock) {
			runningThreads.Remove (t);
		}
	}

	public void RunThread(ChunkThread t){
		lock (threadLock) {
			runningThreads.Add(t);
		}
	}

	public int GetWaitingThreads(){
		lock (queue) {
			return queue.Count;
		}
	}

	public int GetRunningThreads(){
		lock (threadLock) {
			return runningThreads.Count;
		}
	}

	public void Clear(){
		lock (queue) {
			queue.Clear();
		}
		lock (threadLock) {
			runningThreads.Clear();
		}
	}

}
