using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations.Strategies
{
    [CreateAssetMenu(menuName = "vz777/Spatial/Manipulate Strategies/Scaling + Rotation", fileName = "Spatial Scaling and Rotation")]
    public class ScaleAndRotateStrategy : ManipulationStrategy
    {
        public float SmallestScaleValue = 0.1f;
        public float LargestScaleValue = 2f;

        [Inject]
        private EventBus _eventBus;
        private ISpatialSelectable _selectedObject;
        private float _initialHandsDistance;
        private Vector3 _startScale;
        private Vector3 _previousPinchVector;
        private bool _isRotationInitialized;

        /// <summary>
        /// Scale and rotate the object by two-handed pinching.
        /// </summary>
        /// <returns>should end running the next strategy?</returns>
        public override bool TryRun(SpatialPointerState[] spatialPointers, ISpatialSelectable selectable)
        {
            if (spatialPointers.Length != 2) 
                return false;
            
            var primaryPointer = spatialPointers[0];
            var secondaryPointer = spatialPointers[1];
            
            // If the second pointer has ended, then try the next strategy in the queue.
            if (primaryPointer.phase is SpatialPointerPhase.Ended || secondaryPointer.phase is SpatialPointerPhase.Ended)
            {
                _eventBus.Publish(new ScaleAndRotateEndEvent(_selectedObject));
                return false;
            }
            
            var handsDistance = Vector3.Distance(primaryPointer.inputDevicePosition, secondaryPointer.inputDevicePosition);
            
            // Initialize for first touch.
            if (_selectedObject == null)
            {
                _selectedObject = selectable;
                // Publish the event first since the ManipulationTarget might have changed upon event subscription.
                _eventBus.Publish(new ScaleAndRotateStartEvent (_selectedObject, manipulationMode));
                
                _initialHandsDistance = handsDistance;
                _startScale = _selectedObject.ManipulationTarget.LocalScale;
                _previousPinchVector = secondaryPointer.inputDevicePosition - primaryPointer.inputDevicePosition;
                
                return true;
            }

            // Quit to wait for movement on the next frame.
            if (primaryPointer.phase is not SpatialPointerPhase.Moved &&
                secondaryPointer.phase is not SpatialPointerPhase.Moved) 
                return true;
            
            var scaleFactor = handsDistance / _initialHandsDistance;
            var desiredScale = _startScale * scaleFactor;
            
            // Make sure the scale doesn't smaller than a value.
            var initialScale = _selectedObject.ManipulationTarget.InitialScale;
            var x = Mathf.Min(Mathf.Max(desiredScale.x, initialScale.x * SmallestScaleValue), initialScale.x * LargestScaleValue);
            var y = Mathf.Min(Mathf.Max(desiredScale.y, initialScale.y * SmallestScaleValue), initialScale.y * LargestScaleValue);
            var z = Mathf.Min(Mathf.Max(desiredScale.z, initialScale.z * SmallestScaleValue), initialScale.z * LargestScaleValue);
            var finalScale = new Vector3(x, y, z);
            
            // Handle Rotation
            var currentPinchVector = secondaryPointer.inputDevicePosition - primaryPointer.inputDevicePosition;
            
            // Don't rotate if not exceeded the rotation threshold, otherwise it will be too sensitive.
            var rotationDelta = Quaternion.identity;
            var pinchDistance = Vector3.Distance(currentPinchVector, _previousPinchVector);
            
            if (_isRotationInitialized || pinchDistance > .05f)
            {
                if (!_isRotationInitialized)
                {
                    _isRotationInitialized = true;
                    _previousPinchVector = currentPinchVector;
                }
                
                rotationDelta = Quaternion.FromToRotation(_previousPinchVector, currentPinchVector);
                _previousPinchVector = currentPinchVector;
            }

            // Prepare the current scale for display.
            var currentScale = finalScale.x / initialScale.x;
            
            // Publish update event.
            _eventBus.Publish(new ScaleAndRotateUpdateEvent (_selectedObject, finalScale, rotationDelta, primaryPointer, secondaryPointer, handsDistance, currentScale));
            return true;
        }

        public override void Reset()
        {
            _selectedObject = null;
            _initialHandsDistance = 0;
            _startScale = default;
            _isRotationInitialized = false;
        }
    }
}