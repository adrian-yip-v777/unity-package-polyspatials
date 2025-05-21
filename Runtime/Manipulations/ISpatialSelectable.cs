using UnityEngine;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// If any object implements this interface, which means it can be selected by a Spatial Raycast.
    /// </summary>
    public interface ISpatialSelectable
    {
        /// <summary>
        /// Indicate if this object is selectable by spatial raycast.
        /// </summary>
        bool IsSelectable { get; }
        
        /// <summary>
        /// The transform to manipulate with after ray-casting.
        /// </summary>
        public IManipulateTarget ManipulationTarget { get; }
        
        /// <summary>
        /// The manipulation mode of this selectable.
        /// </summary>
        public ManipulationMode Mode { get; }
    }
}