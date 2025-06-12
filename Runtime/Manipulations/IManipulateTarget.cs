using UnityEngine;
using vz777.Foundations;

namespace vz777.PolySpatials.Manipulations
{
    public interface IManipulateTarget
    {
        string Name { get; }
        TrsData? DesiredTransform { get; set; }
        Vector3 InitialScale { get; }

        #region Unity Related
        GameObject GameObject { get; }
        Transform Transform { get; }
        #endregion
    }
}