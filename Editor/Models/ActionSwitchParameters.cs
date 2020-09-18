namespace BitActionSwitch.Editor.Models
{
    internal static class ActionSwitchParameters
    {
        public const string PREFIX = "bas_";
        
        public static string ObjectNumParameterName => $"{PREFIX}ObjectNum";

        public static string VRCSeatedParameterName => "Seated";
        
        public static string VRCIsLocalParameterName => "IsLocal";
        
        public static string InitializedParameterName => $"{PREFIX}Initialized";

        public static string GetLayerName(int groupIndex) => $"{PREFIX}MainLayer{groupIndex.ToString()}";

        public static string ThroughTransitionParameterName => $"{PREFIX}ThroughTransition";
        
        public static string GetObjectStatusLayerName(int objectNum) =>  $"{PREFIX}Object{objectNum.ToString()}Layer";

        public static string GetObjectActiveStatusParameterName(int objectNum) => $"{PREFIX}Is{objectNum.ToString()}Active";
        
        public static string GetObjectFloatStatusParameterName(int objectNum) =>  $"{PREFIX}Is{objectNum.ToString()}F";
    }
}