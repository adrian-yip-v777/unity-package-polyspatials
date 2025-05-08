using vz777.Events;

namespace vz777.PolySpatials.Manipulations.Events
{
    public class ScaleAndRotateStartEvent : EventBase, IManipulationStartEvent
    {
        public ScaleAndRotateStartEvent(ISpatialSelectable selectable, ManipulationMode manipulationMode)
        {
            Selectable = selectable;
            ManipulationMode = manipulationMode;
        }

        public ISpatialSelectable Selectable { get; }
        public ManipulationMode ManipulationMode { get; }
    }
}