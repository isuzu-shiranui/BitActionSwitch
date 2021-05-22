#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Models.Animator;
using BitActionSwitch.Editor.Models.VRCObject;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Editor.Models
{
    public class BitActionSwitchCreator
    {
        private readonly VRCAvatarDescriptor avatarDescriptor;
        private readonly AnimatorController animatorController;
        private readonly VRCExpressionParameters expressionParameters;
        private readonly Scripts.BitActionSwitch bitActionSwitch;
        private readonly string workingDirectory;

        

        public BitActionSwitchCreator(VRCAvatarDescriptor avatarDescriptor, AnimatorController animatorController,
            VRCExpressionParameters expressionParameters, Scripts.BitActionSwitch bitActionSwitch, string workingDirectory)
        {
            this.animatorController = animatorController;
            this.expressionParameters = expressionParameters;
            this.bitActionSwitch = bitActionSwitch;
            this.avatarDescriptor = avatarDescriptor;
            this.workingDirectory = workingDirectory;
        }

        public void Apply()
        {
            EditorUtility.DisplayProgressBar ("Remove Exiting Items", "", 0f);
            var bitActionSwitchAnimator =
                new BitActionSwitchAnimatorCreator(this.animatorController);
            bitActionSwitchAnimator.RemoveExistBitActionSwitchAnimator();
            
            var expressionParameter = new ExpressionParameter(this.expressionParameters);
            expressionParameter.RemoveExistExpressionParameters();

            GlobalClips.ShortEmptyClip =
                ActivationClip.CreateEmptyClip(this.workingDirectory, "ShortWait", 0.00f, 0.01f);
            
            for (var i = 0; i < this.bitActionSwitch.bitActionSwitchGroups.Count; i++)
            {
                var progress = (float)i / this.bitActionSwitch.bitActionSwitchGroups.Count;
                var info = $"{i + 1} / {this.bitActionSwitch.bitActionSwitchGroups.Count}({progress * 100:F2}%)";
                EditorUtility.DisplayProgressBar ($"Create Group{i + 1}", info, progress);

                var bitActionSwitchGroup = this.bitActionSwitch.bitActionSwitchGroups[i];
                if (!bitActionSwitchGroup.variableName.StartsWith(ActionSwitchParameters.PREFIX))
                {
                    bitActionSwitchGroup.variableName = $"{ActionSwitchParameters.PREFIX}{bitActionSwitchGroup.variableName}";
                }
                
                var expressionMenu = new ExpressionMenu(bitActionSwitchGroup.expressionsMenu);

                expressionMenu.RemoveExistExpressionMenuControls();

                var animationClips = this.CreateAnimationClips(bitActionSwitchGroup, i);

                for (var j = 0; j < bitActionSwitchGroup.bitActionSwitchItems.Count; j++)
                {
                    expressionParameter.AddExpressionParameters(ActionSwitchParameters.GetObjectActiveStatusParameterName(j + 1 + i * 9));
                    expressionMenu.AddExpressionMenuControl(bitActionSwitchGroup.bitActionSwitchItems[j], j + 1 + i * 9);
                }

                bitActionSwitchAnimator.CreateBitActionSwitchAnimator(animationClips, bitActionSwitchGroup, i);
            }
            
            EditorUtility.ClearProgressBar();
        }

        private List<ActivationAnimationClipItem> CreateAnimationClips(BitActionSwitchGroup bitActionSwitchGroup, int index)
        {
            var result = new List<ActivationAnimationClipItem>();
            
            foreach (var bitActionSwitchItem in bitActionSwitchGroup.bitActionSwitchItems)
            {
                var clipItem = new ActivationAnimationClipItem();

                if (bitActionSwitchItem.registerType == BitActionSwitchItem.RegisterType.CustomAnim)
                {
                    clipItem.DefaultClip = bitActionSwitchItem.defaultClip;
                    clipItem.NonDefaultClip = bitActionSwitchItem.nonDefaultClip;
                    clipItem.StaticDefaultClip = bitActionSwitchItem.staticDefaultClip;
                    clipItem.StaticNonDefaultClip = bitActionSwitchItem.staticNonDefaultClip;
                }
                else
                {
                    var activeClip = ActivationClip.CreateObjectsActivateClip($"Group{(index + 1).ToString()}_{bitActionSwitchItem.name}",
                        this.avatarDescriptor.gameObject,
                        bitActionSwitchItem.gameObjects, true, this.workingDirectory);
                
                    var inactiveClip = ActivationClip.CreateObjectsActivateClip($"Group{(index + 1).ToString()}_{bitActionSwitchItem.name}", this.avatarDescriptor.gameObject,
                        bitActionSwitchItem.gameObjects, false, this.workingDirectory);

                    clipItem.DefaultClip = activeClip;
                    clipItem.NonDefaultClip = inactiveClip;
                }

                result.Add(clipItem);
            }

            return result;
        }
    }
}
#endif