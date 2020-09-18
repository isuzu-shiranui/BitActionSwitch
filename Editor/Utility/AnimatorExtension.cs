#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BitActionSwitch.Editor.Utility
{
    public static class AnimatorExtension
    {
        public static AnimatorStateTransition AddTransitionDefaultParam(this AnimatorState sourceState, AnimatorState destinationState, bool addDefaultCondition = true)
        {
            var transition = sourceState.AddTransition(destinationState);
            SetDefaultTransitionParam(transition);
            return transition;
        }
        
        public static AnimatorStateTransition AddTransitionDefaultParam(this AnimatorState sourceState, AnimatorState destinationState, AnimatorCondition[] conditions)
        {
            var transition = sourceState.AddTransition(destinationState);
            SetDefaultTransitionParam(transition);
            transition.conditions = conditions;
            return transition;
        }
        
        public static AnimatorStateTransition AddTransitionDefaultParam(this AnimatorState sourceState, AnimatorState destinationState, AnimatorConditionMode mode, float threshold, string parameter)
        {
            var transition = sourceState.AddTransition(destinationState);
            SetDefaultTransitionParam(transition);
            transition.AddCondition(mode, threshold, parameter);
            return transition;
        }

        public static AnimatorStateTransition AddExitTransitionDefaultParam(this AnimatorState sourceState)
        {
            var transition = sourceState.AddExitTransition();
            SetDefaultTransitionParam(transition);
            return transition;
        }
        
        public static AnimatorStateTransition AddAnyTransitionDefaultParam(this AnimatorStateMachine stateMachine, AnimatorState destinationState)
        {
            var transition = stateMachine.AddAnyStateTransition(destinationState);
            SetDefaultTransitionParam(transition);
            return transition;
        }
        
        public static AnimatorStateTransition AddAnyTransitionDefaultParam(this AnimatorStateMachine stateMachine, AnimatorState destinationState, AnimatorConditionMode mode, float threshold, string parameter)
        {
            var transition = stateMachine.AddAnyStateTransition(destinationState);
            SetDefaultTransitionParam(transition);
            transition.AddCondition(mode, threshold, parameter);
            return transition;
        }

        public static AnimatorState AddStateDefaultParam(this AnimatorStateMachine stateMachine, string name, Vector3 position)
        {
            var state = stateMachine.AddState(name, position);
            SetDefaultStateParam(state);
            return state;
        }

        public static AnimatorState CreateBlendTreeInController(this AnimatorController animatorController, string name,
            out BlendTree tree, int layerIndex, string blendParameter, Vector3 position)
        {
            tree = new BlendTree {name = name, hideFlags = HideFlags.HideInHierarchy};
            tree.blendParameter = tree.blendParameterY = blendParameter;
            
            if (AssetDatabase.GetAssetPath(animatorController) != "")
                AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(animatorController));

            var state = animatorController.layers[layerIndex].stateMachine.AddState(tree.name, position);
            state.motion = tree;
            return state;
        }
        
        
        public static void AddChild(this BlendTree blendTree, Motion motion, string parameter)
        {
            blendTree.AddChild(motion, Vector2.zero, 0.0f, parameter);
        }
        
        public static void AddChild(this BlendTree blendTree, Motion motion, Vector2 position, string parameter)
        {
            blendTree.AddChild(motion, position, 0.0f, parameter);
        }
        
        public static void AddChild(this BlendTree blendTree, Motion motion, float threshold, string parameter)
        {
            blendTree.AddChild(motion, Vector2.zero, threshold, parameter);
        }
        
        public static void AddChild(this BlendTree blendTree, Motion motion, Vector2 position, float threshold, string parameter)
        {
            Undo.RecordObject(blendTree, "Added BlendTree Child");
            var children = blendTree.children;
            ArrayUtility.Add(ref children, new ChildMotion
            {
                timeScale = 1f,
                motion = motion,
                position = position,
                threshold = threshold,
                directBlendParameter = parameter
            });
            blendTree.children = children;
        }

        public static void SetMotion(this BlendTree blendTree, Motion motion, int index)
        {
            Undo.RecordObject(blendTree, "Set BlendTree Child Motion");
            var children = blendTree.children;
            var childMotion = blendTree.children[index];
            ArrayUtility.RemoveAt(ref children, index);
            ArrayUtility.Insert(ref children, index, new ChildMotion
            {
                timeScale = childMotion.timeScale,
                motion = motion,
                position = childMotion.position,
                threshold = childMotion.threshold,
                directBlendParameter = childMotion.directBlendParameter
            });
            blendTree.children = children;
        }

        public static void SetNormalizedBlendValues(this BlendTree blendTree, bool value)
        {
            var serializedObject = new SerializedObject(blendTree);
            var property = serializedObject.FindProperty("m_NormalizedBlendValues");
            
            serializedObject.Update();
            property.boolValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        public static ChildAnimatorState First(this ChildAnimatorState[] states, string name)
        {
            foreach (var t in states)
            {
                if (t.state.name == name) return t;
            }
            
            return default;
        }

        public static AnimatorControllerLayer AddLayerDefault(this AnimatorController controller, string name)
        {
            var layer = new AnimatorControllerLayer {name = controller.MakeUniqueLayerName(name), defaultWeight = 1.0f};
            layer.stateMachine = new AnimatorStateMachine {name = layer.name, hideFlags = HideFlags.HideInHierarchy};
            
            if (AssetDatabase.GetAssetPath(controller) != "")
                AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));
            controller.AddLayer(layer);
            return layer;
        }
        
        public static AnimatorControllerLayer AddLayerDefault(this AnimatorController controller, string name, AvatarMask avatarMask)
        {
            var layer = new AnimatorControllerLayer {name = controller.MakeUniqueLayerName(name), defaultWeight = 1.0f, avatarMask = avatarMask};
            layer.stateMachine = new AnimatorStateMachine {name = layer.name, hideFlags = HideFlags.HideInHierarchy};
            
            if (AssetDatabase.GetAssetPath(controller) != "")
                AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));
            controller.AddLayer(layer);
            return layer;
        }

        public static AnimatorControllerLayer[] ReorderLayer(this AnimatorController controller, int from, int to)
        {
            var layers = controller.layers;
            var array = new AnimatorControllerLayer[layers.Length];
            Array.Copy(layers, array, layers.Length);
            var tmp = array[from];
            for (var i = array.Length - 1; i >= to; i--)
            {
                array[i] = array[i - 1];
            }
            array[to] = tmp;
            Undo.RegisterCompleteObjectUndo(controller, "Layer reordering");
            return array;
        }

        private static void SetDefaultTransitionParam(AnimatorStateTransition transition)
        {
            transition.duration = 0.0f;
            transition.hasExitTime = false;
            transition.exitTime = 1.0f;
            transition.hasFixedDuration = false;
            transition.offset = 0;
            transition.canTransitionToSelf = false;
        }
        
        private static void SetDefaultStateParam(AnimatorState state)
        {
            state.writeDefaultValues = false;
            state.iKOnFeet = false;
            state.speed = 1.0f;
        }
    }
}
#endif