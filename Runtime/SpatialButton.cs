using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Events;

namespace vz777.PolySpatials
{
    /// <summary>
    /// A kind of overkilled button with mesh for spatial interaction.
    /// The logic of show/hide presentation should be separated in the future. 
    /// </summary>
    public class SpatialButton : MonoBehaviour
    {
        public event Action Clicked;

        [FormerlySerializedAs("animatedOnEnable")]
        [SerializeField]
        private bool animateOnEnable = true;
        
        [SerializeField]
        private new Collider collider;
        
        [SerializeField]
        private float animateDuration = .5f;
        
        private bool _interactable = true;
        private Dictionary<Component, Color> _colors;
        private Renderer[] _renderers;
        private CanvasGroup[] _canvasGroups;

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

        protected virtual void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers == null || _renderers.Length == 0)
                Debug.LogWarning($"Cannot find any renderers attached to this spatial button: {name}");

            _canvasGroups = GetComponentsInChildren<CanvasGroup>();
            _colors = _renderers.ToDictionary(renderer => (Component)renderer, renderer => renderer.material.color);
        }
        
        protected virtual void OnEnable()
        {
            EventBus.Subscribe<SpatialPointerDetectEvent>(OnPointerDetected);

            if (!animateOnEnable) return;
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
                    pointer.targetObject != gameObject) continue;
                
                OnClicked(pointer);
                break;
            }
        }

        protected virtual void OnClicked(SpatialPointerState pointer) 
        { 
            Clicked?.Invoke();
        }

        public void Hide(bool interactable = false, bool animated = false)
        {
            Interactable = interactable;

            foreach (var canvasGroup in _canvasGroups) 
                DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, animated ? animateDuration : 0);
            
            if (animated)
                ChangeRenderersColor(Color.clear);
            else
                ChangeRenderersColor(Color.clear, animated: false);
        }

        public void Show(bool animated = true)
        {
            foreach (var canvasGroup in _canvasGroups)
            {
                canvasGroup.alpha = 0;
                DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, animated ? animateDuration : 0);
            }
            
            if (animated)
                ChangeRenderersColor(null, onAnimationCompleted: () => Interactable = true);
            else
                ChangeRenderersColor(null, animated: false);
            
            Interactable = true;
        }

        /// <summary>
        /// Change the color of the renderers. 
        /// </summary>
        /// <param name="color">Passing null value to change color to the original color</param>
        private void ChangeRenderersColor(Color? color, bool animated = true, Action onAnimationCompleted = null)
        {
            foreach (var renderer in _renderers)
            {
                // Set the color to clear to make sure there is a transition from transparent to the original color.
                if (color is null)
                    renderer.material.color = Color.clear;
                
                renderer.material
                    .DOColor(color ?? _colors[renderer], animated ? animateDuration : 0)
                    .OnComplete(() =>
                    {
                        if (!animated) return;
                        onAnimationCompleted?.Invoke();
                    });
            }
        }
    }
}