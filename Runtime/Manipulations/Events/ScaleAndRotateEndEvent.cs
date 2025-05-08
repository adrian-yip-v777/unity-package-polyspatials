using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class ScaleAndRotateEndEvent : EventBase, IManipulationEndEvent
    {
        public ScaleAndRotateEndEvent(ISpatialSelectable selectable)
        {
            Selectable = selectable;
        }
        
        public ISpatialSelectable Selectable { get; }
    }
}