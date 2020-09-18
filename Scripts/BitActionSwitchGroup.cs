#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace BitActionSwitch.Scripts
{
    [Serializable]
    public class BitActionSwitchGroup
    {
        [HideInInspector] public bool fold;
        // ReSharper disable once InconsistentNaming
        public VRCExpressionsMenu expressionsMenu;
        public string variableName;
        public List<BitActionSwitchItem> bitActionSwitchItems = new List<BitActionSwitchItem>();
    }
}
#endif