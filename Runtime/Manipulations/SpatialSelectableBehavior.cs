using UnityEngine;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// Responsible for being selected by the spatial pointer and refer the correct manipulation object based on the manipulation mode.
    /// </summary>
    public class SpatialSelectableBehavior : MonoBehaviour, ISpatialSelectable
    {
        #region Inspector Fields
        [SerializeField]
        private ManipulationMode mode = ManipulationMode.Self;
        
        [SerializeField]
        private bool isSelectable = true;

        [SerializeField, Tooltip("The target to manipulate when the mode is Target.")]
        private ManipulationTargetBehavior manipulationTarget;
        #endregion

        #region public Properties
        public string Name => mode is ManipulationMode.Self ? name : ManipulationTarget.Name;
        public ManipulationMode Mode => mode;

        public bool IsSelectable
        {
            get => isSelectable;
            set
            {
                isSelectable = value;
                _collider ??= GetComponent<BoxCollider>();
                _collider.enabled = value;
            }
        }
        public IManipulateTarget ManipulationTarget => mode is ManipulationMode.Self ? _manipulationObjectBehavior : manipulationTarget;
        #endregion

        private IManipulateTarget _manipulationObjectBehavior;
        private EventBus _eventBus;
        private Collider _collider;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _manipulationObjectBehavior = GetComponent<IManipulateTarget>();
            _collider = GetComponent<BoxCollider>();
            _collider.enabled = isSelectable;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<IManipulationStartEvent>(OnManipulationStarted, Priority.Highest);
            _eventBus.Subscribe<IManipulationEndEvent>(OnManipulationEnded, Priority.Highest);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<IManipulationStartEvent>(OnManipulationStarted);
            _eventBus.Unsubscribe<IManipulationEndEvent>(OnManipulationEnded);
        }

        private void OnManipulationStarted(IManipulationStartEvent eventData)
        {
            if (!IsSelectable || eventData.Selectable is not SpatialSelectableBehavior behavior || behavior != this) return;
            
            // If the manipulation mode is master, switch to master mode.
            mode = eventData.ManipulationMode;
        }

        private void OnManipulationEnded(IManipulationEndEvent eventData)
        {
            if (!IsSelectable || eventData.Selectable is not SpatialSelectableBehavior behavior || behavior != this) return;
            mode = ManipulationMode.Self;
        }
    }
}