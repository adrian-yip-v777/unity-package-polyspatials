using System.Collections.Generic;
using UnityEngine;
using vz777.PolySpatials.Manipulations.Strategies;

namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// Responsible for enabling different strategies for different scenarios. 
    /// </summary>
    [CreateAssetMenu (menuName = "vz777/Poly Spatials/Manipulations/Context")]
    public class ManipulationContext : ScriptableObject
    {
        [SerializeField]
        private ManipulationStrategy[] strategies;
        
        /// <summary>
        /// The manipulation strategies of this context to use.
        /// </summary>
        public IReadOnlyCollection<ManipulationStrategy> Strategies => strategies;
    }
}