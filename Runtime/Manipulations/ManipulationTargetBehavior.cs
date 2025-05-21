using System.Collections;
using UnityEngine;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// Responsible for being smoothly manipulated on transform by spatial selection.
    /// </summary>
    public class ManipulationTargetBehavior : MonoBehaviour, IManipulateTarget
    {
        public float Smoothness = 5f;
        protected Coroutine lerpCoroutine;
        protected Vector3? desiredPosition;
        protected Quaternion? desiredRotation;
        
        public Vector3 Position => TransformCache.position;
        public Quaternion Rotation => TransformCache.rotation;
        public Vector3 LocalScale => TransformCache.localScale;
        public Vector3? DesiredLocalScale { get; protected set; }
        public Vector3 InitialScale { get; private set; }

        /// <summary>
        /// The display name of this selectable.
        /// </summary>
        public string Name => name;
        
        protected EventBus EventBus { get; private set; }
        protected Transform TransformCache { get; private set; }

        [Inject]
        private void Construct(EventBus eventBus)
        {
            EventBus = eventBus;
        }

        protected virtual void Awake()
        {
            TransformCache = transform;
            desiredPosition = TransformCache.position;
            desiredRotation = TransformCache.rotation;
            DesiredLocalScale = InitialScale = TransformCache.localScale;
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<IManipulationStartEvent>(OnManipulationStarted);
            EventBus.Subscribe<IManipulationEndEvent>(OnManipulationEnded);
            EventBus.Subscribe<IManipulationStopLerpingEvent>(OnLerpingStopped);
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<IManipulationStartEvent>(OnManipulationStarted);
            EventBus.Unsubscribe<IManipulationEndEvent>(OnManipulationEnded);
            EventBus.Unsubscribe<IManipulationStopLerpingEvent>(OnLerpingStopped);
        }

        protected virtual IEnumerator Lerp()
        {
            do
            {
                if (desiredPosition != null)
                    TransformCache.position = Vector3.Lerp(TransformCache.position, desiredPosition.Value, Time.deltaTime * Smoothness);
                
                if (desiredRotation != null)
                    TransformCache.rotation = Quaternion.Lerp(TransformCache.rotation, desiredRotation.Value,Time.deltaTime * Smoothness);
                
                if (DesiredLocalScale != null)
                    TransformCache.localScale = Vector3.Lerp(TransformCache.localScale, DesiredLocalScale.Value, Time.deltaTime * Smoothness);

                yield return null;
            } while (
                (desiredPosition != null && Vector3.Distance(TransformCache.position, desiredPosition.Value) > 0.01f) ||
                (desiredRotation != null && Quaternion.Angle(TransformCache.rotation, desiredRotation.Value) > 0.01f) ||
                (DesiredLocalScale != null && (DesiredLocalScale.Value - TransformCache.localScale).sqrMagnitude > 0.01f));

            lerpCoroutine = null;
        }
        
        protected virtual void OnManipulationStarted(IManipulationStartEvent eventData)
        {
            var selectable = eventData.Selectable;
            
            // Quit if the target is not this one.
            switch (eventData.ManipulationMode)
            {
                case ManipulationMode.Self:
                    var localSelectable = GetComponent<ISpatialSelectable>();
                    if (localSelectable == null || selectable != localSelectable)
                        return;
                    break;
                
                case ManipulationMode.Master:
                    if (selectable.ManipulationTarget is not ManipulationTargetBehavior behavior || behavior != this)
                        return;
                    break;
            }

            // There is chances that the lerp is still going on before the manipulation started again.
            // Therefore we need to stop the lerp here.
            desiredPosition = TransformCache.position; 
            desiredRotation = TransformCache.rotation;
            DesiredLocalScale = TransformCache.localScale;
            
            EventBus.Subscribe<IManipulationUpdateEvent>(OnManipulationUpdated);
            StopMoving();
        }

        protected virtual void OnManipulationEnded(IManipulationEndEvent @event)
        {
            EventBus.Unsubscribe<IManipulationUpdateEvent>(OnManipulationUpdated);
            StopMoving();
        }

        protected virtual void OnManipulationUpdated(IManipulationUpdateEvent eventData)
        {
            desiredPosition = eventData.DesiredPosition;

            if (eventData.RotationDelta.HasValue)
                desiredRotation = eventData.RotationDelta * desiredRotation;
            else
                desiredRotation = eventData.DesiredRotation;
            
            DesiredLocalScale = eventData.DesiredLocalScale;
            lerpCoroutine ??= StartCoroutine(Lerp());
        }

        protected virtual void StopMoving()
        {
            if (lerpCoroutine != null)
                StopCoroutine(lerpCoroutine);

            lerpCoroutine = null;
        }

        private void OnLerpingStopped(IManipulationStopLerpingEvent eventData)
        {
            StopMoving();
        }
    }
}