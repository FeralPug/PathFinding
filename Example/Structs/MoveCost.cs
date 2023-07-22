using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    [System.Serializable]
    public struct MoveCost
    {
        public float weight;
        public float cost;
        public Color color;
    }
}
