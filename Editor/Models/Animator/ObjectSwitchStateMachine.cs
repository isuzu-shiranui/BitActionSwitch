#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace BitActionSwitch.Editor.Models.Animator
{
    internal static class ObjectSwitchStateMachine
    {
        public static AnimatorStateMachine CreateStateMachine(AnimatorStateMachine parentStateMachine, Vector3 position,
            int index, BitActionSwitchGroup bitActionSwitchGroup, AnimatorStateMachine bitCalculatorStateMachine, int groupIndex)
        {
            var name = (index + 1 + groupIndex * 9).ToString();
            var stateMachine = parentStateMachine.AddStateMachine($"Object {name} Switch", position);
            stateMachine.anyStatePosition = Style.AnyStatePosition;
            stateMachine.entryPosition = Style.EntryPosition;
            stateMachine.exitPosition = Style.ExitPosition;
            stateMachine.parentStateMachinePosition = Style.ParentStateMachinePosition;

            var topState = stateMachine.AddStateDefaultParam($"Object{name}", Style.ObjectTopStatePosition);
            stateMachine.defaultState = topState;
            
            var activeState = stateMachine.AddStateDefaultParam("Active", Style.ActiveStatePosition);
            activeState.motion = GlobalClips.ShortEmptyClip;
            var activeDriver = activeState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            activeDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = ActionSwitchParameters.GetObjectActiveStatusParameterName(index + 1 + groupIndex * 9),
                value = 1.0f
            });
            
            var inactiveState = stateMachine.AddStateDefaultParam("Inactive", Style.InactiveStatePosition);
            inactiveState.motion = GlobalClips.ShortEmptyClip;
            var inactiveDriver = inactiveState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            inactiveDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = ActionSwitchParameters.GetObjectActiveStatusParameterName(index + 1 + groupIndex * 9),
                value = 0.0f
            });

            var conditions = new List<AnimatorCondition>();

            for (var i = 0; i < 1 << bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                var targetDigit = ((1 << index) & i) == 0;
                if (targetDigit) // 0,2,4,6
                {
                    topState.AddTransitionDefaultParam(activeState, AnimatorConditionMode.Equals, i,
                        bitActionSwitchGroup.variableName);
                    conditions.Add(new AnimatorCondition
                    {
                        mode = AnimatorConditionMode.NotEqual,
                        threshold = i,
                        parameter = bitActionSwitchGroup.variableName
                    });
                    
                    // transition
                    inactiveState.AddTransitionDefaultParam(bitCalculatorStateMachine.states[i].state,
                        AnimatorConditionMode.Equals, (1 << index) ^ i, bitActionSwitchGroup.variableName);
                }
                else // 1,3,5,7
                {
                    activeState.AddTransitionDefaultParam(bitCalculatorStateMachine.states[i].state,
                        AnimatorConditionMode.Equals, (1 << index) ^ i, bitActionSwitchGroup.variableName);
                }
            }

            topState.AddTransitionDefaultParam(inactiveState, conditions.ToArray());

            return stateMachine;
        }
        
        private static class Style
        {
            public static readonly Vector3 AnyStatePosition = new Vector3(-275, 0);
            public static readonly Vector3 EntryPosition = new Vector3(25, -50);
            public static readonly Vector3 ExitPosition = new Vector3(-275, -50);  
            
            public static readonly Vector3 ParentStateMachinePosition = new Vector3(2, 300);
            public static readonly Vector3 ObjectTopStatePosition = new Vector3(0, 150);
            public static readonly Vector3 ActiveStatePosition = new Vector3(-250, 300);
            public static readonly Vector3 InactiveStatePosition = new Vector3(250, 300);
        }
    }
}
#endif