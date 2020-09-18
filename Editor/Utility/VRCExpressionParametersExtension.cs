#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Editor.Utility
{
    public static class VRCExpressionParametersExtension
    {
        public static bool Add(this VRCExpressionParameters parameters, VRCExpressionParameters.Parameter parameter)
        {
            var emptyFirstIndex = 0;
            for (var i = 0; i < parameters.parameters.Length; i++)
            {
                if (!string.IsNullOrEmpty(parameters.parameters[i].name)) continue;
                emptyFirstIndex = i;
                break;
            }
            
            if(emptyFirstIndex == parameters.parameters.Length) return false;

            parameters.parameters[emptyFirstIndex] = parameter;

            return true;
        }
    }
}
#endif