#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Linq;
using BitActionSwitch.Editor.Utility;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Editor.Models.VRCObject
{
    public class ExpressionParameter
    {
        private readonly VRCExpressionParameters expressionParameters;

        public ExpressionParameter(VRCExpressionParameters expressionParameters)
        {
            this.expressionParameters = expressionParameters;
        }

        public bool AddExpressionParameters(string bitStatusParameter)
        {
            if (this.expressionParameters.FindParameter(ActionSwitchParameters.ObjectNumParameterName) == null)
            {
                if (!this.expressionParameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = ActionSwitchParameters.ObjectNumParameterName,
                    valueType = VRCExpressionParameters.ValueType.Int
                })) return true;
            }

            if (!this.expressionParameters.Add(new VRCExpressionParameters.Parameter
            {
                name = bitStatusParameter,
                valueType = VRCExpressionParameters.ValueType.Int
            }))
            {
                this.RemoveExistExpressionParameters();
                return true;
            }
            return false;
        }

        public void RemoveExistExpressionParameters()
        {
            for (var index = 0; index < this.expressionParameters.parameters.Length; index++)
            {
                if(!this.expressionParameters.parameters[index].name.StartsWith(ActionSwitchParameters.PREFIX)) continue;
                this.expressionParameters.parameters[index] = new VRCExpressionParameters.Parameter
                {
                    name = "", valueType = VRCExpressionParameters.ValueType.Int
                };
            }
        }
    }
}
#endif