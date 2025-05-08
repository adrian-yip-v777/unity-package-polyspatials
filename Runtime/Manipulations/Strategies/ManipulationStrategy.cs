using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VContainer;

namespace vz777.PolySpatials.Manipulations.Strategies
{
    public abstract class ManipulationStrategy : ScriptableObject
    {
        [SerializeField]
        protected ManipulationMode manipulationMode;
        
        /// <summary>
        /// Responsible for instantiate the manipulate strategies with runtime injection.
        /// </summary>
        public class Factory
        {
            private readonly IObjectResolver _resolver;

            public Factory(IObjectResolver resolver)
            {
                _resolver = resolver;
            }
            
            public T Create<T>(T scriptableObject) where T: ManipulationStrategy
            {
                var instance = Instantiate(scriptableObject);
                _resolver.Inject(instance);
                return instance;
            }
        }
        
        /// <summary>
        /// Try to execute the strategy, return false when it doesn't meet the criteria.
        /// </summary>
        public abstract bool TryRun(SpatialPointerState[] spatialPointers, ISpatialSelectable selectable);
        
        /// <summary>
        /// Reset the cached value for a strategy.
        /// </summary>
        public abstract void Reset();
    }
}