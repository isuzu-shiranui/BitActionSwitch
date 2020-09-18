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

        /// <summary>
        /// Can apply command execute
        /// </summary>
        // ReSharper disable once CognitiveComplexity
        private bool CanApplyCommandExecute()
        {
            var flag1 = this.avatarDescriptor != null && this.animatorController != null && this.expressionParameters != null;
            var flag2 = !string.IsNullOrEmpty(this.BitActionSwitch.workingFolder);
            var flag3 = this.BitActionSwitch.bitActionSwitchGroups.Count > 0;
            var flag4 = this.BitActionSwitch.bitActionSwitchGroups.All(x =>
            {
                return x.expressionsMenu != null && !string.IsNullOrEmpty(x.variableName) && 
                       this.BitActionSwitch
                    .bitActionSwitchGroups.GroupBy(y => y.variableName.ToLower())
                    .Count(y => y.Count() > 1) == 0 && 
                       x.bitActionSwitchItems.Count > 0 &&
                       x.bitActionSwitchItems
                           .GroupBy(y => y.name.ToLower())
                           .Count(y => y.Count() > 1) == 0 &&
                    x.bitActionSwitchItems.All(y =>
                    {
                        if (y.registerType == BitActionSwitchItem.RegisterType.GameObject)
                        {
                            if (y.gameObjects.Any(z => z == null) ||
                                !y.gameObjects.All(z => z.transform.IsChildOf(this.avatarDescriptor.transform)) ||
                                string.IsNullOrEmpty(y.name))
                            {
                                return false;
                            }

                            return true;
                        }

                        return y.staticDefaultClip != null && y.staticDefaultClip != null;
                    });
            });

            return flag1 && flag2 && flag3 && flag4;
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