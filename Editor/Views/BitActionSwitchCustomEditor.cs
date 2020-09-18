#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using BitActionSwitch.Editor.ViewModels;
using BitActionSwitch.Editor.Layout;
using BitActionSwitch.Editor.Models;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BitActionSwitch.Editor.Views
{
    [CustomEditor(typeof(Scripts.BitActionSwitch))]
    public class BitActionSwitchCustomEditor : UnityEditor.Editor
    {
        private BitActionSwitchWindowViewModel viewModel;
        private Scripts.BitActionSwitch bitActionSwitch;
        private ReorderableList reorderableList;
        private List<BitActionSwitchGroupDrawer> bitActionSwitchGroupDrawers = new List<BitActionSwitchGroupDrawer>();

        private void OnEnable()
        {
            this.viewModel = new BitActionSwitchWindowViewModel();

            this.bitActionSwitch = (Scripts.BitActionSwitch) this.target;
            this.viewModel.BitActionSwitch = this.bitActionSwitch;
            
            this.reorderableList = new ReorderableList(this.bitActionSwitch.bitActionSwitchGroups, typeof(BitActionSwitchGroup), false, true, true, true)
            {
                elementHeight = 54,
                drawElementCallback = this.DrawElement,
                drawHeaderCallback = this.DrawHeader,
            };
            this.bitActionSwitchGroupDrawers = this.bitActionSwitch.bitActionSwitchGroups.Select(x =>
                new BitActionSwitchGroupDrawer(this.viewModel.AvatarDescriptor == null ? null : this.viewModel.AvatarDescriptor.gameObject, this.bitActionSwitch, x)).ToList();
            
            this.reorderableList.onAddCallback += list => { this.Add(); };

            this.reorderableList.onRemoveCallback += list =>
            {
                this.bitActionSwitch.bitActionSwitchGroups.RemoveAt(list.index);
                this.bitActionSwitchGroupDrawers.RemoveAt(list.index);
                if (list.index >= list.list.Count - 1) list.index = list.list.Count - 1;
            };

            this.reorderableList.elementHeightCallback +=
                index => this.bitActionSwitchGroupDrawers[index].GetElementHeight();

            if (this.viewModel.BitActionSwitch.bitActionSwitchGroups.Count == 0)
            {
                this.Add();
            }
        }

        private void Add()
        {
            var bitActionSwitchGroup = new BitActionSwitchGroup
                {variableName = $"bas_BitStatus{(this.bitActionSwitch.bitActionSwitchGroups.Count + 1).ToString()}"};
            this.bitActionSwitch.bitActionSwitchGroups.Add(bitActionSwitchGroup);
            var gameObject = this.viewModel.AvatarDescriptor == null ? null : this.viewModel.AvatarDescriptor.gameObject;
            var drawer =
                new BitActionSwitchGroupDrawer(gameObject, this.bitActionSwitch, bitActionSwitchGroup);
            drawer.AddItem(gameObject, bitActionSwitchGroup);
            this.bitActionSwitchGroupDrawers.Add(drawer);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "BitActionSwitch Groups");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            this.bitActionSwitchGroupDrawers[index].OnGUI(rect, index);
        }

        public override void OnInspectorGUI()
        {
            EditorCustomGUILayout.ObjectField("Avatar", this.viewModel.AvatarDescriptor, true, true,
                x => { this.viewModel.AvatarDescriptor = x; }, () => this.viewModel.AvatarDescriptor != null);

            using (new EditorGUI.DisabledScope(this.viewModel.AvatarDescriptor == null))
            {
                EditorGUI.indentLevel++;

                EditorCustomGUILayout.ObjectField("FX Controller", this.viewModel.AnimatorController, false, true,
                    x => { this.viewModel.AnimatorController = x; },
                    () => this.viewModel.AnimatorController != null);

                EditorCustomGUILayout.ObjectField("Parameters", this.viewModel.ExpressionParameters, false, true,
                    x => { this.viewModel.ExpressionParameters = x; },
                    () => this.viewModel.ExpressionParameters != null);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorCustomGUILayout.FolderPathField("Working Folder", this.viewModel.WorkingFolder,
                "Save Anims Path", x => { this.viewModel.WorkingFolder = x; },
                () => !string.IsNullOrEmpty(this.viewModel.WorkingFolder) &&
                      this.viewModel.WorkingFolder.StartsWith("Assets"));
            
            this.reorderableList.DoLayoutList();
            this.reorderableList.displayRemove = this.bitActionSwitch.bitActionSwitchGroups.Count > 1;
            this.reorderableList.displayAdd = this.viewModel.ExpressionParameters != null &&
                                              this.viewModel.ExpressionParameters.parameters.Count(x =>
                                                  !string.IsNullOrEmpty(x.name) &&
                                                  !x.name.StartsWith(ActionSwitchParameters.PREFIX)) +
                                              this.bitActionSwitch.bitActionSwitchGroups.Count < 15;

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!this.viewModel.ApplyCommand.CanExecute()))
            {
                if (GUILayout.Button("Apply")) this.viewModel.ApplyCommand.Execute();
            }
        }
    }
}
#endif