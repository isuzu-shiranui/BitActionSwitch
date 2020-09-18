#if VRC_SDK_VRCSDK3
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace BitActionSwitch.Scripts
{
    [HelpURL("https://docs.google.com/document/d/15LbBSmKl798jN7_FA4wI03edudzhsb4iOVF2-WxgkZ0/edit?usp=sharing")] 
    public class BitActionSwitch : MonoBehaviour
    {
        public VRCAvatarDescriptor targetAvatar;
        public string workingFolder;
        public List<BitActionSwitchGroup> bitActionSwitchGroups = new List<BitActionSwitchGroup>();
    }
}
#endif