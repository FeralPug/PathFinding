namespace FeralPug.PathFinding
{
    public struct PathRequest<T> where T : class, IPathable<T>
    {
        public T start;
        public T end;
        public System.Action<PathableWrapper<T>[], bool> callBack;

        public PathRequest(T start, T end, System.Action<PathableWrapper<T>[], bool> callBack)
        {
            this.start = start;
            this.end = end;
            this.callBack = callBack;
        }
    }
}
