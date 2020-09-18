#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Linq;
using BitActionSwitch.Scripts;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Editor.Models.VRCObject
{
    public class ExpressionMenu
    {
        private readonly VRCExpressionsMenu expressionsMenu;

        public ExpressionMenu(VRCExpressionsMenu expressionsMenu)
        {
            this.expressionsMenu = expressionsMenu;
        }

        public bool AddExpressionMenuControl(BitActionSwitchItem bitActionSwitchItem, int objectNum)
        {
            if (this.expressionsMenu.controls.Count >= 8) return false;
            this.expressionsMenu.controls.Add(new VRCExpressionsMenu.Control
            {
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                icon = bitActionSwitchItem.icon,
                name = bitActionSwitchItem.name,
                style = VRCExpressionsMenu.Control.Style.Style1,
                parameter = new VRCExpressionsMenu.Control.Parameter
                {
                    name = ActionSwitchParameters.ObjectNumParameterName
                },
                value = objectNum
            });
            EditorUtility.SetDirty(this.expressionsMenu);
            AssetDatabase.SaveAssets();
            return true;
        }

        public void RemoveExistExpressionMenuControls()
        {
            var @where = this.expressionsMenu.controls.Where(x => x.parameter != null && x.parameter.name.StartsWith(ActionSwitchParameters.PREFIX)).ToList();
            foreach (var control in @where)
            {
                this.expressionsMenu.controls.Remove(control);
            }
            EditorUtility.SetDirty(this.expressionsMenu);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif