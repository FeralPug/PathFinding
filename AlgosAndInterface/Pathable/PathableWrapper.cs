namespace FeralPug.PathFinding
{
    public class PathableWrapper<T> : IHeapItem<PathableWrapper<T>> where T : class, IPathable<T>
    {
        public T pathable;

        public float Distance { get; set; }

        public float SearchHeuristic { get; set; }

        public float SearchPriority => Distance + SearchHeuristic;

        public PathableWrapper<T> From { get; set; }

        public int HeapIndex { get; set; }

        public int CompareTo(PathableWrapper<T> other)
        {
            int compare = SearchPriority.CompareTo(other.SearchPriority);

            if (compare == 0)
            {
                compare = SearchHeuristic.CompareTo(other.SearchHeuristic);
            }

            return compare;
        }

        public static bool operator == (PathableWrapper<T> a, PathableWrapper<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.pathable == b.pathable;
        }

        public static bool operator !=(PathableWrapper<T> a, PathableWrapper<T> b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return a.pathable != b.pathable;
        }

        public override bool Equals(object obj) => Equals(obj as PathableWrapper<T>);

        public bool Equals(PathableWrapper<T> other)
        {
            if (other is null) return false;
            if (GetType() != other.GetType()) return false;
            if(ReferenceEquals(this, other)) return true;
            if (pathable == null && other.pathable == null) return true;
            if (pathable == null || other.pathable == null) return false;

            return pathable.Equals(other.pathable);
        }

        public override int GetHashCode()
        {
            if (pathable == null) return 0;

            return pathable.GetHashCode();
        }

    }
}
