#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using BitActionSwitch.Editor.Layout;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEngine;

namespace BitActionSwitch.Editor.Views
{
    public class CustomAnimRegisterUi
    {
        private readonly BitActionSwitchItem bitActionSwitchItem;

        public CustomAnimRegisterUi(BitActionSwitchItem bitActionSwitchItem)
        {
            this.bitActionSwitchItem = bitActionSwitchItem;
        }

        public void OnGUI(Rect position)
        {
            var staticActive = position;
            staticActive.height = EditorGUIUtility.singleLineHeight;
            
            var staticInactive = position;
            staticInactive.y += staticActive.height + 2;
            staticInactive.height = EditorGUIUtility.singleLineHeight;
            
            var active = staticInactive;
            active.y += staticInactive.height + 2;
            active.height = EditorGUIUtility.singleLineHeight;
            
            var inactive = active;
            inactive.y += active.height + 2;
            inactive.height = EditorGUIUtility.singleLineHeight;

            EditorCustomGUI.ObjectField(staticActive, "Static Default", this.bitActionSwitchItem.staticDefaultClip,
                false, true, x => this.bitActionSwitchItem.staticDefaultClip = x,
                () => this.bitActionSwitchItem.staticDefaultClip != null);
            
            EditorCustomGUI.ObjectField(staticInactive, "Static NonDefault",
                this.bitActionSwitchItem.staticNonDefaultClip, false, true,
                x => this.bitActionSwitchItem.staticNonDefaultClip = x,
                () => this.bitActionSwitchItem.staticNonDefaultClip != null);
            
            EditorCustomGUI.ObjectField(active, "Default", this.bitActionSwitchItem.defaultClip, true, false,
                x => this.bitActionSwitchItem.defaultClip = x);
            
            EditorCustomGUI.ObjectField(inactive, "NonDefault", this.bitActionSwitchItem.nonDefaultClip, true,
                false, x => this.bitActionSwitchItem.nonDefaultClip = x);
        }

        public float GetElementHeight()
        {
            return EditorGUIUtility.singleLineHeight * 4 + 24;
        }
    }
}
#endif