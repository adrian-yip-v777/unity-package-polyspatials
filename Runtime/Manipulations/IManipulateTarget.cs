using UnityEngine;

namespace vz777.PolySpatials.Manipulations
{
    public interface IManipulateTarget
    {
        string Name { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 LocalScale { get; }
        Vector3? DesiredLocalScale { get; }
        Vector3 InitialScale { get; }
    }
}