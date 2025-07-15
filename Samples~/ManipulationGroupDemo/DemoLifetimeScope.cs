using VContainer;
using VContainer.Unity;
using vz777.Events;
using vz777.PolySpatials.Manipulations;
using vz777.PolySpatials.Manipulations.Strategies;

namespace vz777.PolySpatials.Demos.ManipulationGroupDemo
{
	public class DemoLifetimeScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register<EventBus>(Lifetime.Singleton);
			builder.RegisterEntryPoint<SpatialPointerHandler>();
			
			builder.RegisterEntryPoint<ManipulationHandler>().AsSelf();
			builder.Register<ManipulationStrategy.Factory>(Lifetime.Singleton);
		}
	}
}