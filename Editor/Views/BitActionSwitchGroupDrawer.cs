#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using BitActionSwitch.Editor.Layout;
using BitActionSwitch.Editor.Models;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BitActionSwitch.Editor.Views
{
    public class BitActionSwitchGroupDrawer
    {
        private readonly Scripts.BitActionSwitch bitActionSwitch;
        private readonly BitActionSwitchGroup bitActionSwitchGroup;
        private readonly ReorderableList reorderableList;
        private readonly List<BitActionSwitchItemDrawer> bitActionSwitchItemDrawers;
        private GUIStyle titleStyle;
        
        public BitActionSwitchGroupDrawer(GameObject targetAvatar, Scripts.BitActionSwitch bitActionSwitch,
            BitActionSwitchGroup bitActionSwitchGroup)
        {
            this.bitActionSwitch = bitActionSwitch;
            this.bitActionSwitchGroup = bitActionSwitchGroup;
            this.reorderableList =
                new ReorderableList(bitActionSwitchGroup.bitActionSwitchItems, typeof(BitActionSwitchItem), true, true,
                    true, true)
                {
                    elementHeight = 54,
                    drawElementCallback = this.DrawElement,
                    drawHeaderCallback = this.DrawHeader,
                };

            this.bitActionSwitchItemDrawers = bitActionSwitchGroup.bitActionSwitchItems
                .Select(x => new BitActionSwitchItemDrawer(targetAvatar, this.bitActionSwitchGroup, x)).ToList();

            this.reorderableList.onAddCallback += list => { this.AddItem(targetAvatar, bitActionSwitchGroup); };

            this.reorderableList.onRemoveCallback += list =>
            {
                bitActionSwitchGroup.bitActionSwitchItems.RemoveAt(list.index);
                this.bitActionSwitchItemDrawers.RemoveAt(list.index);
                if (list.index >= list.list.Count - 1) list.index = list.list.Count - 1;
            };
            
            this.reorderableList.elementHeightCallback += index => this.bitActionSwitchItemDrawers[index].GetElementHeight();

            this.reorderableList.onReorderCallbackWithDetails += (list, index, newIndex) =>
            {
                this.bitActionSwitchItemDrawers.ShiftElement(index, newIndex);
            };
        }
        
        public static void Reorder(IList<BitActionSwitchItem> self, int oldIndex, int newIndex)
        {
            var element = self[oldIndex];
            Debug.Log(element.name);

            for (var i = 0; i < self.Count - 1; ++i)
            {
                if (i >= oldIndex) self[i] = self[i + 1];
            }
            
            for (var i = self.Count - 1; i > 0; --i)
            {
                if (i > newIndex) self[i] = self[i - 1];
            }
            
            self[newIndex] = element;
        }

        public void AddItem(GameObject targetAvatar, BitActionSwitchGroup bitActionSwitchGroup)
        {
            var bitActionSwitchItem = new BitActionSwitchItem
            {
                name = ObjectNames.GetUniqueName(bitActionSwitchGroup.bitActionSwitchItems.Select(x => x.name).ToArray(),
                    "Object")
            };
            bitActionSwitchGroup.bitActionSwitchItems.Add(bitActionSwitchItem);
            this.bitActionSwitchItemDrawers.Add(new BitActionSwitchItemDrawer(targetAvatar, this.bitActionSwitchGroup,
                bitActionSwitchItem));
        }

        private void DrawHeader(Rect rect)
        {
            var menuCount = this.bitActionSwitchGroup.expressionsMenu == null ? 0 : this.bitActionSwitchGroup.expressionsMenu.controls.Count - this.bitActionSwitchGroup.expressionsMenu.controls.Count(x => x.parameter.name != null && x.parameter.name.StartsWith(ActionSwitchParameters.PREFIX));
            var b = menuCount > 1;
            EditorGUI.LabelField(rect, $"Group Items - {this.bitActionSwitchGroup.bitActionSwitchItems.Count.ToString()}/8, {(menuCount==0?"No":menuCount.ToString())} Menu{(menuCount > 1?"s":"")} in use other system");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            this.bitActionSwitchItemDrawers[index].OnGUI(rect);
        }
        
        public void OnGUI(Rect position, int index)
        {
            if (this.titleStyle == null)
            {
                this.titleStyle = new GUIStyle(GUI.skin.box)
                {
                    fontStyle = FontStyle.Bold, normal = {textColor = Color.white}
                };
            }

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
            
            GUI.Box(position, $"Group {index + 1}", this.titleStyle);

            // Background
            EditorGUI.DrawRect(backgroundRect, EditorCustomGUI.HeaderBackgroundColor);
            
            // EditorGUI.DrawRect(indicatorRect, this.bitActionSwitchItem.isActiveDefault ? new Color(0.0f, 1f, 1f, 0.2f) :  new Color(1, 0.5f, 0.5f, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, "");

            // foldout
            this.bitActionSwitchGroup.fold = GUI.Toggle(foldoutRect, this.bitActionSwitchGroup.fold, GUIContent.none, EditorStyles.foldout);
            
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        this.bitActionSwitchGroup.fold = !this.bitActionSwitchGroup.fold;
                    }

                    e.Use();
                }
            }
            
            if(!this.bitActionSwitchGroup.fold) return;
            
            var objectField = position;
            objectField.height = EditorGUIUtility.singleLineHeight;
            objectField.y += 22;
            
            var variableNameField = objectField;
            variableNameField.y += objectField.height + 2f;

            var reorderable = variableNameField;
            reorderable.x += 4;
            reorderable.width -= 4;
            reorderable.y += variableNameField.height + 2;

            EditorCustomGUI.ObjectField(objectField, "Menu", this.bitActionSwitchGroup.expressionsMenu, false, true,
                x => { this.bitActionSwitchGroup.expressionsMenu = x; }, () => this.bitActionSwitchGroup.expressionsMenu != null);

            EditorGUI.BeginDisabledGroup(true);
            EditorCustomGUI.TextField(variableNameField, "Variable", this.bitActionSwitchGroup.variableName,
                x => this.bitActionSwitchGroup.variableName = x, () => 
                {
                    var flag1 = !string.IsNullOrEmpty(this.bitActionSwitchGroup.variableName);
                    var flag2 = this.bitActionSwitch.bitActionSwitchGroups
                        .GroupBy(x => x.variableName.ToLower())
                        .Count(x => x.Count() > 1) == 0;
                    return flag1 && flag2;
                });
            EditorGUI.EndDisabledGroup();
            
            this.reorderableList.DoList(reorderable);

            this.reorderableList.displayAdd = this.bitActionSwitchGroup.bitActionSwitchItems.Count < 8 &&
                                              (this.bitActionSwitchGroup.expressionsMenu == null ? 0 :
                                              this.bitActionSwitchGroup.expressionsMenu.controls
                                                  .Count(x => x.parameter.name != null && !x.parameter.name.StartsWith(ActionSwitchParameters.PREFIX))) +
                                              this.bitActionSwitchGroup.bitActionSwitchItems.Count < 8;
        }

        public float GetElementHeight()
        {
            return this.bitActionSwitchGroup.fold
                ? this.reorderableList.GetHeight() + EditorGUIUtility.singleLineHeight * 3 + 16
                : 24f;
        }
    }
}
#endif