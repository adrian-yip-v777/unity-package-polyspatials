using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class ScaleAndRotateUpdateEvent : EventBase, IManipulationUpdateEvent
    {
        public ScaleAndRotateUpdateEvent(ISpatialSelectable selectable, Vector3 desiredLocalScale, Quaternion rotationDelta, SpatialPointerState primaryPointer, SpatialPointerState secondaryPointer, float currentDistance, float currentScale)
        {
            Selectable = selectable;
            RotationDelta = rotationDelta;
            DesiredLocalScale = desiredLocalScale;
            PrimaryPointer = primaryPointer;
            SecondaryPointer = secondaryPointer;
            CurrentDistance = currentDistance;
            CurrentScale = currentScale;
        }

        public ISpatialSelectable Selectable { get; }
        public Vector3? DesiredPosition => null;
        public Quaternion? DesiredRotation => null;
        public Quaternion? RotationDelta { get; }
        public Vector3? DesiredLocalScale { get; }
        public SpatialPointerState PrimaryPointer { get; }
        public SpatialPointerState SecondaryPointer { get; }
        public float CurrentDistance { get; }
        public float CurrentScale { get; }
    }
}