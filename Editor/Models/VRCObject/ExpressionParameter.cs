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
            if (!this.expressionParameters.Add(new VRCExpressionParameters.Parameter
            {
                name = bitStatusParameter,
                valueType = VRCExpressionParameters.ValueType.Bool,
                defaultValue = 0,
                saved = true,
            }))
            {
                this.RemoveExistExpressionParameters();
                return true;
            }
            return false;
        }

        public void RemoveExistExpressionParameters()
        {
            this.expressionParameters.parameters =
                this.expressionParameters.parameters.Where(parameter => !parameter.name.StartsWith(ActionSwitchParameters.PREFIX)).ToArray();
        }
    }
}
#endif