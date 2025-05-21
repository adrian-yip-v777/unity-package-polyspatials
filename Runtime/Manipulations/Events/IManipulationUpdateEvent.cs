using UnityEngine;
using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public interface IManipulationUpdateEvent : IEvent
    {
        ISpatialSelectable Selectable { get; }
        Vector3? DesiredPosition { get; }
        Quaternion? DesiredRotation { get; }
        Quaternion? RotationDelta { get; }
        Vector3? DesiredLocalScale { get; }
    }
}