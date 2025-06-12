using System.Collections.Generic;
using UnityEngine;
using vz777.Foundations;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations
{
    [RequireComponent(typeof(MeshFilter))]
    public class ManipulateGroupBehavior : ManipulationTargetBehavior
    {
        [SerializeField]
        private List<Transform> children = new();
        public IReadOnlyCollection<Transform> Children => children;
        public bool IsManipulating;

        protected override void OnEnable()
        {
            base.OnEnable();
            LerpEnd += OnLerpEnded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LerpEnd += OnLerpEnded;
        }

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

        public void MoveInLerp(TrsData trsData)
        {
            DesiredTransform = trsData;

            children.ForEach(child =>
            {
                child.SetParent(TransformCache);
            });

            LerpCoroutine ??= StartCoroutine(Lerp());
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

            IsManipulating = true;
        }

        protected override void OnManipulationEnded(IManipulationEndEvent @event)
        {
            base.OnManipulationEnded(@event);
            
            if (@event.Selectable.ManipulationTarget is not ManipulateGroupBehavior behavior || behavior != this)
                return;
            
            IsManipulating = false;
        }

        private void OnLerpEnded()
        {
            if (IsManipulating) return;
            
            children.ForEach(child =>
            {
                child.SetParent(TransformCache.parent);
            });
        }
    }
}