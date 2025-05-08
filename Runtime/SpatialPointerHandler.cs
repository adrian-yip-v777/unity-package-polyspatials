using System.Collections.Generic;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using VContainer.Unity;
using vz777.Events;
using vz777.Foundations;
using vz777.PolySpatials.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace vz777.PolySpatials
{
    /// <summary>
    /// Responsible for detect spatial pointer and fire events.
    /// </summary>
    public class SpatialPointerHandler : DisposableBase, ITickable, IStartable
    {
        private readonly EventBus _eventBus;

        public SpatialPointerHandler(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        ~SpatialPointerHandler()
        {
            EnhancedTouchSupport.Disable();
        }
        
        public void Start()
        {
            EnhancedTouchSupport.Enable();
        }
        
        public void Tick()
        {
            var touches = Touch.activeTouches;
            if (touches.Count == 0) return;
            
            // Convert touches to spatial pointers.
            var spatialPointers = new List<SpatialPointerState>();
            foreach (var touch in touches)
            {
                if (!EnhancedSpatialPointerSupport.TryGetPointerState(touch, out var state) && state.Kind is SpatialPointerKind.Touch)
                    continue;
                
                spatialPointers.Add(state);
            }
            
            if (spatialPointers.Count == 0) return;
            
            // Publish the event when there is any spatial pointers.
            _eventBus.Publish(new SpatialPointerDetectEvent (spatialPointers));
        }

        protected override void DisposeManagedResources()
        {
            EnhancedTouchSupport.Disable();
        }
    }
}