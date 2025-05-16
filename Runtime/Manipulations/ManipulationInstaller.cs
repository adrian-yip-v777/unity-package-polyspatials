using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using vz777.PolySpatials.Manipulations.Strategies;
using vz777.VContainer;

namespace vz777.PolySpatials.Manipulations
{
    public class ManipulationInstaller : MonoInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SpatialPointerHandler>();
            builder.RegisterEntryPoint<ManipulationHandler>().AsSelf();
            builder.Register<ManipulationStrategy.Factory>(Lifetime.Singleton);
        }
    }
}