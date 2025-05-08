using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations.Strategies
{
    /// <summary>
    /// Move and rotate the target with your wrist.
    /// </summary>
    [CreateAssetMenu(menuName = "vz777/Spatial/Manipulate Strategies/Movement + Rotation", fileName = "Spatial Movement and Rotation")]
    public class MoveAndRotateStrategy : ManipulationStrategy
    {
        private EventBus _eventBus;
        private Quaternion _rotationOffset;
        private Vector3 _positionOffset;
        private ISpatialSelectable _selectable;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public override bool TryRun(SpatialPointerState[] spatialPointers, ISpatialSelectable selectable)
        {
            // Only accept one pointer to input.
            if (spatialPointers.Length != 1)
                return false;
            
            var primaryPointer = spatialPointers[0];
            
            // If it's a new selectable, initialize the caches.
            if (_selectable != selectable)
            {
                // Publish the start event first because the manipulation target might change according to it.
                _selectable = selectable;
                _eventBus.Publish(new MoveAndRotateStartEvent(_selectable, manipulationMode));
                
                // Calculate the offset.
                var interactionPosition = primaryPointer.interactionPosition;
                var inverseDeviceRotation = Quaternion.Inverse(primaryPointer.inputDeviceRotation);
                _rotationOffset = inverseDeviceRotation * selectable.ManipulationTarget.Rotation;
                _positionOffset = inverseDeviceRotation * (selectable.ManipulationTarget.Position - interactionPosition);
            }
            
            switch (primaryPointer.phase)
            {
                // Wait for any movement.
                case SpatialPointerPhase.Began:
                    return true;
                
                case SpatialPointerPhase.Moved:
                    // Position the piece at the interaction position, maintaining the same relative transform from interaction position to selection pivot
                    var deviceRotation = primaryPointer.inputDeviceRotation;
                    var position = primaryPointer.interactionPosition + deviceRotation * _positionOffset;
                    var rotation = deviceRotation * _rotationOffset;
                    _eventBus.Publish(new MoveAndRotateUpdateEvent(_selectable, position, rotation));
                    return true;
                
                case SpatialPointerPhase.None:
                case SpatialPointerPhase.Ended:
                case SpatialPointerPhase.Cancelled:
                    _eventBus.Publish(new MoveAndRotateEndEvent(_selectable));
                    return false;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Reset()
        {
            _selectable = null; 
            _rotationOffset = default;
            _positionOffset = default;
        }
    }
}