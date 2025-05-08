using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class MoveAndRotateStartEvent : EventBase, IManipulationStartEvent
    {
        public MoveAndRotateStartEvent(ISpatialSelectable selectable, ManipulationMode manipulationMode)
        {
            Selectable = selectable;
            ManipulationMode = manipulationMode;
        }
        
        public ISpatialSelectable Selectable { get; }
        public ManipulationMode ManipulationMode { get; }
    }
}