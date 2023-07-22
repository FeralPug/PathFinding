using System.Collections.Generic;
using System.Linq;

namespace FeralPug.PathFinding
{
    //https://catlikecoding.com/unity/tutorials/hex-map/part-15/
    public class PathFinder<T> where T : class, IPathable<T>
    {
        public int MaxPathableCount { get; set; }

        public PathFinder(int maxPathableCount)
        {
            MaxPathableCount = maxPathableCount;
        }

        public void FindPath(PathRequest<T> pathRequest, System.Action<PathResult<T>> callBack)
        {
            callBack(GetPathResult(pathRequest));
        }

        public PathResult<T> FindPath(PathRequest<T> pathRequest)
        {
            return GetPathResult(pathRequest);
        }

        public PathableWrapper<T>[] GetPathablesInRange(T target, float range, bool obstructed)
        {
            HashSet<PathableWrapper<T>> pathables = new HashSet<PathableWrapper<T>>();
            Queue<PathableWrapper<T>> nextPathables = new Queue<PathableWrapper<T>>();

            PathableWrapper<T> targetWrapper = new PathableWrapper<T>
            {
                pathable = target,
                Distance = 0,
            };

            nextPathables.Enqueue(targetWrapper);

            while(nextPathables.Count > 0)
            {
                var currentWrapper = nextPathables.Dequeue();
                if (currentWrapper == null) continue;

                if (!pathables.Contains(currentWrapper))
                {
                    pathables.Add(currentWrapper);

                    int neighboors = currentWrapper.pathable.NeighboorsCount;
                    for (int i = 0; i < neighboors; i++)
                    {
                        var next = currentWrapper.pathable.NeighboorAtIndex(i);
                        if (next == null || (!next.IsPathable && obstructed)) continue;

                        var nextWrapper = new PathableWrapper<T>
                        {
                            pathable = next,
                            Distance = currentWrapper.Distance + 1
                        };

                        if (nextWrapper.Distance <= range && !nextPathables.Contains(nextWrapper))
                        {
                            nextPathables.Enqueue(nextWrapper);
                        }
                    }
                }
            }

            return pathables.ToArray();
        }

        public PathableWrapper<T>[] GetPathablesInMovementCostRange(T target, float range)
        {
            HashSet<PathableWrapper<T>> pathables = new HashSet<PathableWrapper<T>>();
            Queue<PathableWrapper<T>> nextPathables = new Queue<PathableWrapper<T>>();

            PathableWrapper<T> targetWrapper = new PathableWrapper<T>
            {
                pathable = target,
                Distance = 0,
            };

            nextPathables.Enqueue(targetWrapper);

            while (nextPathables.Count > 0)
            {
                var currentWrapper = nextPathables.Dequeue();
                if (currentWrapper == null) continue;

                if (!pathables.Contains(currentWrapper))
                {
                    pathables.Add(currentWrapper);

                    int neighboors = currentWrapper.pathable.NeighboorsCount;
                    for (int i = 0; i < neighboors; i++)
                    {
                        var next = currentWrapper.pathable.NeighboorAtIndex(i);
                        if (next == null || !next.IsPathable) continue;

                        var nextWrapper = new PathableWrapper<T>
                        {
                            pathable = next,
                            Distance = currentWrapper.Distance + next.PathCost,
                        };

                        if (nextWrapper.Distance <= range && !nextPathables.Contains(nextWrapper))
                        {
                            nextPathables.Enqueue(nextWrapper);
                        }
                    }
                }
            }

            return pathables.ToArray();
        }

        PathResult<T> GetPathResult(PathRequest<T> pathRequest)
        {
            Heap<PathableWrapper<T>> openSet = new Heap<PathableWrapper<T>>(MaxPathableCount);
            HashSet<PathableWrapper<T>> closedSet = new HashSet<PathableWrapper<T>>();

            PathableWrapper<T> start = new PathableWrapper<T>
            {
                pathable = pathRequest.start,
                Distance = 0,
                SearchHeuristic = pathRequest.start.DistanceToOther(pathRequest.end)
            };

            openSet.Add(start);

            PathableWrapper<T> end = new PathableWrapper<T>()
            {
                pathable = pathRequest.end
            };

            while(openSet.Count > 0)
            {
                var current = openSet.RemoveFirst();
                closedSet.Add(current);

                if(current == end)
                {
                    return new PathResult<T>(GetPath(current), true, pathRequest.callBack);
                }

                int neighboors = current.pathable.NeighboorsCount;

                for(int i = 0; i < neighboors; i++)
                {
                    var next = current.pathable.NeighboorAtIndex(i);

                    if (next == null || !next.IsPathable) continue;

                    var nextWrapper = new PathableWrapper<T>
                    {
                        pathable = next,
                    };

                    if (closedSet.Contains(nextWrapper)) continue;

                    nextWrapper.Distance = current.Distance + nextWrapper.pathable.PathCost;
                    nextWrapper.From = current;

                    var openSetRecord = openSet.GetRecord(nextWrapper);
                    if(openSetRecord == null)
                    {                       
                        nextWrapper.SearchHeuristic = next.DistanceToOther(end.pathable);
                        openSet.Add(nextWrapper);
                    }
                    else if(nextWrapper.Distance < openSetRecord.Distance)
                    {
                        openSetRecord.Distance = nextWrapper.Distance;
                        openSetRecord.From = nextWrapper.From;
                        openSet.UpdateItem(openSetRecord);
                    }
                }
            }

            return new PathResult<T>(null, false, pathRequest.callBack);
        }

        PathableWrapper<T>[] GetPath(PathableWrapper<T> endWrapper)
        {
            int i = 1;

            PathableWrapper<T> from = endWrapper.From;

            while(from != null)
            {
                i++;
                from = from.From;
            }

            PathableWrapper<T>[] path = new PathableWrapper<T>[i--];

            for(; i >= 0; i--)
            {
                path[i] = endWrapper;
                endWrapper = endWrapper.From;
            }

            return path;
        }
    }
}
