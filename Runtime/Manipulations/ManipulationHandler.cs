using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VContainer.Unity;
using vz777.Events;
using vz777.Foundations;
using vz777.PolySpatials.Events;
using vz777.PolySpatials.Manipulations.Strategies;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// An orchestrator which is responsible to receive spatial pointers, get the selectable from them, then execute manipulation strategies to manipulate the target's transform.
    /// </summary>
    public class ManipulationHandler : DisposableBase, IStartable
    {
        private readonly EventBus _eventBus;
        private readonly ManipulationStrategy.Factory _factory;
        private ManipulationStrategy[] _strategies;

        public ManipulationHandler(ManipulationStrategy.Factory factory, EventBus eventBus)
        {
            _eventBus = eventBus;
            _factory = factory;
        }
        
        public void Start()
        {
            _eventBus.Subscribe<SpatialPointerDetectEvent>(OnSpatialPointerDetected);
        }

        protected override void DisposeManagedResources()
        {
            _eventBus.Unsubscribe<SpatialPointerDetectEvent>(OnSpatialPointerDetected);
        }

        /// <summary>
        /// Apply a context for different combo of manipulation strategies.
        /// </summary>
        public void ApplyContext(ManipulationContext context)
        {
            _strategies = context.Strategies.Select(_factory.Create).ToArray();
        }

        public void ClearContext()
        {
            if (_strategies == null) return;
            
            // Destroy the instance of strategies to prevent memory leak.
            foreach (var strategy in _strategies)
                Object.Destroy(strategy);

            _strategies = null;
        }
        
        private void OnSpatialPointerDetected(SpatialPointerDetectEvent eventData)
        {
            if (_strategies == null || _strategies.Length == 0)
                return;
            
            var spatialPointers = eventData.SpatialPointers.ToArray();
            
            // Reset all strategies if there is no valid selection
            if (!TryGetSelectable (spatialPointers, out var selectable) || !selectable.IsSelectable)
            {
                foreach (var strategy in _strategies)
                    strategy.Reset(); 
                
                return;
            }
            
            // Find and execute any valid strategy and reset the others.
            var isAnyStrategyExecuted = false;
            foreach (var strategy in _strategies)
            {
                // Reset the strategy if there is a valid being executed.
                if (isAnyStrategyExecuted)
                {
                    strategy.Reset();
                    continue;
                }
                
                // Try to run(execute) a strategy, if it's not valid, reset it instead.
                if (!strategy.TryRun(spatialPointers.ToArray(), selectable))
                {
                    strategy.Reset();
                    continue;
                }
                
                isAnyStrategyExecuted = true;
            }
        }
        
        /// <summary>
        /// Try to get the selectable from the spatial pointers.
        /// <returns>false when there is no selectable being hit.</returns>
        /// </summary>
        private bool TryGetSelectable(SpatialPointerState[] spatialPointers, out ISpatialSelectable selectable)
        {
            selectable = null;
            
            foreach (var spatialPointer in spatialPointers)
            {
                // Make sure the pointer is manipulating a manipulatable object.
                if (spatialPointer.targetObject.TryGetComponent(out selectable))
                    return true;
            }

            return false;
        }
    }
}