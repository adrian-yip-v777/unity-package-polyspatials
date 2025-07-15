using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VContainer;
using vz777.Events;
using vz777.PolySpatials.Events;

namespace vz777.PolySpatials.Demos.BasicDemo
{
	public class ScaleOnSelection : MonoBehaviour
	{
		[Inject]
		private EventBus _eventBus;
		private Transform _lastSelection;

		private void OnEnable()
		{
			_eventBus.Subscribe<SpatialPointerDetectEvent>(OnDetected);
		}

		private void OnDisable()
		{
			_eventBus.Unsubscribe<SpatialPointerDetectEvent>(OnDetected);
		}

		private void OnDetected(SpatialPointerDetectEvent eventData)
		{
			foreach (var pointer in eventData.SpatialPointers)
			{
				if (pointer.phase != SpatialPointerPhase.Ended)
				{
					continue;
				}
				
				if (_lastSelection)
					_lastSelection.localScale = Vector3.one;

				_lastSelection = pointer.targetObject.transform;
				_lastSelection.localScale = Vector3.one * 2f;
				
				// Just handle the first end-phase pointer
				break;
			}
		}
	}
}