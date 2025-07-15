using vz777.Events;
using TMPro;
using UnityEngine;
using VContainer;
using vz777.PolySpatials.Manipulations.Events;

namespace vz777.PolySpatials.Demos.ManipulationDemo.UI
{
    public class ScaleRenderer : MonoBehaviour
    {
        [Inject]
        private EventBus _eventBus;
        
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private Transform infoCanvas;

        [SerializeField]
        private CanvasGroup canvasGroup;
        
        [SerializeField]
        private TMP_Text targetNameText;
        
        [SerializeField]
        private TMP_Text distanceText;
        
        [SerializeField]
        private TMP_Text scaleText;

        private void OnEnable()
        {
            _eventBus.Subscribe<ScaleAndRotateStartEvent> (OnScaleAndRotationStarted);
            _eventBus.Subscribe<ScaleAndRotateUpdateEvent> (OnScaleAndRotationUpdated);
            _eventBus.Subscribe<ScaleAndRotateEndEvent>(OnScaleAndRotationEnded);
        }
        
        private void OnDisable()
        {
            _eventBus.Unsubscribe<ScaleAndRotateStartEvent> (OnScaleAndRotationStarted);
            _eventBus.Unsubscribe<ScaleAndRotateUpdateEvent> (OnScaleAndRotationUpdated);
            _eventBus.Unsubscribe<ScaleAndRotateEndEvent>(OnScaleAndRotationEnded);
        }

        private void OnScaleAndRotationStarted(ScaleAndRotateStartEvent eventData)
        {
            canvasGroup.alpha = 1;
            lineRenderer.enabled = true;
        }

        private void OnScaleAndRotationUpdated(ScaleAndRotateUpdateEvent eventData)
        {
            var primaryPosition = eventData.PrimaryPointer.inputDevicePosition;
            var secondaryPosition = eventData.SecondaryPointer.inputDevicePosition;
            var secondPoint = Vector3.Lerp(primaryPosition, secondaryPosition, 0.3f);
            var thirdPoint = Vector3.Lerp(primaryPosition, secondaryPosition, 0.7f);
            
            lineRenderer.SetPositions(new[] {primaryPosition, secondPoint, thirdPoint, secondaryPosition});
            
            infoCanvas.position = (primaryPosition + secondaryPosition) * 0.5f;
            targetNameText.text = eventData.Selectable.ManipulationTarget.Name;
            distanceText.text = eventData.CurrentDistance.ToString("F");
            scaleText.text = $"{eventData.CurrentScale:P0}";
        }

        private void OnScaleAndRotationEnded(ScaleAndRotateEndEvent eventData)
        {
            canvasGroup.alpha = 0;
            lineRenderer.enabled = false;
        }

    }
}