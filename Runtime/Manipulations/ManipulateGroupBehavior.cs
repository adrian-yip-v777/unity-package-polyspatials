using System.Collections.Generic;
using UnityEngine;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations
{
    [RequireComponent(typeof(MeshFilter))]
    public class ManipulateGroupBehavior : ManipulationTargetBehavior
    {
        [SerializeField]
        private List<Transform> children = new();
        
        private void OnValidate()
        {
            if (children == null || children.Count == 0) return;
                
            var index = 0;
            while (index < children.Count)
            {
                if (children[index].parent == transform.parent)
                {
                    index++;
                    continue;
                }

                children.RemoveAt(index);
            }
        }

        protected override void OnManipulationStarted(IManipulationStartEvent eventData)
        {
            base.OnManipulationStarted(eventData);

            if (eventData.Selectable.ManipulationTarget is not ManipulateGroupBehavior behavior || behavior != this)
                return;
            
            children.ForEach(child =>
            {
                child.SetParent(TransformCache);
            });
        }

        protected override void OnManipulationEnded(IManipulationEndEvent eventData)
        {
            base.OnManipulationEnded(eventData);
            
            if (eventData.Selectable.ManipulationTarget is not ManipulateGroupBehavior behavior || behavior != this)
                return;
            
            children.ForEach(child =>
            {
                child.SetParent(TransformCache.parent);
            });
        }
    }
}