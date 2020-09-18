using UnityEngine;

namespace BitActionSwitch.Editor.Models.Animation
{
    public class ActivationAnimationClipItem
    {
        public AnimationClip DefaultClip { get; set; }
        
        public AnimationClip NonDefaultClip { get; set; }
        
        public AnimationClip StaticDefaultClip { get; set; }
        
        public AnimationClip StaticNonDefaultClip { get; set; }
    }
}