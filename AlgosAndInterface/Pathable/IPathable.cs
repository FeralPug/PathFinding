namespace FeralPug.PathFinding
{
    public interface IPathable<T>
    {
        public float PathCost { get; }

        public bool IsPathable { get; }

        public int NeighboorsCount { get; }

        public T NeighboorAtIndex(int i);

        public int DistanceToOther(T other);
    }
}
