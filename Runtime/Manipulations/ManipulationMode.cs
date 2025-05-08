namespace vz777.PolySpatials.Manipulations
{
    /// <summary>
    /// We need two manipulation modes, see each enum for details.
    /// </summary>
    public enum ManipulationMode
    {
        None = 0,
            
        /// <summary>
        /// Manipulate the selection's transform
        /// </summary>
        Self,
            
        /// <summary>
        /// Manipulate the master target assigned in the inspector.
        /// </summary>
        Master,
    }
}