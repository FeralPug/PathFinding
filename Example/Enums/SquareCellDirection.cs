using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    public enum SquareCellDirection
    {
        North,
        East,
        South,
        West
    }

    public static class SquareCellDirectionExtensions
    {
        public static SquareCellDirection GetReverseDirection(this SquareCellDirection direction)
        {
            switch (direction)
            {
                case SquareCellDirection.North:
                    return SquareCellDirection.South;
                case SquareCellDirection.East:
                    return SquareCellDirection.West;
                case SquareCellDirection.South:
                    return SquareCellDirection.North;
                case SquareCellDirection.West:
                    return SquareCellDirection.East;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public static Vector2Int ToCoordOffset(this SquareCellDirection direction)
        {
            switch (direction)
            {
                case SquareCellDirection.North:
                    return Vector2Int.up;
                case SquareCellDirection.East:
                    return Vector2Int.right;
                case SquareCellDirection.South:
                    return Vector2Int.down;
                case SquareCellDirection.West:
                    return Vector2Int.left;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
