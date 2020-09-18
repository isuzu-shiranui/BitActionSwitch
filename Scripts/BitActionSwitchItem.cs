#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BitActionSwitch.Scripts
{
    [Serializable]
    public class BitActionSwitchItem
    {
        public string name;
        public Texture2D icon;
        public List<GameObject> gameObjects = new List<GameObject>{null};
        public AnimationClip defaultClip;
        public AnimationClip nonDefaultClip;
        public AnimationClip staticDefaultClip;
        public AnimationClip staticNonDefaultClip;
        public RegisterType registerType;
        [HideInInspector] public bool fold;
        
        public enum RegisterType
        {
            GameObject,
            CustomAnim
        }
    }
}
#endif