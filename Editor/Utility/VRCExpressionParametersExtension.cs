#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using VRC.SDK3.Avatars.ScriptableObjects;
using System.Collections.Generic;

namespace BitActionSwitch.Editor.Utility
{
    public static class VRCExpressionParametersExtension
    {
        public static bool Add(this VRCExpressionParameters parameters, VRCExpressionParameters.Parameter parameter)
        {

            var newParameters = new List<VRCExpressionParameters.Parameter>(parameters.parameters);
            newParameters.Add(parameter);
            parameters.parameters = newParameters.ToArray();
            return parameters.CalcTotalCost() <= VRCExpressionParameters.MAX_PARAMETER_COST;
        }
    }
}
#endif