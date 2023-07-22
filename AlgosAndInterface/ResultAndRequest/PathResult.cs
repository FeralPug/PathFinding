namespace FeralPug.PathFinding
{
    public struct PathResult<T> where T : class, IPathable<T>
    {
        public PathableWrapper<T>[] pathableWrappers;
        public bool success;
        public System.Action<PathableWrapper<T>[], bool> callBack;

        public PathResult(PathableWrapper<T>[] pathableWrappers, bool success, 
            System.Action<PathableWrapper<T>[], bool> callBack)
        {
            this.pathableWrappers = pathableWrappers;
            this.success = success;
            this.callBack = callBack;
        }
    }
}
