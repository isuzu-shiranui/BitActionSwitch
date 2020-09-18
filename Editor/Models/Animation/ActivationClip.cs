#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BitActionSwitch.Editor.Models.Animation
{
    internal static class ActivationClip
    {
        public static AnimationClip CreateObjectsActivateClip(string title, GameObject rootObject, IEnumerable<GameObject> targetObject, bool activate, string savePath)
        {
            var clip = new AnimationClip {name = $"{ActionSwitchParameters.PREFIX}{title}_{(activate ? "Default" : "NonDefault")}"};
            
            foreach (var t in targetObject)
            {
                var curveBinding = new EditorCurveBinding
                {
                    type = typeof(GameObject),
                    path = t.transform.GetHierarchyPath(rootObject.transform),
                    propertyName = "m_IsActive"
                };
                var curve = new AnimationCurve();

                if (activate)
                {
                    curve.AddKey(0.00f, t.activeSelf ? 1 : 0);
                    curve.AddKey(1.00f, t.activeSelf ? 1 : 0);
                }
                else
                {
                    curve.AddKey(0.00f, t.activeSelf ? 0 : 1);
                    curve.AddKey(1.00f, t.activeSelf ? 0 : 1);
                }
                
            
                AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
            }
            
            AssetDatabase.CreateAsset(clip, Path.Combine(savePath, clip.name + ".anim"));
            return clip;
        }
        
        public static AnimationClip CreateEmptyClip(string savePath, string title, float start, float end)
        {
            var clip = new AnimationClip {name = $"{ActionSwitchParameters.PREFIX}{title}"};
            var curveBinding = new EditorCurveBinding
            {
                type = typeof(GameObject),
                path = "none",
                propertyName = "m_IsActive"
            };
            
            var curve = new AnimationCurve();
            curve.AddKey(start, 1);
            curve.AddKey(end, 1);
            
            AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
            AssetDatabase.CreateAsset(clip, Path.Combine(savePath, clip.name + ".anim"));
            return clip;
        }
    }
}
#endif