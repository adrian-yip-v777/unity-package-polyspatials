using UnityEngine;

namespace vz777.PolySpatials
{
    /// <summary>
    /// Containing translation, rotation, and scale data.
    /// </summary>
    public readonly struct TrsData
    {
        public TrsData(Transform transform, bool isWorldSpace)
        {
            IsWorldSpace = isWorldSpace;
            
            if (isWorldSpace)
            {
                Position = transform.position;
                Rotation = transform.rotation;
                Scale = transform.lossyScale;
                return;
            }
            
            Position = transform.localPosition;
            Rotation = transform.localRotation;
            Scale = transform.localScale;
        }

        public TrsData(Vector3 position, Quaternion rotation, Vector3 scale, bool isWorldSpace)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            IsWorldSpace = isWorldSpace;
        }

        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }
        public bool IsWorldSpace { get; }

        public override string ToString()
        {
            return $"T: {Position} R: {Rotation} S: {Scale}";
        }
    }
}