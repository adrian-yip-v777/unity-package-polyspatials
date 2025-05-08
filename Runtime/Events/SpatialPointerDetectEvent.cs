using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using vz777.Events;

namespace vz777.PolySpatials.Events
{
    /// <summary>
    /// Publish when any spatial pointers have been detected.
    /// </summary>
    public class SpatialPointerDetectEvent : EventBase
    {
        public SpatialPointerDetectEvent(IEnumerable<SpatialPointerState> spatialPointers)
        {
            SpatialPointers = spatialPointers;
        }
        
        public IEnumerable<SpatialPointerState> SpatialPointers { get; }
    }
}