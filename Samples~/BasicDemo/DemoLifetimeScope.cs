using VContainer;
using VContainer.Unity;
using vz777.Events;

namespace vz777.PolySpatials.Demos.BasicDemo
{
	public class DemoLifetimeScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register<EventBus>(Lifetime.Singleton);
			builder.RegisterEntryPoint<SpatialPointerHandler>();
		}
	}
}