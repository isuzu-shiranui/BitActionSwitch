#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Editor.Utility
{
    public static class VRCAvatarDescriptorExtension
    {
        public static AnimatorController GetPlayableLayer(this VRCAvatarDescriptor avatarDescriptor,
            VRCAvatarDescriptor.AnimLayerType animLayerType)
        {
            var customAnimLayer = avatarDescriptor.baseAnimationLayers?.First(x => x.type == animLayerType);
            if (customAnimLayer?.animatorController == null) return null;
            return (AnimatorController) customAnimLayer.Value.animatorController;
        }
        
        public static void SetPlayableLayer(this VRCAvatarDescriptor avatarDescriptor, AnimatorController animatorController,
            VRCAvatarDescriptor.AnimLayerType animLayerType)
        {
            var avatarDesctiptorSerializedObject = new SerializedObject(avatarDescriptor);
            avatarDesctiptorSerializedObject.Update();

            var indexOf = ArrayUtility.IndexOf(avatarDescriptor.baseAnimationLayers,
                avatarDescriptor.baseAnimationLayers.First(x => x.type == animLayerType));
            
            if(indexOf > -1)
            {
                avatarDescriptor.baseAnimationLayers[indexOf].isDefault = animatorController == null;
                avatarDescriptor.baseAnimationLayers[indexOf].animatorController = animatorController;
            }
            
            avatarDescriptor.customizeAnimationLayers = !avatarDescriptor.baseAnimationLayers.All(x => x.isDefault);

            avatarDesctiptorSerializedObject.ApplyModifiedProperties();
        }
        
        public static VRCExpressionParameters GetExpressionParameters(this VRCAvatarDescriptor avatarDescriptor)
        {
            return avatarDescriptor.expressionParameters;
        }
        
        public static void SetExpressionParameters(this VRCAvatarDescriptor avatarDescriptor, VRCExpressionParameters parameters)
        {
            var avatarDesctiptorSerializedObject = new SerializedObject(avatarDescriptor);
            avatarDesctiptorSerializedObject.Update();
            
            avatarDescriptor.expressionParameters = parameters;
            avatarDescriptor.customExpressions = avatarDescriptor.expressionParameters != null || avatarDescriptor.expressionsMenu != null;
            
            avatarDesctiptorSerializedObject.ApplyModifiedProperties();
        }
        
        public static VRCExpressionsMenu GetExpressionsMenu(this VRCAvatarDescriptor avatarDescriptor)
        {
            return avatarDescriptor.expressionsMenu;
        }
        
        public static void SetExpressionsMenu(this VRCAvatarDescriptor avatarDescriptor, VRCExpressionsMenu menu)
        {
            var avatarDesctiptorSerializedObject = new SerializedObject(avatarDescriptor);
            avatarDesctiptorSerializedObject.Update();
            
            avatarDescriptor.expressionsMenu = menu;
            avatarDescriptor.customExpressions = avatarDescriptor.expressionParameters != null || avatarDescriptor.expressionsMenu != null;
            
            avatarDesctiptorSerializedObject.ApplyModifiedProperties();
        }
    }
}
#endif