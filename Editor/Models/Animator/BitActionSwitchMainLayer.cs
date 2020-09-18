#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace BitActionSwitch.Editor.Models.Animator
{
    internal static class BitActionSwitchMainLayer
    {
        public static void Create(AnimatorStateMachine stateMachine, BitActionSwitchGroup bitActionSwitchGroup,
            int groupIndex)
        {
            stateMachine.entryPosition = Style.EntryPosition;
            stateMachine.anyStatePosition = Style.AnyStatePosition;
            stateMachine.exitPosition = Style.ExitPosition;
            
            var initState = CreateInitState(stateMachine, bitActionSwitchGroup.bitActionSwitchItems, groupIndex);
            stateMachine.defaultState = initState;

            var loadBitStartState = stateMachine.AddStateDefaultParam("Load Bit Start", Style.LoadBitStartStatePosition);
            loadBitStartState.motion = GlobalClips.ShortEmptyClip;
            var loadBitDriver = loadBitStartState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            loadBitDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = ActionSwitchParameters.InitializedParameterName,
                value = 0.0f
            });
            
            var effectStartState = stateMachine.AddStateDefaultParam("Effect State Start", Style.EffectStartStatePosition);
            effectStartState.motion = GlobalClips.ShortEmptyClip;
            
            loadBitStartState.AddTransitionDefaultParam(effectStartState, new[]
            {
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.NotEqual, 
                    threshold = 0.0f,
                    parameter = ActionSwitchParameters.ObjectNumParameterName
                },
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.If, 
                    threshold = 0.0f,
                    parameter = ActionSwitchParameters.VRCIsLocalParameterName
                }
            });
            
            var endState = stateMachine.AddStateDefaultParam("End", Style.EndStatePosition);

            var bitCalculatorStateMachine = BitCalculatorStateMachine.CreateStateMachine(stateMachine, Style.BitCalculatorStatePosition, bitActionSwitchGroup, loadBitStartState, endState, groupIndex);

            for (var i = 0; i < bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                var position = Style.ObjectSwitchStatePosition;
                position.x -= 250 * i;
                var objectSwitchStateMachine = ObjectSwitchStateMachine.CreateStateMachine(stateMachine, position, i,
                    bitActionSwitchGroup, bitCalculatorStateMachine, groupIndex);
                effectStartState.AddTransitionDefaultParam(objectSwitchStateMachine.defaultState,
                    AnimatorConditionMode.Equals, i + 1 + groupIndex * 9, ActionSwitchParameters.ObjectNumParameterName);
            }

            var initToLoad = initState.AddTransitionDefaultParam(loadBitStartState, false);
            initToLoad.hasExitTime = true;

            endState.AddTransitionDefaultParam(loadBitStartState, AnimatorConditionMode.If, 0.0f, ActionSwitchParameters.ThroughTransitionParameterName);
        }

        private static AnimatorState CreateInitState(AnimatorStateMachine stateMachine,
            IReadOnlyList<BitActionSwitchItem> actionSwitchItems, int groupIndex)
        {
            var state = stateMachine.AddStateDefaultParam("Init", Style.InitStatePosition);
            var avatarParameterDriver = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            avatarParameterDriver.parameters = new List<VRC_AvatarParameterDriver.Parameter>
            {
                new VRC_AvatarParameterDriver.Parameter
                {
                    name = ActionSwitchParameters.InitializedParameterName,
                    value = 1.0f
                }
            };
            
            avatarParameterDriver.parameters.AddRange(actionSwitchItems.Select((value, index) => 
                new VRC_AvatarParameterDriver.Parameter
                {
                    name = ActionSwitchParameters.GetObjectFloatStatusParameterName(index + 1 + groupIndex * 9),
                    value = 0.0f
                }));
            
            avatarParameterDriver.parameters.AddRange(actionSwitchItems.Select((value, index) => 
                new VRC_AvatarParameterDriver.Parameter
                {
                    name = ActionSwitchParameters.GetObjectActiveStatusParameterName(index + 1 + groupIndex * 9),
                    value = 0.0f
                }));
            
            return state;
        }
        
        private static class Style
        {
            public static readonly Vector3 AnyStatePosition = new Vector3(-275, 0);
            public static readonly Vector3 EntryPosition = new Vector3(25, -50);
            public static readonly Vector3 ExitPosition = new Vector3(-275, -50);  
            
            public static readonly Vector3 InitStatePosition = new Vector3(0, 0);
            public static readonly Vector3 LoadBitStartStatePosition = new Vector3(0, 100);
            public static readonly Vector3 BitCalculatorStatePosition = new Vector3(0, 200);
            public static readonly Vector3 ObjectSwitchStatePosition = new Vector3(0, 400);
            public static readonly Vector3 EffectStartStatePosition = new Vector3(250, 200);
            public static readonly Vector3 EndStatePosition = new Vector3(-250, 200);
        }
    }
}
#endif