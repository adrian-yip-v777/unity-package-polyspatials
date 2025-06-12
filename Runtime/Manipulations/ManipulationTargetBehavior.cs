using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using vz777.Events;
using vz777.Foundations;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// Responsible for being smoothly manipulated on transform by spatial selection.
    /// </summary>
    public class ManipulationTargetBehavior : MonoBehaviour, IManipulateTarget
    {
        public float Smoothness = 5f;
        protected Coroutine LerpCoroutine;
        protected event UnityAction LerpEnd;
        
        public GameObject GameObject => gameObject;
        public Transform Transform => TransformCache;
        public TrsData? DesiredTransform { get; set; }
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
            DesiredTransform = new TrsData(TransformCache);
            InitialScale = TransformCache.localScale;
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<IManipulationStartEvent>(OnManipulationStarted);
            EventBus.Subscribe<IManipulationEndEvent>(OnManipulationEnded);
            EventBus.Subscribe<IManipulationStopLerpingEvent>(OnLerpStopped);
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<IManipulationStartEvent>(OnManipulationStarted);
            EventBus.Unsubscribe<IManipulationEndEvent>(OnManipulationEnded);
            EventBus.Unsubscribe<IManipulationStopLerpingEvent>(OnLerpStopped);
        }

        protected virtual IEnumerator Lerp()
        {
            var distanceCheck = 0f;
            var rotationCheck = 0f;
            var scaleCheck = 0f;
            do
            {
                if (DesiredTransform == null) continue;
                TransformCache.position = Vector3.Lerp(TransformCache.position, DesiredTransform.Value.Position, Time.deltaTime * Smoothness);
                TransformCache.rotation = Quaternion.Lerp(TransformCache.rotation, DesiredTransform.Value.Rotation,Time.deltaTime * Smoothness);
                TransformCache.localScale = Vector3.Lerp(TransformCache.localScale, DesiredTransform.Value.LocalScale, Time.deltaTime * Smoothness);

                distanceCheck = Vector3.Distance(TransformCache.position, DesiredTransform.Value.Position);
                rotationCheck = Mathf.Abs(Quaternion.Dot(TransformCache.rotation, DesiredTransform.Value.Rotation));
                scaleCheck = Vector3.Distance(DesiredTransform.Value.LocalScale, TransformCache.localScale); 
                
                yield return null;
            } while (
                DesiredTransform != null && (distanceCheck > .1f || 
                                             !Mathf.Approximately(rotationCheck, 1) || 
                                             scaleCheck > .1f ));

            LerpCoroutine = null;
            LerpEnd?.Invoke();
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
            DesiredTransform = new TrsData(TransformCache);
            EventBus.Subscribe<IManipulationUpdateEvent>(OnManipulationUpdated);
            StopMoving();
        }

        protected virtual void OnManipulationEnded(IManipulationEndEvent @event)
        {
            if (@event.Selectable.ManipulationTarget is not ManipulationTargetBehavior behavior || behavior != this)
                return;
            
            EventBus.Unsubscribe<IManipulationUpdateEvent>(OnManipulationUpdated);
        }

        protected virtual void OnManipulationUpdated(IManipulationUpdateEvent eventData)
        {
            var desiredPosition = eventData.DesiredPosition ?? (DesiredTransform?.Position ?? TransformCache.position);
            var desiredRotation = DesiredTransform?.Rotation ?? TransformCache.rotation;
            
            if (eventData.RotationDelta.HasValue)
                desiredRotation = eventData.RotationDelta.Value * desiredRotation;
            else
                desiredRotation = eventData.DesiredRotation ?? TransformCache.rotation;
            
            var desiredLocalScale = eventData.DesiredLocalScale ?? (DesiredTransform?.LocalScale ?? TransformCache.localScale);
            
            DesiredTransform = new TrsData(desiredPosition, desiredRotation, desiredLocalScale);
            LerpCoroutine ??= StartCoroutine(Lerp());
        }

        protected virtual void StopMoving()
        {
            if (LerpCoroutine != null)
                StopCoroutine(LerpCoroutine);

            LerpCoroutine = null;
            DesiredTransform = new TrsData(TransformCache);
        }

        private void OnLerpStopped(IManipulationStopLerpingEvent eventData)
        {
            StopMoving();
        }
    }
}