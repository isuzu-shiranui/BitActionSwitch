#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Linq;
using BitActionSwitch.Editor.Layout;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEngine;

namespace BitActionSwitch.Editor.Views
{
    public class BitActionSwitchItemDrawer
    {
        private readonly GameObjectRegisterUi gameObjectRegisterUi;
        private readonly CustomAnimRegisterUi customAnimRegisterUi;
        private readonly GameObject targetAvatar;
        private readonly BitActionSwitchGroup bitActionSwitchGroup;
        private readonly BitActionSwitchItem bitActionSwitchItem;

        public BitActionSwitchItemDrawer(GameObject targetAvatar, BitActionSwitchGroup bitActionSwitchGroup, BitActionSwitchItem bitActionSwitchItem)
        {
            this.targetAvatar = targetAvatar;
            this.bitActionSwitchGroup = bitActionSwitchGroup;
            this.bitActionSwitchItem = bitActionSwitchItem;
            this.gameObjectRegisterUi = new GameObjectRegisterUi(bitActionSwitchItem);
            this.customAnimRegisterUi = new CustomAnimRegisterUi(bitActionSwitchItem);
        }

        public void OnGUI(Rect position)
        {
            var backgroundRect = new Rect(position){height = 20};
            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;
            labelRect.y += 2;
            labelRect.height = 16f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;
            
            var indicatorRect = backgroundRect;
            indicatorRect.x = position.width + 17f;
            indicatorRect.y += 3f;
            indicatorRect.width = 13f;
            indicatorRect.height = 13f;

            var boxRect = position;
            boxRect.height -= 12f;
            GUI.Box(boxRect, "");

            // Background
            EditorGUI.DrawRect(backgroundRect, EditorCustomGUI.HeaderBackgroundColor);
            
            // EditorGUI.DrawRect(indicatorRect, this.bitActionSwitchItem.isActiveDefault ? new Color(0.0f, 1f, 1f, 0.2f) :  new Color(1, 0.5f, 0.5f, 0.2f));

            // Title
            EditorCustomGUI.TextField(labelRect, "", this.bitActionSwitchItem.name, 
                x => this.bitActionSwitchItem.name = x,
                () =>
                {
                    var flag1 = !string.IsNullOrEmpty(this.bitActionSwitchItem.name);
                    var flag2 = this.bitActionSwitchGroup.bitActionSwitchItems
                        .GroupBy(x => x.name.ToLower())
                        .Count(x => x.Count() > 1) == 0;
                    return flag1 && flag2;
                });

            // foldout
            this.bitActionSwitchItem.fold = GUI.Toggle(foldoutRect, this.bitActionSwitchItem.fold, GUIContent.none, EditorStyles.foldout);
            
            if(!this.bitActionSwitchItem.fold) return;
            
            var cachedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 125;

            var iconRect = position;
            iconRect.width = 48;
            iconRect.height = 48;
            iconRect.x += 3;
            iconRect.y += 22;

            var enumRect = position;
            enumRect.x += iconRect.width + 8;
            enumRect.width -= iconRect.width + 12;
            enumRect.height = EditorGUIUtility.singleLineHeight + 2;
            enumRect.y = iconRect.y;
            
            var objectRect = enumRect;
            objectRect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorCustomGUI.ObjectField(iconRect, "", this.bitActionSwitchItem.icon, false, false,
                x => { this.bitActionSwitchItem.icon = x; });

            EditorCustomGUI.EnumPopup<BitActionSwitchItem.RegisterType>(enumRect, "Register Type",
                this.bitActionSwitchItem.registerType, x => this.bitActionSwitchItem.registerType = x);
            

            if (this.bitActionSwitchItem.registerType == BitActionSwitchItem.RegisterType.GameObject)
            {
                this.gameObjectRegisterUi.OnGUI(objectRect, this.targetAvatar);
            }
            else
            {
                this.customAnimRegisterUi.OnGUI(objectRect);
            }

            EditorGUIUtility.labelWidth = cachedLabelWidth;
        }

        public float GetElementHeight()
        {
            return this.bitActionSwitchItem.fold
                ? 36 + (this.bitActionSwitchItem.registerType == BitActionSwitchItem.RegisterType.GameObject
                    ? this.gameObjectRegisterUi.GetElementHeight()
                    : this.customAnimRegisterUi.GetElementHeight())
                : 24f;
        }
    }
}
#endif