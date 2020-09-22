#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System;
using System.Linq;
using BitActionSwitch.Editor.Models.Animation;
using BitActionSwitch.Editor.Utility;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace BitActionSwitch.Editor.Models.Animator
{
    internal static class BitCalculatorStateMachine
    {
        public static AnimatorStateMachine CreateStateMachine(AnimatorStateMachine parentStateMachine, Vector3 position,
            BitActionSwitchGroup bitActionSwitchGroup,AnimatorState loadBitStartState, AnimatorState endState, int groupIndex)
        {
            var stateMachine = parentStateMachine.AddStateMachine("BitCalculator", position);
            stateMachine.anyStatePosition = Style.AnyStatePosition;
            stateMachine.entryPosition = Style.EntryPosition;
            stateMachine.exitPosition = Style.ExitPosition;
            stateMachine.parentStateMachinePosition = Style.ParentStateMachinePosition;

            var maxDigit = Convert.ToString(1 << (bitActionSwitchGroup.bitActionSwitchItems.Count - 1), 2).Length;
            var zeroState = stateMachine.AddStateDefaultParam("".PadLeft(maxDigit, '0'), Style.ZeroStatePosition);
            zeroState.motion = GlobalClips.ShortEmptyClip;
            
            var zeroDriver = zeroState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            zeroDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = bitActionSwitchGroup.variableName,
                value = 0.0f
            });
            
            zeroDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = ActionSwitchParameters.ObjectNumParameterName,
                value = 0.0f
            });
            
            zeroDriver.parameters.AddRange(bitActionSwitchGroup.bitActionSwitchItems.Select((value, index) => 
                new VRC_AvatarParameterDriver.Parameter
                {
                    name = ActionSwitchParameters.GetObjectFloatStatusParameterName(index + 1 + groupIndex * 9),
                    value = 0.0f
                }));
            
            zeroState.AddTransitionDefaultParam(endState, AnimatorConditionMode.NotEqual, 0.0f, ActionSwitchParameters.ObjectNumParameterName);
            zeroState.AddTransitionDefaultParam(endState, new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.IfNot,
                            parameter = ActionSwitchParameters.VRCIsLocalParameterName, 
                            threshold = 0.0f
                        },
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.NotEqual,
                            parameter = bitActionSwitchGroup.variableName, 
                            threshold = 0
                        },
                    });
            loadBitStartState.AddTransitionDefaultParam(zeroState, new[]
            {
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.IfNot,
                    parameter = ActionSwitchParameters.VRCIsLocalParameterName, 
                    threshold = 0.0f
                },
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals,
                    parameter = bitActionSwitchGroup.variableName, 
                    threshold = 0.0f
                },
            });
            
            var loadToZero = loadBitStartState.AddTransitionDefaultParam(zeroState, new[]
            {
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.If, 
                    parameter = ActionSwitchParameters.VRCIsLocalParameterName,
                    threshold = 0.0f
                },
                new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals, 
                    parameter = ActionSwitchParameters.ObjectNumParameterName,
                    threshold = 0.0f
                }, 
            });

            var conditions = loadToZero.conditions;
            ArrayUtility.AddRange(ref conditions, bitActionSwitchGroup.bitActionSwitchItems.Select((value, index) => new AnimatorCondition
            {
                mode = AnimatorConditionMode.IfNot,
                parameter = ActionSwitchParameters.GetObjectActiveStatusParameterName(index + 1),
                threshold = 0.0f
            }).ToArray());
            loadToZero.conditions = conditions;

            var stateIndex = 1;
            for (var i = 0; i < bitActionSwitchGroup.bitActionSwitchItems.Count; i++)
            {
                for (var j = 0; j < 1 << i; j++)
                {
                    var bitStatePosition = Style.ZeroStatePosition;
                    bitStatePosition.x -= 250 * (i + 1);
                    bitStatePosition.y += 100 * j;
                    var stateName = Convert.ToString(stateIndex, 2).PadLeft(maxDigit, '0');
                    
                    var progress = (float)i / bitActionSwitchGroup.bitActionSwitchItems.Count;
                    var info = $"{i + 1} / {bitActionSwitchGroup.bitActionSwitchItems.Count}({progress * 100:F2}%) - {stateName}";
                    EditorUtility.DisplayProgressBar ($"Create Group{groupIndex + 1} - Create States {bitActionSwitchGroup.bitActionSwitchItems[i].name}", info, progress);
                    
                    var bitState = stateMachine.AddStateDefaultParam(stateName, bitStatePosition);
                    bitState.motion = GlobalClips.ShortEmptyClip;

                    var bitDriver = bitState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                    bitDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                    {
                        name = bitActionSwitchGroup.variableName,
                        value = stateIndex
                    });

                    bitDriver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                    {
                        name = ActionSwitchParameters.ObjectNumParameterName,
                        value = 0.0f
                    });

                    var binaryArray = stateIndex.ToBinaryArray(maxDigit);
                    bitDriver.parameters.AddRange(Enumerable.Range(0, maxDigit).Select(x =>
                        new VRC_AvatarParameterDriver.Parameter
                        {
                            name = ActionSwitchParameters.GetObjectFloatStatusParameterName(x + 1 + groupIndex * 9),
                            value = binaryArray[x] ? 1.0f : 0.0f
                        }));

                    // transition
                    bitState.AddTransitionDefaultParam(endState, AnimatorConditionMode.NotEqual, 0.0f, ActionSwitchParameters.ObjectNumParameterName);

                    bitState.AddTransitionDefaultParam(endState, new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.IfNot,
                            parameter = ActionSwitchParameters.VRCIsLocalParameterName, 
                            threshold = 0.0f
                        },
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.NotEqual,
                            parameter = bitActionSwitchGroup.variableName, 
                            threshold = stateIndex
                        },
                    });
                    loadBitStartState.AddTransitionDefaultParam(bitState, new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.IfNot,
                            parameter = ActionSwitchParameters.VRCIsLocalParameterName, 
                            threshold = 0.0f
                        },
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.Equals,
                            parameter = bitActionSwitchGroup.variableName, 
                            threshold = stateIndex
                        },
                    });

                    var loadToBit = loadBitStartState.AddTransitionDefaultParam(bitState, new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.If, parameter = ActionSwitchParameters.VRCIsLocalParameterName,
                            threshold = 0.0f
                        },
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.Equals, 
                            parameter = ActionSwitchParameters.ObjectNumParameterName,
                            threshold = 0.0f
                        }, 
                    });

                    var animatorConditions = loadToBit.conditions;
                    ArrayUtility.AddRange(ref animatorConditions, bitActionSwitchGroup.bitActionSwitchItems.Select((value, index) => new AnimatorCondition
                    {
                        mode = binaryArray[index] ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot,
                        parameter = ActionSwitchParameters.GetObjectActiveStatusParameterName(index + 1 + groupIndex * 9),
                        threshold = 0.0f
                    }).ToArray());
                    loadToBit.conditions = animatorConditions;

                    stateIndex++;
                }
            }

            return stateMachine;
        }

        private static class Style
        {
            public static readonly Vector3 AnyStatePosition = new Vector3(-275, 0);
            public static readonly Vector3 EntryPosition = new Vector3(25, -50);
            public static readonly Vector3 ExitPosition = new Vector3(-275, -50);  
            
            public static readonly Vector3 ParentStateMachinePosition = new Vector3(0, 0);
            public static readonly Vector3 ZeroStatePosition = new Vector3(250, 150);
        }
    }
}
#endif