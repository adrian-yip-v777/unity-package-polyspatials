using UnityEngine;
using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public interface IManipulationUpdateEvent : IEvent
    {
        ISpatialSelectable Selectable { get; }
        public Vector3? DesiredPosition { get; }
        public Quaternion? DesiredRotation { get; }
        public Vector3? DesiredLocalScale { get; }
    }
}