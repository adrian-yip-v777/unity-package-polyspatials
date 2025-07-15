using UnityEngine;
using VContainer;
using vz777.PolySpatials.Manipulations;

namespace vz777.PolySpatials.Demos.ManipulationDemo
{
	/// <summary>
	/// Responsible for applying a context OnEnable.
	/// </summary>
	public class Bootstrap : MonoBehaviour
	{
		[SerializeField]
		private ManipulationContext context;
		
		[Inject]
		private ManipulationHandler _manipulationHandler;

		private void OnEnable()
		{
			_manipulationHandler.ApplyContext(context);
		}

		private void OnDisable()
		{
			_manipulationHandler.ClearContext();
		}
	}
}