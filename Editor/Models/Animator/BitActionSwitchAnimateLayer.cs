#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor.Animations;
using UnityEngine;

namespace BitActionSwitch.Editor.Models.Animator
{
    internal static class BitActionSwitchAnimateLayer
    {
        public static void Create(AnimatorStateMachine stateMachine, int index, BitActionSwitchGroup bitActionSwitchGroup,
            ActivationAnimationClipItem animationClip, int groupIndex)
        {
            stateMachine.entryPosition = Style.EntryPosition;
            stateMachine.anyStatePosition = Style.AnyStatePosition;
            stateMachine.exitPosition = Style.ExitPosition;
            
            var initState = stateMachine.AddStateDefaultParam("Init", Style.InitStatePosition);
            var staticBitActiveState = stateMachine.AddStateDefaultParam("Static Bit Active", Style.StaticBitActiveStatePosition);
            var staticBitInactiveState = stateMachine.AddStateDefaultParam("Static Bit Inactive", Style.StaticBitInactiveStatePosition);
            var bitActiveState = stateMachine.AddStateDefaultParam("Bit Active", Style.BitActiveStatePosition);
            var bitInactiveState = stateMachine.AddStateDefaultParam("Bit Inactive", Style.BitInactiveStatePosition);
            
            // set motion
            var bitActionSwitchItem = bitActionSwitchGroup.bitActionSwitchItems[index];
            if (bitActionSwitchItem.registerType == BitActionSwitchItem.RegisterType.GameObject)
            {
                staticBitActiveState.motion = animationClip.NonDefaultClip;
                staticBitInactiveState.motion = animationClip.DefaultClip;
                
                bitActiveState.motion = staticBitActiveState.motion;
                bitInactiveState.motion = staticBitInactiveState.motion;
            }
            else
            {
                staticBitActiveState.motion = bitActionSwitchItem.staticNonDefaultClip;
                staticBitInactiveState.motion = bitActionSwitchItem.staticDefaultClip;

                bitActiveState.motion = bitActionSwitchItem.defaultClip == null ? staticBitActiveState.motion : bitActionSwitchItem.nonDefaultClip;
                bitInactiveState.motion = bitActionSwitchItem.nonDefaultClip == null ? staticBitInactiveState.motion : bitActionSwitchItem.defaultClip;
            }

            // transition
            stateMachine.AddAnyTransitionDefaultParam(initState, AnimatorConditionMode.If, 0.0f, ActionSwitchParameters.InitializedParameterName);
            
            staticBitActiveState.AddTransitionDefaultParam(bitInactiveState, AnimatorConditionMode.Equals, index + 1 + groupIndex * 9,
                ActionSwitchParameters.ObjectNumParameterName);
            
            staticBitInactiveState.AddTransitionDefaultParam(bitActiveState, AnimatorConditionMode.Equals, index + 1 + groupIndex * 9,
                ActionSwitchParameters.ObjectNumParameterName);

            var activeToInactive = bitActiveState.AddTransitionDefaultParam(bitInactiveState, AnimatorConditionMode.Less, 0.5f,
                ActionSwitchParameters.GetObjectFloatStatusParameterName(index + 1 + groupIndex * 9));
            activeToInactive.hasExitTime = true;
            activeToInactive.exitTime = 1.0f;

            var inactiveToActive = bitInactiveState.AddTransitionDefaultParam(bitActiveState, AnimatorConditionMode.Greater, 0.5f,
                ActionSwitchParameters.GetObjectFloatStatusParameterName(index + 1 + groupIndex * 9));
            inactiveToActive.hasExitTime = true;
            inactiveToActive.exitTime = 1.0f;
            

            var conditions = new List<AnimatorCondition>();
            
            for (var i = 0; i < 1 << bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                var targetDigit = ((1 << index) & i) != 0;
                if (!targetDigit) continue;// 1,3,5,7
                
                initState.AddTransitionDefaultParam(staticBitActiveState, AnimatorConditionMode.Equals, i,
                    bitActionSwitchGroup.variableName);
                conditions.Add(new AnimatorCondition
                {
                    mode = AnimatorConditionMode.NotEqual,
                    threshold = i,
                    parameter = bitActionSwitchGroup.variableName
                });
            }
            
            initState.AddTransitionDefaultParam(staticBitInactiveState, conditions.ToArray());
        }
        
        private static class Style
        {
            public static readonly Vector3 AnyStatePosition = new Vector3(275, -100);
            public static readonly Vector3 EntryPosition = new Vector3(25, -100);
            public static readonly Vector3 ExitPosition = new Vector3(-226, -100);  
            
            public static readonly Vector3 InitStatePosition = new Vector3(0, 0);
            public static readonly Vector3 StaticBitActiveStatePosition = new Vector3(-250, 100);
            public static readonly Vector3 StaticBitInactiveStatePosition = new Vector3(250, 100);
            public static readonly Vector3 BitActiveStatePosition = new Vector3(250, 200);
            public static readonly Vector3 BitInactiveStatePosition = new Vector3(-250, 200);
        }
    }
}
#endif