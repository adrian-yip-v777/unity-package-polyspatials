using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class MoveAndRotateEndEvent : EventBase, IManipulationEndEvent
    {
        public MoveAndRotateEndEvent(ISpatialSelectable selectable)
        {
            Selectable = selectable;
        }
        
        public ISpatialSelectable Selectable { get; }
    }
}