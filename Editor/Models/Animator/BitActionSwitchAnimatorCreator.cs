#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor.Animations;
using UnityEngine;

namespace BitActionSwitch.Editor.Models.Animator
{
    internal class BitActionSwitchAnimatorCreator
    {
        private readonly AnimatorController animatorController;

        public BitActionSwitchAnimatorCreator(AnimatorController animatorController)
        {
            this.animatorController = animatorController;
        }
        
        public void CreateBitActionSwitchAnimator(List<ActivationAnimationClipItem> animationClips, BitActionSwitchGroup bitActionSwitchGroup, int groupIndex)
        {
            this.AddAnimatorParameters(bitActionSwitchGroup, groupIndex);

            var mainLayer = this.animatorController.AddLayerDefault(ActionSwitchParameters.GetLayerName(groupIndex + 1));
            BitActionSwitchMainLayer.Create(mainLayer.stateMachine, bitActionSwitchGroup, groupIndex);

            for (var i = 0; i < bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                var animateLayer = this.animatorController.AddLayerDefault(ActionSwitchParameters.GetObjectStatusLayerName(i + 1 + groupIndex * 9));
                BitActionSwitchAnimateLayer.Create(animateLayer.stateMachine, i, bitActionSwitchGroup, animationClips[i], groupIndex);
            }
        }

        private void AddAnimatorParameters(BitActionSwitchGroup bitActionSwitchGroup, int groupIndex)
        {
            if(this.animatorController.parameters.All(x => x.name != ActionSwitchParameters.VRCSeatedParameterName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Bool,
                    name = ActionSwitchParameters.VRCSeatedParameterName,
                    defaultBool = false
                });
            }

            if(this.animatorController.parameters.All(x => x.name != ActionSwitchParameters.VRCIsLocalParameterName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Bool,
                    name = ActionSwitchParameters.VRCIsLocalParameterName,
                    defaultBool = false
                });
            }

            if (this.animatorController.parameters.All(x => x.name != bitActionSwitchGroup.variableName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Int,
                    name = bitActionSwitchGroup.variableName,
                    defaultInt = 0
                });
            }

            if (this.animatorController.parameters.All(x => x.name != ActionSwitchParameters.ObjectNumParameterName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Int,
                    name = ActionSwitchParameters.ObjectNumParameterName,
                    defaultInt = 0
                });
            }

            if (this.animatorController.parameters.All(x => x.name != ActionSwitchParameters.ThroughTransitionParameterName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Bool,
                    name = ActionSwitchParameters.ThroughTransitionParameterName,
                    defaultBool = true
                });
            }

            if (this.animatorController.parameters.All(x => x.name != ActionSwitchParameters.InitializedParameterName))
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Bool,
                    name = ActionSwitchParameters.InitializedParameterName,
                    defaultBool = false
                });
            }

            for (var i = 0; i < bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Bool,
                    name = ActionSwitchParameters.GetObjectActiveStatusParameterName(i + 1 + groupIndex * 9),
                    defaultBool = false
                });
                
                this.animatorController.AddParameter(new AnimatorControllerParameter
                {
                    type = AnimatorControllerParameterType.Float,
                    name = ActionSwitchParameters.GetObjectFloatStatusParameterName(i + 1 + groupIndex * 9),
                    defaultFloat = 0.0f
                });
            }
        }

        public void RemoveExistBitActionSwitchAnimator()
        {
            foreach (var animatorControllerLayer in this.animatorController.layers.Where(x => x.name.StartsWith(ActionSwitchParameters.PREFIX)))
            {
                var findIndex =
                    Array.FindIndex(this.animatorController.layers, x => x.name == animatorControllerLayer.name);
                this.animatorController.RemoveLayer(findIndex);
            }

            foreach (var animatorControllerParameter in this.animatorController.parameters.Where(x =>
                x.name.StartsWith(ActionSwitchParameters.PREFIX)))
            {
                this.animatorController.RemoveParameter(animatorControllerParameter);
            }
        }
    }
}
#endif