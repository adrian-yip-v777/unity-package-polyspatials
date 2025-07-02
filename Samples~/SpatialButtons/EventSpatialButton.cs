using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

namespace vz777.PolySpatials
{
    public class EventSpatialButton : SpatialButton
    {
        [SerializeField]
        private UnityEvent<SpatialPointerState> onClick;
        
        protected override void OnClicked(SpatialPointerState pointer)
        {
            base.OnClicked(pointer);
            onClick?.Invoke(pointer);
        }
    }
}