#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Linq;
using BitActionSwitch.Editor.Models;
using BitActionSwitch.Editor.Mvvm;
using BitActionSwitch.Editor.Utility;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using BitActionSwitch.Scripts;
using UnityEngine;

namespace BitActionSwitch.Editor.ViewModels
{
    public class BitActionSwitchWindowViewModel : BindableBase
    {
        private VRCAvatarDescriptor avatarDescriptor;
        private AnimatorController animatorController;
        private VRCExpressionParameters expressionParameters;
        private Scripts.BitActionSwitch bitActionSwitch;
        private bool canReplaceExpressionMenu;

        /// <summary>
        /// ctor
        /// </summary>
        public BitActionSwitchWindowViewModel()
        {
            this.ApplyCommand = new DelegateCommand(this.ExecuteApply, this.CanApplyCommandExecute);
        }

        public Scripts.BitActionSwitch BitActionSwitch
        {
            get => this.bitActionSwitch;
            set
            {
                if (!this.SetProperty(ref this.bitActionSwitch, value)) return;
                if(value == null) return;
                this.AvatarDescriptor = value.targetAvatar;
            }
        }

        /// <summary>
        /// AvatarDescriptor
        /// </summary>
        public VRCAvatarDescriptor AvatarDescriptor
        {
            get => this.avatarDescriptor;
            set
            {
                if (!this.SetProperty(ref this.avatarDescriptor, value)) return;

                if (value != null)
                {
                    this.BitActionSwitch.targetAvatar = value;
                    this.AnimatorController = value.GetPlayableLayer(VRCAvatarDescriptor.AnimLayerType.FX);
                    this.ExpressionParameters = value.GetExpressionParameters();
                }
                else
                {
                    this.BitActionSwitch.targetAvatar = null;
                    this.AnimatorController = null;
                }
            }
        }

        /// <summary>
        /// FX AnimatorController
        /// </summary>
        public AnimatorController AnimatorController
        {
            get => this.animatorController;
            set
            {
                if (!this.SetProperty(ref this.animatorController, value)) return;
                this.AvatarDescriptor.SetPlayableLayer(value, VRCAvatarDescriptor.AnimLayerType.FX);
            }
        }

        /// <summary>
        /// ExpressionParameters
        /// </summary>
        public VRCExpressionParameters ExpressionParameters
        {
            get => this.expressionParameters;
            set
            {
                if(!this.SetProperty(ref this.expressionParameters, value)) return;
                this.AvatarDescriptor.SetExpressionParameters(value);
            }
        }
        

        /// <summary>
        /// Working directory is create anims directory.
        /// </summary>
        public string WorkingFolder
        {
            get => this.BitActionSwitch.workingFolder;
            set => this.SetProperty(ref this.BitActionSwitch.workingFolder, value);
        }

        /// <summary>
        /// Apply command
        /// </summary>
        public DelegateCommand ApplyCommand { get; }

        public bool IsErrorAvatar() => !this.SetError(this.AvatarDescriptor == null);

        public bool IsErrorAnimator() => !this.SetError(this.AnimatorController == null);
        
        public bool IsErrorParameter() => !this.SetError(this.ExpressionParameters == null);

        public bool IsErrorWorkingFolder() =>
            !this.SetError(string.IsNullOrEmpty(this.WorkingFolder) ||
                          !this.WorkingFolder.StartsWith("Assets"));

        public bool IsErrorExpressionMenu(BitActionSwitchGroup group) => !this.SetError(group.expressionsMenu == null);

        public bool IsErrorVariableName(string variableName)
        {
            var flag1 = this.SetError(variableName.HasInvalidChars());
            if (flag1) return false;
            
            var flag2 = this.SetError(this.bitActionSwitch.bitActionSwitchGroups
                .GroupBy(x => x.variableName.ToLower())
                .Count(x => x.Count() > 1) > 0);
            
            return !flag2;
        }

        public bool IsErrorGroupItemName(string groupItemName)
        {
            var flag1 = this.SetError(groupItemName.HasInvalidChars());
            if (flag1) return false;
            
            var flag2 = this.SetError(this.bitActionSwitch.bitActionSwitchGroups.All(x => x.bitActionSwitchItems
                .GroupBy(y => y.name.ToLower())
                .Count(y => y.Count() > 1) > 0));
            return !flag2;
        }

        public bool IsErrorRegisterGameObject(GameObject gameObject)
        {
            var flag1 = this.SetError(gameObject == null || this.BitActionSwitch.targetAvatar == null);
            if (flag1) return false;
            
            var flag2 = this.SetError(!gameObject.transform.IsChildOf(this.BitActionSwitch.targetAvatar.transform));
            if (flag2) return false;

            var flag3 = this.SetError(!this.BitActionSwitch.bitActionSwitchGroups
                .SelectMany(x => x.bitActionSwitchItems.SelectMany(y => y.gameObjects))
                .Where(x => x != null)
                .GroupBy(x => x.transform.GetHierarchyPath())
                .All(x => x.Count() < 2));

            return !flag3;
        }

        /// <summary>
        /// Can apply command execute
        /// </summary>
        // ReSharper disable once CognitiveComplexity
        private bool CanApplyCommandExecute()
        {
            // var hasError = this.HasError;
            // this.ClearErrors();
            // return !hasError;

            var flag = this.IsErrorAvatar() &&
                this.IsErrorAnimator() &&
                this.IsErrorParameter() &&
                this.IsErrorWorkingFolder();
            
            var flag2 = this.BitActionSwitch.bitActionSwitchGroups.All(this.IsErrorExpressionMenu);
            var flag3 = this.BitActionSwitch.bitActionSwitchGroups.All(x => this.IsErrorVariableName(x.variableName));
            var flag4 = this.BitActionSwitch.bitActionSwitchGroups.SelectMany(x => x.bitActionSwitchItems)
                .All(x => this.IsErrorGroupItemName(x.name));

            var flag5 = this.BitActionSwitch.bitActionSwitchGroups
                .SelectMany(x => x.bitActionSwitchItems)
                .Where(x => x.registerType == BitActionSwitchItem.RegisterType.GameObject)
                .SelectMany(x => x.gameObjects)
                .All(this.IsErrorRegisterGameObject);
            
            return flag && flag2 && flag3 && flag4 && flag5;
        }

        /// <summary>
        /// Apply command execute methods
        /// </summary>
        private void ExecuteApply()
        {
            var actionSwitch = new BitActionSwitchCreator(this.AvatarDescriptor, this.AnimatorController,
                    this.ExpressionParameters, this.BitActionSwitch, this.WorkingFolder);
            actionSwitch.Apply();

            EditorUtility.SetDirty(this.ExpressionParameters);
            EditorUtility.SetDirty(this.BitActionSwitch);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveOpenScenes();
        }

    }
}
#endif