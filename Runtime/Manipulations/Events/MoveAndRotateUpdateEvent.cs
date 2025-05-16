using UnityEngine;
using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class MoveAndRotateUpdateEvent : EventBase, IManipulationUpdateEvent
    {
        public MoveAndRotateUpdateEvent(
            ISpatialSelectable selectable,
            Vector3 desiredPosition,
            Quaternion desiredRotation)
        {
            Selectable = selectable;
            DesiredPosition = desiredPosition;
            DesiredRotation = desiredRotation;
        }

        public ISpatialSelectable Selectable { get; }
        public Vector3? DesiredPosition { get; }
        public Quaternion? DesiredRotation { get; }
        public Vector3? DesiredLocalScale => null;
    }
}