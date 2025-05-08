using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public interface IManipulationEndEvent : IEvent
    {
        ISpatialSelectable Selectable { get; }
    }
}