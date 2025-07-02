using _Project.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using VContainer;
using vz777.Events;
using vz777.Foundations;
using vz777.PolySpatials.Events;

namespace vz777.PolySpatials
{
    /// <summary>
    /// A kind of overkilled button with mesh for spatial interaction.
    /// The logic of show/hide presentation should be separated in the future. 
    /// </summary>
    public class SpatialButton : MonoBehaviour, IShowBehavior, IInteractable
    {
        public event UnityAction Clicked, Showed, Hid;
        
        [SerializeField]
        private new Collider collider;
        private bool _interactable = true;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            EventBus = eventBus;
        }

        protected EventBus EventBus { get; private set; }
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                collider.enabled = value;
            }
        }
        
        protected virtual void OnEnable()
        {
            EventBus.Subscribe<SpatialPointerDetectEvent>(OnPointerDetected);
            Show();
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<SpatialPointerDetectEvent>(OnPointerDetected);
        }

        private void OnPointerDetected(SpatialPointerDetectEvent eventData)
        {
            if (!Interactable) return;
            
            foreach (var pointer in eventData.SpatialPointers)
            {
                // Only works with ended phase pointer
                // to prevent multiple triggers on this button.
                if (pointer.phase is not SpatialPointerPhase.Ended || 
                    pointer.targetObject != collider.gameObject) continue;
                
                OnClicked(pointer);
                break;
            }
        }

        protected virtual void OnClicked(SpatialPointerState pointer) 
        { 
            Clicked?.Invoke();
        }

        public void Show()
        {
            Interactable = true;
            Showed?.Invoke();
        }

        public void Hide()
        {
            Interactable = false;
            Hid?.Invoke();
        }
    }
}