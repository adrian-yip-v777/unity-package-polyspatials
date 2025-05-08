using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public interface IManipulationUpdateEvent : IEvent
    {
        ISpatialSelectable Selectable { get; }
    }
}