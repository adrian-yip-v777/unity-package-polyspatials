using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;
using vz777.PolySpatials.Manipulations.Strategies;
using vz777.VContainer;

namespace vz777.PolySpatials.Manipulations
{
    public class ManipulationInstaller : MonoInstaller
    {
        [SerializeField, FormerlySerializedAs("_manipulationStrategies")]
        private ManipulationStrategy[] manipulationStrategies;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(manipulationStrategies);
            builder.RegisterEntryPoint<SpatialPointerHandler>();
            builder.RegisterEntryPoint<ManipulationHandler>();
            builder.Register<ManipulationStrategy.Factory>(Lifetime.Singleton);
        }
    }
}