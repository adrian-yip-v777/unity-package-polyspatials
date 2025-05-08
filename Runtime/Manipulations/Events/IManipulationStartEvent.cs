using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public interface IManipulationStartEvent : IEvent
    {
        ISpatialSelectable Selectable { get; }
        
        /// <summary>
        /// Which target do you want to manipulate at the end.
        /// </summary>
        ManipulationMode ManipulationMode { get; }
    }
}