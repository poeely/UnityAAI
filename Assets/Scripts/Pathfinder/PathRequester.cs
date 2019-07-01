using System;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

// Author: Vincent J.S. Versnel

// Contains all necessary information for the copmletion of a path request.
public class PathThreadInfo
{
    public NavGraph navGraph;

    public Vector3 source;
    public Vector3 target;

    public Path path;
    public Action<Path> callback;

    public PathThreadInfo(NavGraph navGraph, Vector3 source, Vector3 target, Action<Path> callback)
    {
        this.navGraph = navGraph;
        this.source = source;
        this.target = target;
        this.callback = callback;
    }
}

/// <summary>
/// A globally available service to allow clients access to paths without interfering with the framerate.
/// </summary>
public class PathRequester : MonoBehaviour
{
    private static PathRequester instance;

    public static PathRequester Instance
    {
        get { return instance; }
    }

    private PathFinder pathFinder;
    private Queue<PathThreadInfo> completedQueue;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        pathFinder = new PathFinder();
        completedQueue = new Queue<PathThreadInfo>();

        ThreadPool.SetMinThreads(10, 10);
    }

    public void RequestPath(NavGraph graph, Vector3 source, Vector3 target, Action<Path> callback)
    {
        PathThreadInfo threadInfo = new PathThreadInfo(graph, source, target, callback);
        ThreadPool.QueueUserWorkItem(SearchAStarThread, threadInfo);
    }

    private void SearchAStarThread(System.Object stateInfo)
    {
        PathThreadInfo threadInfo = (PathThreadInfo)stateInfo;

        // Locking the pathfinder was not intentional...
        // Unlocking it caused for gigabytes of memory leaks...
        // We were not able to find the cause however, so we decided to lock it.
        lock(pathFinder)
        {
            threadInfo.path = pathFinder.SearchAStar(threadInfo.navGraph, threadInfo.source, threadInfo.target);
        }

        // Queues are not thread safe.
        lock(completedQueue)
        {
            completedQueue.Enqueue(threadInfo);
        }
    }

    private void Update()
    {
        // When there are paths complete, send them off with the designated callbacks.
        for (int i = 0; i < completedQueue.Count; i++)
        {
            PathThreadInfo info = completedQueue.Dequeue();
            info.callback.Invoke(info.path);
        }
    }
}
